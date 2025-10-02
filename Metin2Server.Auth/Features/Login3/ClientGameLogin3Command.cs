using MediatR;

namespace Metin2Server.Auth.Features.Login3;

public record ClientGameLogin3Command(string Username, string Password, uint[] AdwClientKey) : IRequest;