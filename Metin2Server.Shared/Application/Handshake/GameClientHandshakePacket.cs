namespace Metin2Server.Auth.Features.Handshake;

public record GameClientHandshakePacket(uint Handshake, uint CurrentTime, int Delta);