using System.Buffers.Binary;
using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.Common.CharacterPoints;

public class GameClientCharacterPointsOutCodec : IPacketOutCodec<GameClientCharacterPointsPacket>
{
    public ReadOnlyMemory<byte> Write(GameClientCharacterPointsPacket packet)
    {
        var buffer = new byte[GameClientCharacterPointsPacket.Size()];
        var span = buffer.AsSpan();
        var offset = 0;

        foreach (var point in packet.Points)
        {
            BinaryPrimitives.WriteInt32LittleEndian(span.Slice(offset, sizeof(int)), point);
            offset += sizeof(int);
        }
        
        return buffer;
    }
}