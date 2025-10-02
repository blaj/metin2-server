using Metin2Server.Domain.Entities;

namespace Metin2Server.Domain.Repositories;

public interface ILoginKeyRepository
{
    Task<LoginKey> IssueAsync(
        long accountId,
        long? sessionId,
        TimeSpan ttl,
        string? expectedIp,
        CancellationToken cancellationToken);

    Task<(bool ok, LoginKey? entry)> TryConsumeAsync(
        uint key,
        long? expectedAccountId,
        string? clientIp,
        CancellationToken cancellationToken);
}