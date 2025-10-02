using Metin2Server.Shared.Common;

namespace Metin2Server.Channel.Features.CharacterDelete;

public record ClientGameCharacterDeletePacket(byte Index, char[] PrivateCode)
{
    public static int Size() =>
        sizeof(byte) +
        sizeof(byte) * Constants.PrivateCodeLength;
}