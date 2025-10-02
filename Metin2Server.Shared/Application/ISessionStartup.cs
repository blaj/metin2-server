using Metin2Server.Shared.Protocol;

namespace Metin2Server.Shared.Application;

public interface ISessionStartup
{
    Task RunAsync(
        ISessionContext session,
        Func<GameClientHeader, ReadOnlyMemory<byte>, CancellationToken, Task> send,
        CancellationToken cancellationToken);
}