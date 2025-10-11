using Metin2Server.Database.Domain.Entities;

namespace Metin2Server.Database.Domain.Repositories;

public interface ICharacterItemRepository : IAuditingEntityRepository<CharacterItem>
{
    Task<IEnumerable<CharacterItem>> FindAllByCharacterIdAndAccountIdAsync(
        long characterId,
        long accountId,
        CancellationToken cancellationToken);
}