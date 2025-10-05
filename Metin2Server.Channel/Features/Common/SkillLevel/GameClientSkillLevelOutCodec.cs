using System.Buffers.Binary;
using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.Common.SkillLevel;

public class GameClientSkillLevelOutCodec : IPacketOutCodec<GameClientSkillLevelPacket>
{
    public ReadOnlyMemory<byte> Write(GameClientSkillLevelPacket packet)
    {
        var buffer = new byte[GameClientSkillLevelPacket.Size()];
        var span = buffer.AsSpan();
        var offset = 0;

        foreach (var skill in packet.Skills)
        {
            BinaryPrimitives.WriteInt32LittleEndian(span.Slice(offset, sizeof(int)), skill);
            offset += sizeof(int);
        }
        
        return buffer;
    }
}