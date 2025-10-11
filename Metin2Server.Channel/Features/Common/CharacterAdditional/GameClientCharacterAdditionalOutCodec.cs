using System.Buffers.Binary;
using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.Common.CharacterAdditional;

public class GameClientCharacterAdditionalOutCodec : IPacketOutCodec<GameClientCharacterAdditionalPacket>
{
    public ReadOnlyMemory<byte> Write(GameClientCharacterAdditionalPacket packet)
    {
        var buffer = new byte[GameClientCharacterAdditionalPacket.Size()];
        var span = buffer.AsSpan();
        var offset = 0;
        
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, sizeof(uint)), packet.Vid);
        offset += sizeof(uint);

        foreach (var name in packet.Name)
        {
            span[offset++] = (byte)name;
        }

        foreach (var part in packet.Parts)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, sizeof(ushort)), part);
            offset += sizeof(ushort);
        }

        span[offset++] = (byte)packet.Empire;
        
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, sizeof(uint)), packet.GuildId);
        offset += sizeof(uint);
        
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, sizeof(uint)), packet.Level);
        offset += sizeof(uint);
        
        BinaryPrimitives.WriteInt16LittleEndian(span.Slice(offset, sizeof(short)), packet.Alignment);
        offset += sizeof(short);

        span[offset++] = (byte) packet.PkMode;
        
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, sizeof(uint)), packet.MountVnum);
        
        return buffer;
    }
}