using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.Common.ItemSet;

public class GameClientItemSetOutCodec : IPacketOutCodec<GameClientItemSetPacket>
{
    public ReadOnlyMemory<byte> Write(GameClientItemSetPacket packet)
    {
        throw new NotImplementedException();
    }
}