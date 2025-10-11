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
    private readonly AccountService.AccountServiceClient _accountServiceClient;
    private readonly CharacterService.CharacterServiceClient _characterServiceClient;

    public ClientGameCharacterDeleteCommandHandler(
        ISessionAccessor sessionAccessor,
        AccountService.AccountServiceClient accountServiceClient,
        CharacterService.CharacterServiceClient characterServiceClient)
    {
        _sessionAccessor = sessionAccessor;
        _accountServiceClient = accountServiceClient;
        _characterServiceClient = characterServiceClient;
    }

    public async Task<Unit> Handle(ClientGameCharacterDeleteCommand command, CancellationToken cancellationToken)
    {
        var currentSession = _sessionAccessor.Current;
        var currentPacketOutCollector = _sessionAccessor.CurrentPacketOutCollector;

        var getPrivateCodeByIdGrpcResponse = await _accountServiceClient.GetPrivateCodeByIdAsync(
            new GetPrivateCodeByIdGrpcRequest
            {
                Id = currentSession.AccountId!.Value
            },
            cancellationToken: cancellationToken);

        if (getPrivateCodeByIdGrpcResponse.ResultCase == GetPrivateCodeByIdGrpcResponse.ResultOneofCase.NotFound)
        {
            return AddDeleteFailurePacket(currentPacketOutCollector);
        }

        var getCharacterLevelByAccountIdAndIndexGrpcResponse = await _characterServiceClient.GetCharacterLevelByAccountIdAndIndexAsync(
            new GetCharacterLevelByAccountIdAndIndexGrpcRequest
            {
                AccountId = currentSession.AccountId!.Value,
                Index = command.Index
            },
            cancellationToken: cancellationToken);

        if (getCharacterLevelByAccountIdAndIndexGrpcResponse.ResultCase ==
            GetCharacterLevelByAccountIdAndIndexGrpcResponse.ResultOneofCase.NotFound)
        {
            return AddDeleteFailurePacket(currentPacketOutCollector);
        }

        if (getCharacterLevelByAccountIdAndIndexGrpcResponse.Level > Constants.CharacterDeleteLevelLimit)
        {
            return AddDeleteFailurePacket(currentPacketOutCollector);
        }

        if (getPrivateCodeByIdGrpcResponse.PrivateCode != command.PrivateCode)
        {
            return AddDeleteFailurePacket(currentPacketOutCollector);
        }

        var deleteCharacterByAccountIdAndIndexGrpcResponse = await _characterServiceClient.DeleteCharacterByAccountIdAndIndexAsync(
            new DeleteCharacterByAccountIdAndIndexGrpcRequest
            {
                AccountId = currentSession.AccountId.Value,
                Index = command.Index
            },
            cancellationToken: cancellationToken);

        if (!deleteCharacterByAccountIdAndIndexGrpcResponse.Success)
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