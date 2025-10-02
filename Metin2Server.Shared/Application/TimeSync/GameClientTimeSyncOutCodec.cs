using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Shared.Application.TimeSync;

public class GameClientTimeSyncOutCodec : IPacketOutCodec<GameClientTimeSyncPacket>
{
    public ReadOnlyMemory<byte> Write(GameClientTimeSyncPacket gameClientHandshakePacket)
    {
        return ReadOnlyMemory<byte>.Empty;
    }
}