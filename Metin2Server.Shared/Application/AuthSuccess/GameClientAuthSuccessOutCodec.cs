using System.Buffers.Binary;
using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Shared.Application.AuthSuccess;

public class GameClientAuthSuccessOutCodec : IPacketOutCodec<GameClientAuthSuccessPacket>
{
    public ReadOnlyMemory<byte> Write(GameClientAuthSuccessPacket gameClientHandshakePacket)
    {
        var buffer = new byte[5];
        
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(0, 4), gameClientHandshakePacket.LoginKey);
        buffer[4] = gameClientHandshakePacket.Result;
        
        return buffer;
    }
}