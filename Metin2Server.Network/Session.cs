using System.Net.Sockets;
using Metin2Server.Shared.Application;
using Metin2Server.Shared.Encryption;
using Metin2Server.Shared.Protocol;
using Microsoft.Extensions.Logging;

namespace Metin2Server.Network;

public class Session : ISessionContext, IAsyncDisposable
{
    public long Id { get; }
    public Socket Socket { get; }
    public string Endpoint { get; }
    public DateTime LastActivityUtc { get; private set; } = DateTime.UtcNow;
    public SessionState State { get; } = new();
    public uint? LoginKey { get; set; }
    public long? AccountId { get; set; }
    public int SequenceIndex { get; set; } = 0;
    
    private int _phase;

    private readonly SemaphoreSlim _sendLock = new(1, 1);

    public Session(long id, Socket socket)
    {
        Id = id;
        Socket = socket;
        Endpoint = socket.RemoteEndPoint?.ToString() ?? "?";
        Socket.NoDelay = true;
        Phase = SessionPhase.Handshake;
    }

    public SessionPhase Phase
    {
        get => (SessionPhase)Volatile.Read(ref _phase);
        set => Interlocked.Exchange(ref _phase, (int)value);
    }
    
    public void Touch() => LastActivityUtc = DateTime.UtcNow;

    public async ValueTask DisposeAsync()
    {
        try
        {
            Socket.Shutdown(SocketShutdown.Both);
        }
        catch
        {
            // ignored
        }

        try
        {
            Socket.Close();
        }
        catch
        {
            // ignored
        }

        _sendLock.Dispose();
        await Task.CompletedTask;
    }
}