using MediatR;
using Metin2Server.Channel.Features.Common.Empire;
using Metin2Server.Channel.Features.Common.LoginSuccessNewslot;
using Metin2Server.Channel.Features.Common.SimpleCharacter.Factory;
using Metin2Server.Shared.Application;
using Metin2Server.Shared.Application.LoginFailure;
using Metin2Server.Shared.Application.Phase;
using Metin2Server.Shared.Common;
using Metin2Server.Shared.DbContracts;
using Metin2Server.Shared.Protocol;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Channel.Features.Login2;

public class ClientGameLogin2CommandHandler : IRequestHandler<ClientGameLogin2Command>
{
    private readonly DbService.DbServiceClient _dbServiceClient;
    private readonly LoginKeyService.LoginKeyServiceClient _loginKeyServiceClient;
    private readonly CharacterService.CharacterServiceClient _characterServiceClient;
    private readonly ISessionAccessor _sessionAccessor;

    public ClientGameLogin2CommandHandler(
        DbService.DbServiceClient dbServiceClient,
        LoginKeyService.LoginKeyServiceClient loginKeyServiceClient,
        CharacterService.CharacterServiceClient characterServiceClient,
        ISessionAccessor sessionAccessor)
    {
        _dbServiceClient = dbServiceClient;
        _loginKeyServiceClient = loginKeyServiceClient;
        _characterServiceClient = characterServiceClient;
        _sessionAccessor = sessionAccessor;
    }

    public async Task<Unit> Handle(ClientGameLogin2Command command, CancellationToken cancellationToken)
    {
        var currentSession = _sessionAccessor.Current;
        var currentPacketOutCollector = _sessionAccessor.CurrentPacketOutCollector;

        var getAccountByLoginResponse =
            await _dbServiceClient.GetAccountByLoginAsync(
                new GetAccountByLoginRequest() { Login = command.Username },
                cancellationToken: cancellationToken);

        if (getAccountByLoginResponse.ResultCase == GetAccountByLoginResponse.ResultOneofCase.NotFound)
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
        currentSession.AccountId = getAccountByLoginResponse.Account.Id;

        if (getAccountByLoginResponse.Account.Empire == Shared.DbContracts.Common.Empire.Unknown)
        {
            var empire = (Shared.Enums.Empire)Random.Shared.Next(1, 3);

            var changeAccountEmpireResponse = await _dbServiceClient.ChangeAccountEmpireAsync(
                new ChangeAccountEmpireRequest
                {
                    Id = getAccountByLoginResponse.Account.Id,
                    Empire = (Shared.DbContracts.Common.Empire)empire
                },
                cancellationToken: cancellationToken);

            if (!changeAccountEmpireResponse.Ok)
            {
                currentPacketOutCollector.Add(new GameClientLoginFailurePacket("NOID"));

                return Unit.Value;
            }

            currentPacketOutCollector.Add(new GameClientEmpirePacket(empire));

            return Unit.Value;
        }

        currentSession.Phase = SessionPhase.SelectCharacter;
        currentPacketOutCollector.Add(new GameClientPhasePacket(PhaseWireMapper.Map(currentSession.Phase)));

        var getCharactersByAccountIdResponse =
            await _characterServiceClient.GetCharactersByAccountIdAsync(
                new GetCharactersByAccountIdRequest { AccountId = getAccountByLoginResponse.Account.Id },
                cancellationToken: cancellationToken);
        
        var characters = new[]
        {
            SimpleCharacterFactory.Empty(),
            SimpleCharacterFactory.Empty(),
            SimpleCharacterFactory.Empty(),
            SimpleCharacterFactory.Empty()
        };

        foreach (var character in getCharactersByAccountIdResponse.Characters)
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