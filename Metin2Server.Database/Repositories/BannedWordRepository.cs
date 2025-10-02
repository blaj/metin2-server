using Metin2Server.Database.Data;
using Metin2Server.Domain.Entities;
using Metin2Server.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Metin2Server.Database.Repositories;

public class BannedWordRepository : ArchiveEntityRepository<BannedWord>, IBannedWordRepository
{
    private readonly DbSet<BannedWord> _dbSet;

    public BannedWordRepository(GameDbContext dbContext) : base(dbContext)
    {
        _dbSet = dbContext.Set<BannedWord>();
    }

    public async Task<bool> ExistsByWordAsync(string word, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(bannedWord => bannedWord.Word.ToUpper() == word.ToUpper(), cancellationToken);
    }
}