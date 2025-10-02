using Metin2Server.Shared.Common;

namespace Metin2Server.Channel.Features.Common.SimpleCharacter.Dto;

public record SimpleCharacter(
    uint Id,
    char[] Name,
    byte Job,
    byte Level,
    uint PlayMinutes,
    byte St,
    byte Ht,
    byte Dx,
    byte Iq,
    ushort MainPart,
    byte ChangeName,
    ushort HairPart,
    byte[] Dummy,
    uint X,
    uint Y,
    uint Addr,
    ushort Port,
    byte SkillGroup)
{
    public static int Size() =>
        sizeof(uint) +
        (sizeof(byte) * (Constants.CharacterNameMaxLength + 1)) +
        sizeof(byte) +
        sizeof(byte) +
        sizeof(uint) + 
        sizeof(byte) +
        sizeof(byte) +
        sizeof(byte) +
        sizeof(byte) +
        sizeof(ushort) +
        sizeof(byte) +
        sizeof(ushort) +
        (sizeof(byte) * Constants.DummyLength) +
        sizeof(uint) +
        sizeof(uint) +
        sizeof(uint) + 
        sizeof(ushort) +
        sizeof(byte);
}