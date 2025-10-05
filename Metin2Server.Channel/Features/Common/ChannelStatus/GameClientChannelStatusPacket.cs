namespace Metin2Server.Channel.Features.Common.ChannelStatus;

public record GameClientChannelStatusPacket(uint Size, (ushort Port, Shared.Enums.ChannelStatus Status)[] Channels);