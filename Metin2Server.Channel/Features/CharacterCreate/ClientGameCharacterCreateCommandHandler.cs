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
    private readonly AccountService.AccountServiceClient _accountServiceClient;

    public ClientGameCharacterCreateCommandHandler(
        ISessionAccessor sessionAccessor,
        BannedWordService.BannedWordServiceClient bannedWordServiceClient,
        CharacterService.CharacterServiceClient characterServiceClient,
        AccountService.AccountServiceClient accountServiceClient)
    {
        _sessionAccessor = sessionAccessor;
        _bannedWordServiceClient = bannedWordServiceClient;
        _characterServiceClient = characterServiceClient;
        _accountServiceClient = accountServiceClient;
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

        var existsByWordGrpcResponse =
            await _bannedWordServiceClient.ExistsByWordAsync(
                new ExistsByWordGrpcRequest { Word = command.Name },
                cancellationToken: cancellationToken);

        if (existsByWordGrpcResponse.Exists)
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

        var tryBeginCharacterCreationGrpcResponse = await _characterServiceClient.TryBeginCharacterCreationAsync(
            new TryBeginCharacterCreationGrpcRequest { AccountId = currentSession.AccountId.Value },
            cancellationToken: cancellationToken);

        if (!tryBeginCharacterCreationGrpcResponse.Allowed)
        {
            AddCreateFailurePacket(currentPacketOutCollector, 0);

            return Unit.Value;
        }

        var existsByIndexAndAccountIdGrpcResponse = await _characterServiceClient.ExistsByIndexAndAccountIdAsync(
            new ExistsByIndexAndAccountIdGrpcRequest { Index = command.Index, AccountId = currentSession.AccountId.Value },
            cancellationToken: cancellationToken);

        if (existsByIndexAndAccountIdGrpcResponse.Exists)
        {
            AddCreateFailurePacket(currentPacketOutCollector, 1);

            return Unit.Value;
        }

        var existsByNameGrpcResponse =
            await _characterServiceClient.ExistsByNameAsync(
                new ExistsByNameGrpcRequest { Name = command.Name },
                cancellationToken: cancellationToken);

        if (existsByNameGrpcResponse.Exists)
        {
            AddCreateFailurePacket(currentPacketOutCollector, 0);

            return Unit.Value;
        }

        var getAccountByIdGrpcResponse = await _accountServiceClient.GetAccountByIdAsync(
            new GetAccountByIdGrpcRequest { Id = currentSession.AccountId!.Value },
            cancellationToken: cancellationToken);

        if (getAccountByIdGrpcResponse.ResultCase == GetAccountByIdGrpcResponse.ResultOneofCase.NotFound)
        {
            AddCreateFailurePacket(currentPacketOutCollector, 0);

            return Unit.Value;
        }

        var createCharacterGrpcResponse = await _characterServiceClient.CreateCharacterAsync(
            new CreateCharacterGrpcRequest
            {
                AccountId = getAccountByIdGrpcResponse.Account.Id,
                Name = command.Name,
                Race = command.Race.ToProto(),
                Shape = command.Shape,
                Index = command.Index
            },
            cancellationToken: cancellationToken);

        if (createCharacterGrpcResponse.ResultCase == CreateCharacterGrpcResponse.ResultOneofCase.NotFound)
        {
            AddCreateFailurePacket(currentPacketOutCollector, 0);

            return Unit.Value;
        }

        currentPacketOutCollector.Add(
            new GameClientCharacterCreateSuccessPacket(
                command.Index,
                SimpleCharacterFactory.FromProto(createCharacterGrpcResponse.Character)));

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