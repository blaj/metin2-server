using Metin2Server.Database.Domain.Entities;

namespace Metin2Server.Database.Domain.Repositories;

public interface IAuditingEntityRepository<TEntity> : IRepository<TEntity> where TEntity : AuditingEntity
{
}