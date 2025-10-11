using MediatR;
using Metin2Server.Channel.Features.Common.LoginSuccessNewslot;
using Metin2Server.Channel.Features.Common.SimpleCharacter.Factory;
using Metin2Server.Shared.Application;
using Metin2Server.Shared.Application.Phase;
using Metin2Server.Shared.Common;
using Metin2Server.Shared.DbContracts;
using Metin2Server.Shared.DbContracts.Common;
using Metin2Server.Shared.Protocol;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Channel.Features.Empire;

public class ClientGameEmpireCommandHandler : IRequestHandler<ClientGameEmpireCommand>
{
    private readonly AccountService.AccountServiceClient _accountServiceClient;
    private readonly ISessionAccessor _sessionAccessor;

    public ClientGameEmpireCommandHandler(
        AccountService.AccountServiceClient accountServiceClient,
        ISessionAccessor sessionAccessor)
    {
        _accountServiceClient = accountServiceClient;
        _sessionAccessor = sessionAccessor;
    }

    public async Task<Unit> Handle(ClientGameEmpireCommand command, CancellationToken cancellationToken)
    {
        var currentSession = _sessionAccessor.Current;
        var currentPacketOutCollector = _sessionAccessor.CurrentPacketOutCollector;

        if (currentSession.AccountId == null)
        {
            return GameClientPhasePacketUtils.AddToPacketCollector(
                currentSession, 
                currentPacketOutCollector,
                SessionPhase.Closing);
        }

        var getAccountByIdGrpcResponse = await _accountServiceClient.GetAccountByIdAsync(
            new GetAccountByIdGrpcRequest { Id = currentSession.AccountId.Value },
            cancellationToken: cancellationToken);

        if (getAccountByIdGrpcResponse.ResultCase == GetAccountByIdGrpcResponse.ResultOneofCase.NotFound)
        {
            return GameClientPhasePacketUtils.AddToPacketCollector(
                currentSession, 
                currentPacketOutCollector,
                SessionPhase.Closing);
        }

        var changeAccountEmpireGrpcResponse = await _accountServiceClient.ChangeAccountEmpireAsync(
            new ChangeAccountEmpireGrpcRequest
                { Id = getAccountByIdGrpcResponse.Account.Id, Empire = (EmpireGrpc)command.Empire },
            cancellationToken: cancellationToken);

        if (!changeAccountEmpireGrpcResponse.Ok)
        {
            return GameClientPhasePacketUtils.AddToPacketCollector(
                currentSession, 
                currentPacketOutCollector,
                SessionPhase.Closing);
        }

        var characters = new[]
        {
            SimpleCharacterFactory.Empty(),
            SimpleCharacterFactory.Empty(),
            SimpleCharacterFactory.Empty(),
            SimpleCharacterFactory.Empty()
        };

        var guildIds = new uint[]
        {
            0, 0, 0, 0
        };

        var guildNames = new[]
        {
            StringUtils.EmptyChars(Constants.GuildNameMaxLength + 1),
            StringUtils.EmptyChars(Constants.GuildNameMaxLength + 1),
            StringUtils.EmptyChars(Constants.GuildNameMaxLength + 1),
            StringUtils.EmptyChars(Constants.GuildNameMaxLength + 1)
        };

        currentPacketOutCollector.Add(new GameClientLoginSuccessNewslotPacket(
            characters,
            guildIds,
            guildNames,
            (uint)currentSession.Id,
            (uint)Random.Shared.Next(1, int.MaxValue)));

        return Unit.Value;
    }
}