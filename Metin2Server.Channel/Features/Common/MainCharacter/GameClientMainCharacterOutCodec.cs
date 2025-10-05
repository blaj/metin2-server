using System.Buffers.Binary;
using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.Common.MainCharacter;

public class GameClientMainCharacterOutCodec : IPacketOutCodec<GameClientMainCharacterPacket>
{
    public ReadOnlyMemory<byte> Write(GameClientMainCharacterPacket packet)
    {
        var buffer = new byte[GameClientMainCharacterPacket.Size()];
        var span = buffer.AsSpan();
        var offset = 0;

        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, sizeof(uint)), packet.Vid);
        offset += sizeof(uint);

        BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, sizeof(ushort)), (ushort)packet.Race);
        offset += sizeof(ushort);

        foreach (var name in packet.Name)
        {
            span[offset++] = (byte)name;
        }
        
        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(offset, sizeof(int)), packet.X);
        offset += sizeof(int);
        
        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(offset, sizeof(int)), packet.Y);
        offset += sizeof(int);
        
        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(offset, sizeof(int)), packet.Z);
        offset += sizeof(int);

        span[offset++] = (byte)packet.Empire;

        span[offset] = packet.SkillGroup;
        
        return buffer;
    }
}