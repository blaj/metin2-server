using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.Common.Channel;

public class GameClientChannelOutPacket : IPacketOutCodec<GameClientChannelPacket>
{
    public ReadOnlyMemory<byte> Write(GameClientChannelPacket packet)
    {
        return new[] { packet.Channel };
    }
}