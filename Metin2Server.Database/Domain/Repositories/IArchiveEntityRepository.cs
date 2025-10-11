using Metin2Server.Database.Domain.Entities;

namespace Metin2Server.Database.Domain.Repositories;

public interface IArchiveEntityRepository<TEntity> : IRepository<TEntity> where TEntity : ArchiveEntity
{
}