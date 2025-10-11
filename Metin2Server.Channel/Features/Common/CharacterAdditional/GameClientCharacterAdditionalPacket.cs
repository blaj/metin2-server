using Metin2Server.Shared.Common;
using Metin2Server.Shared.Enums;

namespace Metin2Server.Channel.Features.Common.CharacterAdditional;

public record GameClientCharacterAdditionalPacket(
    uint Vid,
    char[] Name,
    ushort[] Parts,
    Shared.Enums.Empire Empire,
    uint GuildId,
    uint Level,
    short Alignment,
    CharacterPkMode PkMode,
    uint MountVnum)
{
    public static int Size() =>
        sizeof(uint) +
        (sizeof(byte) * (Constants.CharacterNameMaxLength + 1)) +
        (sizeof(ushort) * (int)CharacterEquipmentPart.Num) +
        sizeof(Shared.Enums.Empire) +
        sizeof(uint) +
        sizeof(uint) +
        sizeof(short) +
        sizeof(byte) +
        sizeof(uint);
}