using Grpc.Core;
using Metin2Server.Database.Domain.Repositories;
using Metin2Server.Database.Extensions;
using Metin2Server.Shared.DbContracts;

namespace Metin2Server.Database.Grpc;

public class CharacterItemServiceImpl : CharacterItemService.CharacterItemServiceBase
{
    private readonly ICharacterItemRepository _characterItemRepository;

    public CharacterItemServiceImpl(ICharacterItemRepository characterItemRepository)
    {
        _characterItemRepository = characterItemRepository;
    }

    public override async Task<GetCharacterItemsByCharacterIdAndAccountIdGrpcResponse>
        GetCharacterItemsByCharacterIdAndAccountId(
            GetCharacterItemsByCharacterIdAndAccountIdGrpcRequest request,
            ServerCallContext context)
    {
        var characterItems =
            await _characterItemRepository.FindAllByCharacterIdAndAccountIdAsync(
                request.CharacterId,
                request.AccountId,
                context.CancellationToken);

        return new GetCharacterItemsByCharacterIdAndAccountIdGrpcResponse
        {
            CharacterItems = { characterItems.Select(characterItem => characterItem.ToProto()) }
        };
    }
}