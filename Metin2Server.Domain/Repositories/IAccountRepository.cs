using Metin2Server.Domain.Entities;

namespace Metin2Server.Domain.Repositories;

public interface IAccountRepository : IAuditingEntityRepository<Account>
{
    Task<Account?> FindOneByLoginAsync(string login, CancellationToken cancellationToken);
}