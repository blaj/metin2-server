namespace Metin2Server.Channel.Features.Common.ChannelStatus;

public record GameClientChannelStatusPacket(ushort Port, ChannelStatus Status);