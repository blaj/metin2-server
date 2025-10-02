using System.Net;
using Metin2Server.Auth.Features.Login3;
using Metin2Server.Network;
using Metin2Server.Shared.Application;
using Metin2Server.Shared.Application.AuthSuccess;
using Metin2Server.Shared.Application.Handshake;
using Metin2Server.Shared.Application.LoginFailure;
using Metin2Server.Shared.Application.Phase;
using Metin2Server.Shared.Application.TimeSync;
using Metin2Server.Shared.Common;
using Metin2Server.Shared.DbContracts;
using Metin2Server.Shared.Encryption;
using Metin2Server.Shared.Extensions;
using Metin2Server.Shared.Protocol;
using Metin2Server.Shared.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Metin2Server.Auth;

public class Program
{
    public static async Task Main(string[] args)
    {
        using var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, config) =>
            {
                config.Sources.Clear();

                config
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddLogging(config => config.AddSerilog());

                services.AddGrpcClient<DbService.DbServiceClient>("DbClient",
                    options =>
                    {
                        options.Address =
                            new Uri(context.Configuration["Grpc:DbService"]!);
                    });
                
                services.AddGrpcClient<LoginKeyService.LoginKeyServiceClient>("LoginKeyClient",
                    options =>
                    {
                        options.Address =
                            new Uri(context.Configuration["Grpc:LoginKeyService"]!);
                    });

                services.AddSlicesMediatR(typeof(ClientGameLogin3CommandHandler).Assembly);
                services.AddSlicesMediatR(typeof(ClientGameHandshakeCommandHandler).Assembly);

                services.AddSingleton<TcpServer>();
                services.AddSingleton<ISessionAccessor, SessionAccessor>();
                services.AddSingleton<PacketOutCollector>();
                services.AddSingleton<PacketTransport>();
                services.AddSingleton<PacketSequencer>();
                services.AddSingleton<PasswordHasherService>(_ =>
                    new PasswordHasherService(context.Configuration["Security:Pepper"]!));

                services.AddSingleton<ISessionStartup, AuthHandshakeStartup>();

                services.AddSingleton<GameClientHandshakeOutCodec>();
                services.AddSingleton<GameClientPhaseOutCodec>();
                services.AddSingleton<GameClientTimeSyncOutCodec>();
                services.AddSingleton<GameClientLoginFailureOutCodec>();
                services.AddSingleton<GameClientAuthSuccessOutCodec>();
                services.AddSingleton<ClientGameHandshakeInCodec>();
                services.AddSingleton<ClientGameLogin3InCodec>();

                services.AddSingleton<PacketRuleRegistryIn>(_ =>
                {
                    var registry = new PacketRuleRegistryIn();

                    registry.RegisterRule(
                        ClientGameHeader.Handshake,
                        new PacketRule(
                            PacketSizeKind.NoneFixed,
                            SequenceBehavior.None,
                            SessionPhase.Handshake,
                            12,
                            null));

                    registry.RegisterRule(
                        ClientGameHeader.Login3,
                        new PacketRule(
                            PacketSizeKind.NoneFixed,
                            SequenceBehavior.ExpectInbound,
                            SessionPhase.Login | SessionPhase.Auth,
                            Constants.LoginMaxLength + 1 + Constants.PasswordMaxLength + 1 + 4 * sizeof(uint),
                            0));

                    return registry;
                });

                services.AddSingleton<PacketRuleRegistryOut>(_ =>
                {
                    var registry = new PacketRuleRegistryOut();

                    registry.RegisterRule(
                        GameClientHeader.Handshake,
                        new PacketRule(
                            PacketSizeKind.NoneFixed,
                            SequenceBehavior.None,
                            SessionPhase.Handshake,
                            12,
                            null));

                    registry.RegisterRule(
                        GameClientHeader.Phase,
                        new PacketRule(
                            PacketSizeKind.NoneFixed,
                            SequenceBehavior.None,
                            SessionPhase.Handshake | SessionPhase.Login | SessionPhase.InGame,
                            1,
                            null));

                    registry.RegisterRule(
                        GameClientHeader.TimeSync,
                        new PacketRule(
                            PacketSizeKind.NoneFixed,
                            SequenceBehavior.None,
                            SessionPhase.Handshake | SessionPhase.Login | SessionPhase.InGame,
                            0,
                            null));

                    registry.RegisterRule(
                        GameClientHeader.LoginFailure,
                        new PacketRule(
                            PacketSizeKind.NoneFixed,
                            SequenceBehavior.None,
                            SessionPhase.Handshake | SessionPhase.Login | SessionPhase.Auth,
                            Constants.LoginStatusMaxLength + 1,
                            null));

                    registry.RegisterRule(
                        GameClientHeader.AuthSuccess,
                        new PacketRule(
                            PacketSizeKind.NoneFixed,
                            SequenceBehavior.None,
                            SessionPhase.Auth,
                            5,
                            null));

                    return registry;
                });

                services.AddSingleton<PacketInRegistry>(serviceProvider =>
                {
                    var registry = new PacketInRegistry();

                    registry.Map(
                        ClientGameHeader.Handshake,
                        serviceProvider.GetRequiredService<ClientGameHandshakeInCodec>(),
                        packet => new ClientGameHandshakeCommand(packet.Handshake, packet.CurrentTime, packet.Delta));

                    registry.Map(
                        ClientGameHeader.Login3,
                        serviceProvider.GetRequiredService<ClientGameLogin3InCodec>(),
                        packet => new ClientGameLogin3Command(
                            StringUtils.FromCBuffer(packet.Username),
                            StringUtils.FromCBuffer(packet.Password),
                            packet.AdwClientKey));

                    return registry;
                });

                services.AddSingleton<PacketOutRegistry>(serviceProvider =>
                {
                    var registry = new PacketOutRegistry();

                    registry.Map(
                        GameClientHeader.Handshake,
                        serviceProvider.GetRequiredService<GameClientHandshakeOutCodec>());

                    registry.Map(
                        GameClientHeader.Phase,
                        serviceProvider.GetRequiredService<GameClientPhaseOutCodec>());

                    registry.Map(
                        GameClientHeader.TimeSync,
                        serviceProvider.GetRequiredService<GameClientTimeSyncOutCodec>());

                    registry.Map(
                        GameClientHeader.LoginFailure,
                        serviceProvider.GetRequiredService<GameClientLoginFailureOutCodec>());

                    registry.Map(
                        GameClientHeader.AuthSuccess,
                        serviceProvider.GetRequiredService<GameClientAuthSuccessOutCodec>());

                    return registry;
                });
            })
            .Build();

        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Starting...");

        using var cancellationTokenSource = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cancellationTokenSource.Cancel();
        };

        
        var configuration = host.Services.GetRequiredService<IConfiguration>();
        var tcp = host.Services.GetRequiredService<TcpServer>();
        _ = tcp.StartAsync(IPAddress.Loopback, Convert.ToInt32(configuration["port"]), cancellationTokenSource.Token);

        await host.RunAsync(cancellationTokenSource.Token);
    }
}