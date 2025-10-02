using MediatR;

namespace Metin2Server.Channel.Features.Login2;

public record ClientGameLogin2Command(string Username, uint LoginKey, uint[] AdwClientKey) : IRequest;