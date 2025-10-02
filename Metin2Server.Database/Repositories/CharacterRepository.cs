using Metin2Server.Database.Data;
using Metin2Server.Domain.Entities;
using Metin2Server.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Metin2Server.Database.Repositories;

public class CharacterRepository : AuditingEntityRepository<Character>, ICharacterRepository
{
    private readonly DbSet<Character> _dbSet;

    public CharacterRepository(GameDbContext dbContext) : base(dbContext)
    {
        _dbSet = dbContext.Set<Character>();
    }

    public async Task<bool> ExistsByIndexAndAccountIdAsync(
        byte index,
        long accountId,
        CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(
            character => character.Index == index && character.Account.Id == accountId,
            cancellationToken);
    }

    public async Task<bool> ExistsByName(string name, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(character => character.Name.ToUpper() == name.ToUpper(), cancellationToken);
    }

    public async Task<IEnumerable<Character>> FindAllByAccountIdOrderByIndexAsync(
        long accountId,
        CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(character => character.Account.Id == accountId)
            .OrderByDescending(character => character.Index)
            .ToListAsync(cancellationToken);
    }

    public async Task<Character?> FindOneByAccountIdAndIndex(
        long accountId,
        byte index,
        CancellationToken cancellationToken)
    {
        return await _dbSet
            .Where(character => character.Account.Id == accountId && character.Index == index)
            .FirstOrDefaultAsync(cancellationToken);
    }
}