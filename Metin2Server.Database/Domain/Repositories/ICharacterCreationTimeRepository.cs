namespace Metin2Server.Database.Domain.Repositories;

public interface ICharacterCreationTimeRepository
{
    Task<bool> TryConsumeAsync(long accountId, TimeSpan window, CancellationToken cancellationToken);
}