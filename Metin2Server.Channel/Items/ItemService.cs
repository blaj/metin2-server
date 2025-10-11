using Metin2Server.Shared.DbContracts;
using Metin2Server.Shared.Dto;

namespace Metin2Server.Channel.Items;

public class ItemService
{
    private readonly CharacterItemService.CharacterItemServiceClient _characterItemServiceClient;

    public ItemService(CharacterItemService.CharacterItemServiceClient characterItemServiceClient)
    {
        _characterItemServiceClient = characterItemServiceClient;
    }
    
    public void AssignItemsToCharacterAsync(GameCharacter gameCharacter, CancellationToken cancellationToken)
    {
        
    }
}