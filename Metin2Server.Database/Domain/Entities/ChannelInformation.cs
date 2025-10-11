using Metin2Server.Shared.Enums;

namespace Metin2Server.Database.Domain.Entities;

public class ChannelInformation
{
    public required ushort Port { get; set; }
    public required byte ServerIndex { get; set; }
    public required ChannelStatus Status { get; set; }
    public uint OnlinePlayers { get; set; } = 0;
    public double CpuLoad { get; set; } = 0.0;
    public double MemoryLoad { get; set; } = 0.0;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}