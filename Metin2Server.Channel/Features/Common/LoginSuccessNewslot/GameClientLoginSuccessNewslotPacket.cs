namespace Metin2Server.Channel.Features.Common.LoginSuccessNewslot;

public record GameClientLoginSuccessNewslotPacket(
    SimpleCharacter.Dto.SimpleCharacter[] Characters,
    uint[] GuildIds,
    char[][] GuildNames,
    uint Handle,
    uint RandomKey);