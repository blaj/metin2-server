using Metin2Server.Database.Domain.Entities;

namespace Metin2Server.Database.Domain.Repositories;

public interface IAccountRepository : IAuditingEntityRepository<Account>
{
    Task<Account?> FindOneByLoginAsync(string login, CancellationToken cancellationToken);
}