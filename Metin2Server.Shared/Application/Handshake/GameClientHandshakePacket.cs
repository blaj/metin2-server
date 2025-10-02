namespace Metin2Server.Shared.Application.Handshake;

public record GameClientHandshakePacket(uint Handshake, uint CurrentTime, int Delta);