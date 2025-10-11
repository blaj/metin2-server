using Metin2Server.Database.Domain.Entities;

namespace Metin2Server.Database.Domain.Repositories;

public interface ICharacterRepository : IAuditingEntityRepository<Character>
{
    Task<bool> ExistsByIndexAndAccountIdAsync(byte index, long accountId, CancellationToken cancellationToken);

    Task<bool> ExistsByName(string name, CancellationToken cancellationToken);

    Task<IEnumerable<Character>> FindAllByAccountIdOrderByIndexAsync(long accountId, CancellationToken cancellationToken);

    Task<Character?> FindOneByAccountIdAndIndex(long accountId, byte index, CancellationToken cancellationToken);
}