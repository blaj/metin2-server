using System.Net;
using Metin2Server.Infrastructure;
using Metin2Server.Network;
using Metin2Server.Shared.Protocol;
using Metin2Server.Slices.PlayerLogin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Metin2Server.Channel;

public class Program
{
    public static async Task Main(string[] args)
    {
        using var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddLogging(config => config.AddSerilog());

                services.AddSlicesMediatR(typeof(PlayerLoginHandler).Assembly);

                services.AddSingleton<TcpServer>();
                services.AddSingleton<PacketRuleRegistryIn>();
                services.AddSingleton<PacketRuleRegistryOut>(_ =>
                {
                    var registry = new PacketRuleRegistryOut();
                    
                    registry.RegisterRule(
                        GameClientHeader.Handshake,
                        new PacketRule(
                            PacketSizeKind.TotalSize,
                            SequenceBehavior.None, 
                            SessionPhase.Handshake,
                            4,
                            null));

                    return registry;
                });
                services.AddSingleton<PacketInRegistry>();
                services.AddSingleton<PacketOutRegistry>();
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

        var tcp = host.Services.GetRequiredService<TcpServer>();
        _ = tcp.StartAsync(IPAddress.Loopback, 11000, cancellationTokenSource.Token);

        await host.RunAsync();
    }
}