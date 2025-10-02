using Metin2Server.Database.Data;
using Metin2Server.Domain.Entities;
using Metin2Server.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Metin2Server.Database.Repositories;

public abstract class AuditingEntityRepository<TEntity> : Repository<TEntity>, IAuditingEntityRepository<TEntity>
    where TEntity : AuditingEntity
{
    private readonly DbSet<TEntity> _dbSet;

    public AuditingEntityRepository(GameDbContext dbContext) : base(dbContext)
    {
        _dbSet = dbContext.Set<TEntity>();
    }

    public new async Task DeleteAsync(long id, CancellationToken cancellationToken)
    {
        await _dbSet
            .Where(entity => entity.Id == id)
            .ExecuteUpdateAsync(
                entity => entity
                    .SetProperty(property => property.Deleted, true)
                    .SetProperty(property => property.DeletedAt, DateTime.UtcNow),
                cancellationToken);
    }
}