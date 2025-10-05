using System.Buffers.Binary;
using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.Move;

public class ClientGameMoveInCodec : IPacketInCodec<ClientGameMovePacket>
{
    public ClientGameMovePacket Read(ReadOnlySpan<byte> payload)
    {
        var offset = 0;

        var func = payload[offset++];
        var arg = payload[offset++];
        var rot = payload[offset++];

        var x = BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(offset, sizeof(int)));
        offset += sizeof(int);

        var y = BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(offset, sizeof(int)));
        offset += sizeof(int);

        var time = BinaryPrimitives.ReadUInt32LittleEndian(payload.Slice(offset, sizeof(int)));

        return new ClientGameMovePacket(func, arg, rot, x, y, time);
    }
}