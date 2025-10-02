using System.Net;
using Metin2Server.Channel.Features.CharacterCreate;
using Metin2Server.Channel.Features.CharacterDelete;
using Metin2Server.Channel.Features.Common.ChannelStatus;
using Metin2Server.Channel.Features.Common.CharacterCreateFailure;
using Metin2Server.Channel.Features.Common.CharacterCreateSuccess;
using Metin2Server.Channel.Features.Common.CharacterDeleteFailure;
using Metin2Server.Channel.Features.Common.CharacterDeleteSuccess;
using Metin2Server.Channel.Features.Common.Empire;
using Metin2Server.Channel.Features.Common.LoginSuccessNewslot;
using Metin2Server.Channel.Features.Empire;
using Metin2Server.Channel.Features.Login2;
using Metin2Server.Channel.Features.StateChecker;
using Metin2Server.Domain.Entities;
using Metin2Server.Network;
using Metin2Server.Shared.Application;
using Metin2Server.Shared.Application.Handshake;
using Metin2Server.Shared.Application.Phase;
using Metin2Server.Shared.Common;
using Metin2Server.Shared.DbContracts;
using Metin2Server.Shared.Enums;
using Metin2Server.Shared.Extensions;
using Metin2Server.Shared.Protocol;
using Metin2Server.Shared.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Empire = Metin2Server.Shared.Enums.Empire;

namespace Metin2Server.Channel;

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

                services.AddGrpcClient<BannedWordService.BannedWordServiceClient>("BannedWordClient",
                    options =>
                    {
                        options.Address =
                            new Uri(context.Configuration["Grpc:BannedWordService"]!);
                    });

                services.AddGrpcClient<CharacterService.CharacterServiceClient>("CharacterClient",
                    options =>
                    {
                        options.Address =
                            new Uri(context.Configuration["Grpc:CharacterService"]!);
                    });

                services.AddSlicesMediatR(typeof(ClientGameCharacterCreateCommandHandler).Assembly);
                services.AddSlicesMediatR(typeof(ClientGameHandshakeCommandHandler).Assembly);

                services.AddSingleton<TcpServer>();
                services.AddSingleton<ISessionAccessor, SessionAccessor>();
                services.AddSingleton<PacketOutCollector>();
                services.AddSingleton<PacketTransport>();
                services.AddSingleton<PacketSequencer>();

                services.AddSingleton<GameClientHandshakeOutCodec>();
                services.AddSingleton<GameClientPhaseOutCodec>();
                services.AddSingleton<GameClientLoginSuccessNewslotOutCodec>();
                services.AddSingleton<GameClientEmpireOutCodec>();
                services.AddSingleton<GameClientChannelStatusOutCodec>();
                services.AddSingleton<GameClientCharacterCreateFailureOutCodec>();
                services.AddSingleton<GameClientCharacterCreateSuccessOutCodec>();
                services.AddSingleton<GameClientCharacterDeleteSuccessOutCodec>();
                services.AddSingleton<GameClientCharacterDeleteFailureOutCodec>();
                services.AddSingleton<ClientGameHandshakeInCodec>();
                services.AddSingleton<ClientGameLogin2InCodec>();
                services.AddSingleton<ClientGameStateCheckerInCodec>();
                services.AddSingleton<ClientGameEmpireInCodec>();
                services.AddSingleton<ClientGameCharacterCreateInCodec>();
                services.AddSingleton<ClientGameCharacterDeleteInCodec>();

                services.AddSingleton<ISessionStartup, AuthHandshakeStartup>();

                services.AddSingleton<PacketRuleRegistryIn>(_ =>
                {
                    var registry = new PacketRuleRegistryIn();

                    registry.RegisterRule(
                        ClientGameHeader.Handshake,
                        new PacketRule(
                            PacketSizeKind.NoneFixed,
                            SequenceBehavior.None,
                            SessionPhase.Handshake | SessionPhase.Login | SessionPhase.Auth,
                            12,
                            null));

                    registry.RegisterRule(
                        ClientGameHeader.Login2,
                        new PacketRule(
                            PacketSizeKind.NoneFixed,
                            SequenceBehavior.ExpectInbound,
                            SessionPhase.Login | SessionPhase.Auth,
                            Constants.LoginMaxLength + 1 + sizeof(uint) + 4 * sizeof(uint),
                            null));

                    registry.RegisterRule(
                        ClientGameHeader.StateChecker,
                        new PacketRule(
                            PacketSizeKind.NoneFixed,
                            SequenceBehavior.None,
                            SessionPhase.Login | SessionPhase.Auth | SessionPhase.Handshake,
                            0,
                            null));

                    registry.RegisterRule(
                        ClientGameHeader.Empire,
                        new PacketRule(
                            PacketSizeKind.NoneFixed,
                            SequenceBehavior.ExpectInbound,
                            SessionPhase.SelectCharacter,
                            sizeof(Empire),
                            null));

                    registry.RegisterRule(
                        ClientGameHeader.CharacterCreate,
                        new PacketRule(
                            PacketSizeKind.NoneFixed,
                            SequenceBehavior.ExpectInbound,
                            SessionPhase.Login | SessionPhase.Auth | SessionPhase.Handshake |
                            SessionPhase.SelectCharacter,
                            ClientGameCharacterCreatePacket.Size(),
                            null));

                    registry.RegisterRule(
                        ClientGameHeader.CharacterDelete,
                        new PacketRule(
                            PacketSizeKind.NoneFixed,
                            SequenceBehavior.ExpectInbound,
                            SessionPhase.Login | SessionPhase.Auth | SessionPhase.Handshake |
                            SessionPhase.SelectCharacter,
                            ClientGameCharacterDeletePacket.Size(),
                            null));

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
                        GameClientHeader.LoginSuccessNewslot,
                        new PacketRule(
                            PacketSizeKind.NoneFixed,
                            SequenceBehavior.None,
                            SessionPhase.Login | SessionPhase.SelectCharacter,
                            328,
                            null));

                    registry.RegisterRule(
                        GameClientHeader.Empire,
                        new PacketRule(
                            PacketSizeKind.NoneFixed,
                            SequenceBehavior.None,
                            SessionPhase.Login | SessionPhase.SelectCharacter,
                            1,
                            null));

                    registry.RegisterRule(
                        GameClientHeader.RespondChannelStatus,
                        new PacketRule(
                            PacketSizeKind.NoneFixed,
                            SequenceBehavior.None,
                            SessionPhase.Login | SessionPhase.Auth | SessionPhase.Handshake,
                            sizeof(ushort) + sizeof(byte),
                            null));

                    registry.RegisterRule(
                        GameClientHeader.CharacterCreateFailure,
                        new PacketRule(
                            PacketSizeKind.NoneFixed,
                            SequenceBehavior.None,
                            SessionPhase.Login | SessionPhase.SelectCharacter,
                            GameClientCharacterCreateFailurePacket.Size(),
                            null));

                    registry.RegisterRule(
                        GameClientHeader.CharacterCreateSuccess,
                        new PacketRule(
                            PacketSizeKind.NoneFixed,
                            SequenceBehavior.None,
                            SessionPhase.Login | SessionPhase.SelectCharacter,
                            GameClientCharacterCreateSuccessPacket.Size(),
                            null));
                    
                    registry.RegisterRule(
                        GameClientHeader.CharacterDeleteSuccess,
                        new PacketRule(
                            PacketSizeKind.NoneFixed,
                            SequenceBehavior.None,
                            SessionPhase.Login | SessionPhase.SelectCharacter,
                            GameClientCharacterDeleteSuccessPacket.Size(),
                            null));
                    
                    registry.RegisterRule(
                        GameClientHeader.CharacterDeleteFailure,
                        new PacketRule(
                            PacketSizeKind.NoneFixed,
                            SequenceBehavior.None,
                            SessionPhase.Login | SessionPhase.SelectCharacter,
                            GameClientCharacterDeleteFailurePacket.Size(),
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
                        ClientGameHeader.Login2,
                        serviceProvider.GetRequiredService<ClientGameLogin2InCodec>(),
                        packet => new ClientGameLogin2Command(
                            StringUtils.FromCBuffer(packet.Username),
                            packet.LoginKey,
                            packet.AdwClientKey));

                    registry.Map(
                        ClientGameHeader.StateChecker,
                        serviceProvider.GetRequiredService<ClientGameStateCheckerInCodec>(),
                        _ => new ClientGameStateCheckerCommand());

                    registry.Map(
                        ClientGameHeader.Empire,
                        serviceProvider.GetRequiredService<ClientGameEmpireInCodec>(),
                        packet => new ClientGameEmpireCommand(packet.Empire));

                    registry.Map(
                        ClientGameHeader.CharacterCreate,
                        serviceProvider.GetRequiredService<ClientGameCharacterCreateInCodec>(),
                        packet => new ClientGameCharacterCreateCommand(
                            packet.Index,
                            StringUtils.FromCBuffer(packet.Name),
                            (Race)packet.Race,
                            packet.Shape));

                    registry.Map(
                        ClientGameHeader.CharacterDelete,
                        serviceProvider.GetRequiredService<ClientGameCharacterDeleteInCodec>(),
                        packet => new ClientGameCharacterDeleteCommand(
                            packet.Index,
                            StringUtils.FromCBuffer(packet.PrivateCode)));

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
                        GameClientHeader.LoginSuccessNewslot,
                        serviceProvider.GetRequiredService<GameClientLoginSuccessNewslotOutCodec>());

                    registry.Map(
                        GameClientHeader.Empire,
                        serviceProvider.GetRequiredService<GameClientEmpireOutCodec>());

                    registry.Map(
                        GameClientHeader.RespondChannelStatus,
                        serviceProvider.GetRequiredService<GameClientChannelStatusOutCodec>());

                    registry.Map(
                        GameClientHeader.CharacterCreateFailure,
                        serviceProvider.GetRequiredService<GameClientCharacterCreateFailureOutCodec>());

                    registry.Map(
                        GameClientHeader.CharacterCreateSuccess,
                        serviceProvider.GetRequiredService<GameClientCharacterCreateSuccessOutCodec>());

                    registry.Map(
                        GameClientHeader.CharacterDeleteSuccess,
                        serviceProvider.GetRequiredService<GameClientCharacterDeleteSuccessOutCodec>());
                    
                    registry.Map(
                        GameClientHeader.CharacterDeleteFailure,
                        serviceProvider.GetRequiredService<GameClientCharacterDeleteFailureOutCodec>());
                    
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