using System.Buffers.Binary;
using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.Common.CharacterAdd;

public class GameClientCharacterAddOutCodec : IPacketOutCodec<GameClientCharacterAddPacket>
{
    public ReadOnlyMemory<byte> Write(GameClientCharacterAddPacket packet)
    {
        var buffer = new byte[GameClientCharacterAddPacket.Size()];
        var span = buffer.AsSpan();
        var offset = 0;
        
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, sizeof(uint)), packet.Vid);
        offset += sizeof(uint);
        
        BinaryPrimitives.WriteSingleLittleEndian(span.Slice(offset, sizeof(uint)), packet.Angle);
        offset += sizeof(float);
        
        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(offset, sizeof(int)), packet.X);
        offset += sizeof(uint);
        
        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(offset, sizeof(int)), packet.Y);
        offset += sizeof(uint);
        
        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(offset, sizeof(int)), packet.Z);
        offset += sizeof(uint);

        span[offset++] = (byte)packet.CharacterType;
        
        BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, sizeof(ushort)), (ushort)packet.Race);
        offset += sizeof(ushort);
        
        span[offset++] = packet.MovingSpeed;
        span[offset++] = packet.AttackSpeed;
        span[offset++] = packet.StateFlag;
        
        foreach (var affectFlag in packet.AffectFlags)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, sizeof(uint)), affectFlag);
            offset += sizeof(uint);
        }
        
        return buffer;
    }
}