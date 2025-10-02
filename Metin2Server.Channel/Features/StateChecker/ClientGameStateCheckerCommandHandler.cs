using MediatR;
using Metin2Server.Channel.Features.Common.ChannelStatus;
using Metin2Server.Shared.Application;
using Microsoft.Extensions.Configuration;

namespace Metin2Server.Channel.Features.StateChecker;

public class ClientGameStateCheckerCommandHandler : IRequestHandler<ClientGameStateCheckerCommand>
{
    private readonly ISessionAccessor _sessionAccessor;
    private readonly IConfiguration _configuration;

    public ClientGameStateCheckerCommandHandler(
        ISessionAccessor sessionAccessor,
        IConfiguration configuration)
    {
        _sessionAccessor = sessionAccessor;
        _configuration = configuration;
    }

    public Task<Unit> Handle(ClientGameStateCheckerCommand command, CancellationToken cancellationToken)
    {
        var currentPacketOutCollector = _sessionAccessor.CurrentPacketOutCollector;

        currentPacketOutCollector.Add(
            new GameClientChannelStatusPacket(Convert.ToUInt16(_configuration["Port"]), ChannelStatus.Online));

        return Task.FromResult(Unit.Value);
    }
}