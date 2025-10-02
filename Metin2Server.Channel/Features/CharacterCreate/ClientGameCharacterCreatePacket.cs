using Metin2Server.Shared.Common;

namespace Metin2Server.Channel.Features.CharacterCreate;

public record ClientGameCharacterCreatePacket(
    byte Index,
    char[] Name,
    ushort Race,
    byte Shape,
    byte Con,
    byte Int,
    byte Str,
    byte Dex)
{
    public static int Size() =>
        sizeof(byte) +
        sizeof(byte) * (Constants.CharacterNameMaxLength + 1) +
        sizeof(ushort) +
        sizeof(byte) +
        sizeof(byte) +
        sizeof(byte) +
        sizeof(byte) +
        sizeof(byte);
}