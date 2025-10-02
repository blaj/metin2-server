using MediatR;
using Metin2Server.Channel.Features.Common.CharacterCreateFailure;
using Metin2Server.Channel.Features.Common.CharacterCreateSuccess;
using Metin2Server.Channel.Features.Common.SimpleCharacter.Factory;
using Metin2Server.Shared.Application;
using Metin2Server.Shared.Common;
using Metin2Server.Shared.DbContracts;
using Metin2Server.Shared.Dto;
using Metin2Server.Shared.Extensions;

namespace Metin2Server.Channel.Features.CharacterCreate;

public class ClientGameCharacterCreateCommandHandler : IRequestHandler<ClientGameCharacterCreateCommand>
{
    private readonly ISessionAccessor _sessionAccessor;
    private readonly BannedWordService.BannedWordServiceClient _bannedWordServiceClient;
    private readonly CharacterService.CharacterServiceClient _characterServiceClient;
    private readonly DbService.DbServiceClient _dbServiceClient;

    public ClientGameCharacterCreateCommandHandler(
        ISessionAccessor sessionAccessor,
        BannedWordService.BannedWordServiceClient bannedWordServiceClient,
        CharacterService.CharacterServiceClient characterServiceClient,
        DbService.DbServiceClient dbServiceClient)
    {
        _sessionAccessor = sessionAccessor;
        _bannedWordServiceClient = bannedWordServiceClient;
        _characterServiceClient = characterServiceClient;
        _dbServiceClient = dbServiceClient;
    }

    public async Task<Unit> Handle(ClientGameCharacterCreateCommand command, CancellationToken cancellationToken)
    {
        var currentSession = _sessionAccessor.Current;
        var currentPacketOutCollector = _sessionAccessor.CurrentPacketOutCollector;

        if (!IsNameValid(command.Name))
        {
            AddCreateFailurePacket(currentPacketOutCollector, 1);

            return Unit.Value;
        }

        var bannedWordExistsResponse =
            await _bannedWordServiceClient.ExistsByWordAsync(
                new ExistsByWordRequest { Word = command.Name },
                cancellationToken: cancellationToken);

        if (bannedWordExistsResponse.Exists)
        {
            AddCreateFailurePacket(currentPacketOutCollector, 1);

            return Unit.Value;
        }

        if (command.Shape > 1)
        {
            AddCreateFailurePacket(currentPacketOutCollector, 1);

            return Unit.Value;
        }

        if (currentSession.AccountId == null)
        {
            AddCreateFailurePacket(currentPacketOutCollector, 0);

            return Unit.Value;
        }

        var beginCharacterCreationResponse = await _characterServiceClient.TryBeginCharacterCreationAsync(
            new TryBeginCharacterCreationRequest { AccountId = currentSession.AccountId.Value },
            cancellationToken: cancellationToken);

        if (!beginCharacterCreationResponse.Allowed)
        {
            AddCreateFailurePacket(currentPacketOutCollector, 0);

            return Unit.Value;
        }

        var characterExistsByIndexAndAccountIdResponse = await _characterServiceClient.ExistsByIndexAndAccountIdAsync(
            new ExistsByIndexAndAccountIdRequest { Index = command.Index, AccountId = currentSession.AccountId.Value },
            cancellationToken: cancellationToken);

        if (characterExistsByIndexAndAccountIdResponse.Exists)
        {
            AddCreateFailurePacket(currentPacketOutCollector, 1);

            return Unit.Value;
        }

        var characterExistsByNameResponse =
            await _characterServiceClient.ExistsByNameAsync(
                new ExistsByNameRequest { Name = command.Name },
                cancellationToken: cancellationToken);

        if (characterExistsByNameResponse.Exists)
        {
            AddCreateFailurePacket(currentPacketOutCollector, 0);

            return Unit.Value;
        }

        var getAccountByIdResponse = await _dbServiceClient.GetAccountByIdAsync(
            new GetAccountByIdRequest { Id = currentSession.AccountId!.Value },
            cancellationToken: cancellationToken);

        if (getAccountByIdResponse.ResultCase == GetAccountByIdResponse.ResultOneofCase.NotFound)
        {
            AddCreateFailurePacket(currentPacketOutCollector, 0);

            return Unit.Value;
        }

        var createCharacterResponse = await _characterServiceClient.CreateCharacterAsync(
            new CreateCharacterRequest
            {
                AccountId = getAccountByIdResponse.Account.Id,
                Name = command.Name,
                Race = command.Race.ToProto(),
                Shape = command.Shape,
                Index = command.Index
            },
            cancellationToken: cancellationToken);

        if (createCharacterResponse.ResultCase == CreateCharacterResponse.ResultOneofCase.NotFound)
        {
            AddCreateFailurePacket(currentPacketOutCollector, 0);

            return Unit.Value;
        }

        currentPacketOutCollector.Add(
            new GameClientCharacterCreateSuccessPacket(
                command.Index,
                SimpleCharacterFactory.FromProto(createCharacterResponse.Character)));

        return Unit.Value;
    }

    private void AddCreateFailurePacket(PacketOutCollector packetOutCollector, byte type)
    {
        packetOutCollector.Add(new GameClientCharacterCreateFailurePacket(type));
    }

    private static bool IsNameValid(string name)
    {
        return name.Length >= 2 && name.All(char.IsLetterOrDigit) &&
               name.Length - 1 <= Constants.CharacterNameMaxLength;
    }
}