using Metin2Server.Domain.Entities;

namespace Metin2Server.Domain.Repositories;

public interface IBannedWordRepository : IArchiveEntityRepository<BannedWord>
{
    Task<bool> ExistsByWordAsync(string word, CancellationToken cancellationToken);
}