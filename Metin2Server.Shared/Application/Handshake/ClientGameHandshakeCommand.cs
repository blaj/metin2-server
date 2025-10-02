using MediatR;

namespace Metin2Server.Auth.Features.Handshake;

public record ClientGameHandshakeCommand(uint Handshake, uint CurrentTime, int Delta) : IRequest<object?>;