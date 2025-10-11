using Metin2Server.Database.Data;
using Metin2Server.Database.Domain.Entities;
using Metin2Server.Database.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Metin2Server.Database.Repositories;

public class AccountRepository : AuditingEntityRepository<Account>, IAccountRepository
{
    private readonly DbSet<Account> _dbSet;

    public AccountRepository(GameDbContext dbContext) : base(dbContext)
    {
        _dbSet = dbContext.Set<Account>();
    }

    public async Task<Account?> FindOneByLoginAsync(string login, CancellationToken cancellationToken)
    {
        return await _dbSet.FirstOrDefaultAsync(account => account.Login == login, cancellationToken);
    }
}