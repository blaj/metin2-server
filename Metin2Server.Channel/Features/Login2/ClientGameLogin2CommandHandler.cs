using MediatR;
using Metin2Server.Channel.Features.Common.Empire;
using Metin2Server.Channel.Features.Common.LoginSuccessNewslot;
using Metin2Server.Channel.Features.Common.SimpleCharacter.Factory;
using Metin2Server.Shared.Application;
using Metin2Server.Shared.Application.LoginFailure;
using Metin2Server.Shared.Application.Phase;
using Metin2Server.Shared.Common;
using Metin2Server.Shared.DbContracts;
using Metin2Server.Shared.DbContracts.Common;
using Metin2Server.Shared.Protocol;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Channel.Features.Login2;

public class ClientGameLogin2CommandHandler : IRequestHandler<ClientGameLogin2Command>
{
    private readonly AccountService.AccountServiceClient _accountServiceClient;
    private readonly LoginKeyService.LoginKeyServiceClient _loginKeyServiceClient;
    private readonly CharacterService.CharacterServiceClient _characterServiceClient;
    private readonly ISessionAccessor _sessionAccessor;

    public ClientGameLogin2CommandHandler(
        AccountService.AccountServiceClient accountServiceClient,
        LoginKeyService.LoginKeyServiceClient loginKeyServiceClient,
        CharacterService.CharacterServiceClient characterServiceClient,
        ISessionAccessor sessionAccessor)
    {
        _accountServiceClient = accountServiceClient;
        _loginKeyServiceClient = loginKeyServiceClient;
        _characterServiceClient = characterServiceClient;
        _sessionAccessor = sessionAccessor;
    }

    public async Task<Unit> Handle(ClientGameLogin2Command command, CancellationToken cancellationToken)
    {
        var currentSession = _sessionAccessor.Current;
        var currentPacketOutCollector = _sessionAccessor.CurrentPacketOutCollector;

        var getAccountByLoginGrpcResponse =
            await _accountServiceClient.GetAccountByLoginAsync(
                new GetAccountByLoginGrpcRequest { Login = command.Username },
                cancellationToken: cancellationToken);

        if (getAccountByLoginGrpcResponse.ResultCase == GetAccountByLoginGrpcResponse.ResultOneofCase.NotFound)
        {
            currentPacketOutCollector.Add(new GameClientLoginFailurePacket("NOID"));

            return Unit.Value;
        }

        // var loginKeyConsumeResponse = await _loginKeyServiceClient.ConsumeAsync(new ConsumeLoginKeyRequest
        //     {
        //         Key = command.LoginKey,
        //         ExpectedAccountId = getAccountByLoginResponse.Account.Id
        //     },
        //     cancellationToken: cancellationToken);
        //
        // if (!loginKeyConsumeResponse.Ok)
        // {
        //     currentPacketOutCollector.Add(new GameClientLoginFailurePacket("NOID"));
        //
        //     return Unit.Value;
        // }

        currentSession.LoginKey = command.LoginKey;
        currentSession.AccountId = getAccountByLoginGrpcResponse.Account.Id;

        if (getAccountByLoginGrpcResponse.Account.Empire == EmpireGrpc.Unknown)
        {
            var empire = (Shared.Enums.Empire)Random.Shared.Next(1, 3);

            var changeAccountEmpireGrpcResponse = await _accountServiceClient.ChangeAccountEmpireAsync(
                new ChangeAccountEmpireGrpcRequest
                {
                    Id = getAccountByLoginGrpcResponse.Account.Id,
                    Empire = (EmpireGrpc)empire
                },
                cancellationToken: cancellationToken);

            if (!changeAccountEmpireGrpcResponse.Ok)
            {
                currentPacketOutCollector.Add(new GameClientLoginFailurePacket("NOID"));

                return Unit.Value;
            }

            currentPacketOutCollector.Add(new GameClientEmpirePacket(empire));

            return Unit.Value;
        }

        currentSession.Phase = SessionPhase.SelectCharacter;
        currentPacketOutCollector.Add(new GameClientPhasePacket(PhaseWireMapper.Map(currentSession.Phase)));

        var getCharactersByAccountIdGrpcResponse =
            await _characterServiceClient.GetCharactersByAccountIdAsync(
                new GetCharactersByAccountIdGrpcRequest { AccountId = getAccountByLoginGrpcResponse.Account.Id },
                cancellationToken: cancellationToken);
        
        var characters = new[]
        {
            SimpleCharacterFactory.Empty(),
            SimpleCharacterFactory.Empty(),
            SimpleCharacterFactory.Empty(),
            SimpleCharacterFactory.Empty()
        };

        foreach (var character in getCharactersByAccountIdGrpcResponse.Characters)
        {
            if (character.Index < 0 || character.Index >= characters.Length)
            {
                continue;
            }
            
            characters[character.Index] = SimpleCharacterFactory.FromProto(character);
        }

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