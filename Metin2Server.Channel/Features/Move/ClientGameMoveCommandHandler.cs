using MediatR;

namespace Metin2Server.Channel.Features.Move;

public class ClientGameMoveCommandHandler : IRequestHandler<ClientGameMoveCommand>
{
    public async Task<Unit> Handle(ClientGameMoveCommand request, CancellationToken cancellationToken)
    {
        return Unit.Value;
    }
}