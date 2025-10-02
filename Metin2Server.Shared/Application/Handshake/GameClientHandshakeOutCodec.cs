using System.Buffers.Binary;
using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Shared.Application.Handshake;

public class GameClientHandshakeOutCodec : IPacketOutCodec<GameClientHandshakePacket>
{
    public ReadOnlyMemory<byte> Write(GameClientHandshakePacket gameClientHandshakePacket)
    {
        var buf = new byte[12];
        var span = buf.AsSpan();
        
        BinaryPrimitives.WriteUInt32BigEndian(span.Slice(0, 4), gameClientHandshakePacket.Handshake);
        BinaryPrimitives.WriteUInt32BigEndian(span.Slice(4, 4), gameClientHandshakePacket.CurrentTime);
        BinaryPrimitives.WriteInt32BigEndian (span.Slice(8, 4), gameClientHandshakePacket.Delta);
        
        return buf;
    }
}