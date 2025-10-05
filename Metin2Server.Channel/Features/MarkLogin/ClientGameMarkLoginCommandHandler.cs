using MediatR;

namespace Metin2Server.Channel.Features.MarkLogin;

public class ClientGameMarkLoginCommandHandler : IRequestHandler<ClientGameMarkLoginCommand>
{
    public Task<Unit> Handle(ClientGameMarkLoginCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Unit.Value);
    }
}