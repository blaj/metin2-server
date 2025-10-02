namespace Metin2Server.Channel.Features.Login2;

public record ClientGameLogin2Packet(char[] Username, uint LoginKey, uint[] AdwClientKey);