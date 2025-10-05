using MediatR;

namespace Metin2Server.Channel.Features.ClientVersion2;

public class ClientGameClientVersion2CommandHandler : IRequestHandler<ClientGameClientVersion2Command>
{
    public Task<Unit> Handle(ClientGameClientVersion2Command request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Unit.Value);
    }
}