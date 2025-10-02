using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.StateChecker;

public class ClientGameStateCheckerInCodec : IPacketInCodec<ClientGameStateCheckerPacket>
{
    public ClientGameStateCheckerPacket Read(ReadOnlySpan<byte> payload)
    {
        return new ClientGameStateCheckerPacket();
    }
}