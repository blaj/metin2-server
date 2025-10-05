using System.Buffers.Binary;
using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.MarkLogin;

public class ClientGameMarkLoginInCodec : IPacketInCodec<ClientGameMarkLoginPacket>
{
    public ClientGameMarkLoginPacket Read(ReadOnlySpan<byte> payload)
    {
        var offset = 0;

        var handle = BinaryPrimitives.ReadUInt32LittleEndian(payload.Slice(offset, sizeof(uint)));
        offset += sizeof(uint);

        var randomKey = BinaryPrimitives.ReadUInt32LittleEndian(payload.Slice(offset, sizeof(uint)));

        return new ClientGameMarkLoginPacket(handle, randomKey);
    }
}