using Metin2Server.Shared.Common;
using Metin2Server.Shared.Enums;

namespace Metin2Server.Channel.Features.Common.MainCharacter;

public record GameClientMainCharacterPacket(
    uint Vid,
    Race Race,
    char[] Name,
    int X,
    int Y,
    int Z,
    Shared.Enums.Empire Empire,
    byte SkillGroup)
{
    public static int Size() =>
        sizeof(uint) +
        sizeof(Race) +
        (sizeof(byte) * (Constants.CharacterNameMaxLength + 1)) +
        sizeof(int) +
        sizeof(int) +
        sizeof(int) +
        sizeof(Shared.Enums.Empire) +
        sizeof(byte);
}