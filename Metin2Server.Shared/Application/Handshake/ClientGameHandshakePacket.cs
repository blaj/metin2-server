namespace Metin2Server.Auth.Features.Handshake;

public record ClientGameHandshakePacket(uint Handshake, uint CurrentTime, int Delta);