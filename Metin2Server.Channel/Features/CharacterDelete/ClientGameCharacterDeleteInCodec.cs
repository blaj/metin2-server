using Metin2Server.Shared.Common;
using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.CharacterDelete;

public class ClientGameCharacterDeleteInCodec : IPacketInCodec<ClientGameCharacterDeletePacket>
{
    public ClientGameCharacterDeletePacket Read(ReadOnlySpan<byte> payload)
    {
        var offset = 0;

        var index = payload[offset++];

        var privateCode = new char[Constants.PrivateCodeLength];
        for (var i = 0; i < Constants.PrivateCodeLength; i++)
        {
            privateCode[i] = (char)payload[offset + i];
        }

        return new ClientGameCharacterDeletePacket(index, privateCode);
    }
}