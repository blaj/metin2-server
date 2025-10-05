using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.EnterGame;

public class ClientGameEnterGameInCodec : IPacketInCodec<ClientGameEnterGamePacket>
{
    public ClientGameEnterGamePacket Read(ReadOnlySpan<byte> payload)
    {
        return new ClientGameEnterGamePacket();
    }
}