namespace Metin2Server.Shared.Application.Handshake;

public record ClientGameHandshakePacket(uint Handshake, uint CurrentTime, int Delta);