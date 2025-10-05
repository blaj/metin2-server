using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.CharacterSelect;

public class ClientGameCharacterSelectInCodec : IPacketInCodec<ClientGameCharacterSelectPacket>
{
    public ClientGameCharacterSelectPacket Read(ReadOnlySpan<byte> payload)
    {
        return new ClientGameCharacterSelectPacket(payload[0]);
    }
}