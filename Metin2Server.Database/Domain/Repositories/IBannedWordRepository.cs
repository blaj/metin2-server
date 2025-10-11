using Metin2Server.Database.Domain.Entities;

namespace Metin2Server.Database.Domain.Repositories;

public interface IBannedWordRepository : IArchiveEntityRepository<BannedWord>
{
    Task<bool> ExistsByWordAsync(string word, CancellationToken cancellationToken);
}