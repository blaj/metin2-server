using Metin2Server.Database.Data;
using Metin2Server.Domain.Entities;
using Metin2Server.Domain.Repositories;

namespace Metin2Server.Database.Repositories;

public abstract class ArchiveEntityRepository<TEntity> : Repository<TEntity>, IArchiveEntityRepository<TEntity>
    where TEntity : ArchiveEntity
{
    protected ArchiveEntityRepository(GameDbContext dbContext) : base(dbContext)
    {
    }
}