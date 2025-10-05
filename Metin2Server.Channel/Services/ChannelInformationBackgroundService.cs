using System.Diagnostics;
using Metin2Server.Shared.DbContracts;
using Metin2Server.Shared.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ChannelStatus = Metin2Server.Shared.Enums.ChannelStatus;

namespace Metin2Server.Channel.Services;

public class ChannelInformationBackgroundService : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(60);

    private readonly byte _serverIndex;
    private readonly ushort _port;
    private readonly ChannelInformationService.ChannelInformationServiceClient _channelInformationServiceClient;
    private readonly ILogger<ChannelInformationBackgroundService> _logger;

    private readonly int _cpuCount = Environment.ProcessorCount;
    
    private TimeSpan _lastCpu = TimeSpan.Zero;
    private DateTime _lastWall = DateTime.MinValue;
    
    public ChannelInformationBackgroundService(
        byte serverIndex,
        ushort port,
        ChannelInformationService.ChannelInformationServiceClient channelInformationServiceClient,
        ILogger<ChannelInformationBackgroundService> logger)
    {
        _serverIndex = serverIndex;
        _port = port;
        _channelInformationServiceClient = channelInformationServiceClient;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var process = Process.GetCurrentProcess();
        _lastCpu = process.TotalProcessorTime;
        _lastWall = DateTime.UtcNow;

        await SendAsync(process, cancellationToken);
        
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(Interval, cancellationToken);
            process.Refresh();
            await SendAsync(process, cancellationToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _channelInformationServiceClient.RemoveChannelInformationAsync(
            new RemoveChannelInformationRequest { Port = _port },
            cancellationToken: cancellationToken);

        await base.StopAsync(cancellationToken);
    }
    
    private async Task SendAsync(Process process, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var cpuDeltaMs  = (process.TotalProcessorTime - _lastCpu).TotalMilliseconds;
        var wallDeltaMs = (now - _lastWall).TotalMilliseconds;
        var cpuPct = wallDeltaMs <= 0 ? 0d : (cpuDeltaMs / (wallDeltaMs * _cpuCount)) * 100d;
        
        if (cpuPct < 0)
        {
            cpuPct = 0;
        }
        
        if (cpuPct > 100)
        {
            cpuPct = 100;
        }

        _lastCpu = process.TotalProcessorTime;
        _lastWall = now;

        var workingSetMb = process.WorkingSet64 / (1024d * 1024d);

        try
        {
            await _channelInformationServiceClient.SendChannelInformationAsync(new SendChannelInformationRequest
            {
                ChannelInformation = new ChannelInformation
                {
                    Port = _port,
                    ServerIndex = _serverIndex,
                    Status = ChannelStatus.Online.ToProto(),
                    OnlinePlayers = 0,
                    CpuLoad = cpuPct,
                    MemoryLoad = workingSetMb
                }
            }, cancellationToken: cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to send channel information");
        }
    }
}