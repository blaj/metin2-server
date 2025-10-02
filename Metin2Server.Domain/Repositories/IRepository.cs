using Metin2Server.Domain.Entities;

namespace Metin2Server.Domain.Repositories;

public interface IRepository<TEntity> where TEntity : IdEntity
{
    Task<IEnumerable<TEntity>> FindAllAsync(CancellationToken cancellationToken);

    Task<TEntity?> FindOneByIdAsync(long id, CancellationToken cancellationToken);
    
    Task<bool> ExistsByIdAsync(long id, CancellationToken cancellationToken);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken);

    void Update(TEntity entity);

    Task DeleteAsync(long id, CancellationToken cancellationToken);
}