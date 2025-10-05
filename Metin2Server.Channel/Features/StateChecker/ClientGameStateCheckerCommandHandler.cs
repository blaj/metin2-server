using Google.Protobuf.WellKnownTypes;
using MediatR;
using Metin2Server.Channel.Features.Common.ChannelStatus;
using Metin2Server.Shared.Application;
using Metin2Server.Shared.DbContracts;
using Metin2Server.Shared.Extensions;
using Metin2Server.Shared.Utils;
using Microsoft.Extensions.Configuration;

namespace Metin2Server.Channel.Features.StateChecker;

public class ClientGameStateCheckerCommandHandler : IRequestHandler<ClientGameStateCheckerCommand>
{
    private readonly ISessionAccessor _sessionAccessor;
    private readonly IConfiguration _configuration;
    private readonly ChannelInformationService.ChannelInformationServiceClient _channelInformationServiceClient;

    public ClientGameStateCheckerCommandHandler(
        ISessionAccessor sessionAccessor,
        IConfiguration configuration,
        ChannelInformationService.ChannelInformationServiceClient channelInformationServiceClient)
    {
        _sessionAccessor = sessionAccessor;
        _configuration = configuration;
        _channelInformationServiceClient = channelInformationServiceClient;
    }

    public async Task<Unit> Handle(ClientGameStateCheckerCommand command, CancellationToken cancellationToken)
    {
        var currentPacketOutCollector = _sessionAccessor.CurrentPacketOutCollector;

        var getChannelInformationsResponse = await _channelInformationServiceClient.GetChannelInformationsAsync(
            new Empty(),
            cancellationToken: cancellationToken);

        currentPacketOutCollector.Add(
            new GameClientChannelStatusPacket((uint)getChannelInformationsResponse.Entries.Count,
                getChannelInformationsResponse.Entries
                    .Select(channelInformation =>
                        (GrpcUtils.ToUShortChecked(channelInformation.Port), channelInformation.Status.ToEntity()))
                    .ToArray()));

        return Unit.Value;
    }
}