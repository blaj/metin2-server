using MediatR;

namespace Metin2Server.Shared.Application.Handshake;

public record ClientGameHandshakeCommand(uint Handshake, uint CurrentTime, int Delta) : IRequest;