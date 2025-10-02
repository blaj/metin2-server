using MediatR;
using Metin2Server.Channel.Features.Common.CharacterDeleteFailure;
using Metin2Server.Channel.Features.Common.CharacterDeleteSuccess;
using Metin2Server.Shared.Application;
using Metin2Server.Shared.Common;
using Metin2Server.Shared.DbContracts;

namespace Metin2Server.Channel.Features.CharacterDelete;

public class ClientGameCharacterDeleteCommandHandler : IRequestHandler<ClientGameCharacterDeleteCommand>
{
    private readonly ISessionAccessor _sessionAccessor;
    private readonly DbService.DbServiceClient _dbServiceClient;
    private readonly CharacterService.CharacterServiceClient _characterServiceClient;

    public ClientGameCharacterDeleteCommandHandler(
        ISessionAccessor sessionAccessor,
        DbService.DbServiceClient dbServiceClient,
        CharacterService.CharacterServiceClient characterServiceClient)
    {
        _sessionAccessor = sessionAccessor;
        _dbServiceClient = dbServiceClient;
        _characterServiceClient = characterServiceClient;
    }

    public async Task<Unit> Handle(ClientGameCharacterDeleteCommand command, CancellationToken cancellationToken)
    {
        var currentSession = _sessionAccessor.Current;
        var currentPacketOutCollector = _sessionAccessor.CurrentPacketOutCollector;

        var getPrivateCodeResponse = await _dbServiceClient.GetPrivateCodeByIdAsync(
            new GetPrivateCodeByIdRequest
            {
                Id = currentSession.AccountId!.Value
            },
            cancellationToken: cancellationToken);

        if (getPrivateCodeResponse.ResultCase == GetPrivateCodeByIdResponse.ResultOneofCase.NotFound)
        {
            return AddDeleteFailurePacket(currentPacketOutCollector);
        }

        var getCharacterLevelResponse = await _characterServiceClient.GetCharacterLevelByAccountIdAndIndexAsync(
            new GetCharacterLevelByAccountIdAndIndexRequest
            {
                AccountId = currentSession.AccountId!.Value,
                Index = command.Index
            },
            cancellationToken: cancellationToken);

        if (getCharacterLevelResponse.ResultCase ==
            GetCharacterLevelByAccountIdAndIndexResponse.ResultOneofCase.NotFound)
        {
            return AddDeleteFailurePacket(currentPacketOutCollector);
        }

        if (getCharacterLevelResponse.Level > Constants.CharacterDeleteLevelLimit)
        {
            return AddDeleteFailurePacket(currentPacketOutCollector);
        }

        if (getPrivateCodeResponse.PrivateCode != command.PrivateCode)
        {
            return AddDeleteFailurePacket(currentPacketOutCollector);
        }

        var deleteCharacterResponse = await _characterServiceClient.DeleteCharacterByAccountIdAndIndexAsync(
            new DeleteCharacterByAccountIdAndIndexRequest
            {
                AccountId = currentSession.AccountId.Value,
                Index = command.Index
            },
            cancellationToken: cancellationToken);

        if (!deleteCharacterResponse.Success)
        {
            return AddDeleteFailurePacket(currentPacketOutCollector);
        }
        
        currentPacketOutCollector.Add(new GameClientCharacterDeleteSuccessPacket(command.Index));
        
        return Unit.Value;
    }

    private Unit AddDeleteFailurePacket(PacketOutCollector packetOutCollector)
    {
        packetOutCollector.Add(new GameClientCharacterDeleteFailurePacket(1));
        return Unit.Value;
    }
}