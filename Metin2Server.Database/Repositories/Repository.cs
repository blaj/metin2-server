using Metin2Server.Database.Data;
using Metin2Server.Database.Domain.Entities;
using Metin2Server.Database.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Metin2Server.Database.Repositories;

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : IdEntity
{
    private readonly DbSet<TEntity> _dbSet;

    public Repository(GameDbContext dbContext)
    {
        _dbSet = dbContext.Set<TEntity>();
    }

    public async Task<TEntity?> FindOneByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync(id, cancellationToken);
    }

    public async Task<bool> ExistsByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> FindAllAsync(CancellationToken cancellationToken)
    {
        return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        _dbSet.Update(entity);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var entity = await _dbSet.FindAsync(id, cancellationToken);

        if (entity == null)
        {
            return;
        }

        _dbSet.Remove(entity);
    }
}