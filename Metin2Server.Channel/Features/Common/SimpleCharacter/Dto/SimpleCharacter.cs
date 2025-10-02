namespace Metin2Server.Channel.Features.Common.Dto;

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
    byte SkillGroup);