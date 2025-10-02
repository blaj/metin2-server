using System.Buffers.Binary;
using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Shared.Application.Handshake;

public class ClientGameHandshakeInCodec : IPacketInCodec<ClientGameHandshakePacket>
{
    public ClientGameHandshakePacket Read(ReadOnlySpan<byte> payload)
    {
        var handshake = BinaryPrimitives.ReadUInt32BigEndian(payload.Slice(0, 4));
        var time = BinaryPrimitives.ReadUInt32BigEndian(payload.Slice(4, 4));
        var delta = BinaryPrimitives.ReadInt32BigEndian(payload.Slice(8, 4));

        return new ClientGameHandshakePacket(handshake, time, delta);
    }
}