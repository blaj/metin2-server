using Metin2Server.Domain.Entities;

namespace Metin2Server.Domain.Repositories;

public interface IAuditingEntityRepository<TEntity> : IRepository<TEntity> where TEntity : AuditingEntity
{
}