using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Shared.Application.Phase;

public class GameClientPhaseOutCodec : IPacketOutCodec<GameClientPhasePacket>
{
    public ReadOnlyMemory<byte> Write(GameClientPhasePacket gameClientPhasePacket)
    {
        return new[] { (byte)gameClientPhasePacket.Phase };
    }
}