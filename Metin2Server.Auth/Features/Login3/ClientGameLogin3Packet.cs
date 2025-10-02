namespace Metin2Server.Auth.Features.Login3;

public record ClientGameLogin3Packet(char[] Username, char[] Password, uint[] AdwClientKey);