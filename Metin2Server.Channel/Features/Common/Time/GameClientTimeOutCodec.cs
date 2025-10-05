using System.Buffers.Binary;
using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.Common.Time;

public class GameClientTimeOutCodec : IPacketOutCodec<GameClientTimePacket>
{
    public ReadOnlyMemory<byte> Write(GameClientTimePacket packet)
    {
        var buffer = new byte[GameClientTimePacket.Size()];
        var span = buffer.AsSpan();
        
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(0, sizeof(uint)), packet.Time);

        return buffer;
    }
}