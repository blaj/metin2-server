using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.Empire;

public class ClientGameEmpireInCodec : IPacketInCodec<ClientGameEmpirePacket>
{
    public ClientGameEmpirePacket Read(ReadOnlySpan<byte> payload)
    {
        return new ClientGameEmpirePacket((Shared.Enums.Empire)payload[0]);
    }
}