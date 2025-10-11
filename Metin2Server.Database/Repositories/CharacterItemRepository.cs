using Metin2Server.Database.Data;
using Metin2Server.Database.Domain.Entities;
using Metin2Server.Database.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Metin2Server.Database.Repositories;

public class CharacterItemRepository : AuditingEntityRepository<CharacterItem>, ICharacterItemRepository
{
    private readonly DbSet<CharacterItem> _dbSet;

    public CharacterItemRepository(GameDbContext dbContext) : base(dbContext)
    {
        _dbSet = dbContext.Set<CharacterItem>();
    }

    public async Task<IEnumerable<CharacterItem>> FindAllByCharacterIdAndAccountIdAsync(
        long characterId,
        long accountId,
        CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(characterItem =>
                characterItem.Character.Id == characterId && characterItem.Character.Account.Id == accountId)
            .OrderBy(characterItem => characterItem.Position)
            .ToListAsync(cancellationToken);
    }
}