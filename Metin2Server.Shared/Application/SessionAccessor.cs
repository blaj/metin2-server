namespace Metin2Server.Shared.Application;

public class SessionAccessor : ISessionAccessor
{
    public static IDisposable Scope(ISessionContext session) => new ScopeToken(session);

    private static readonly AsyncLocal<ISessionContext?> Cur = new();
    private static readonly AsyncLocal<PacketOutCollector?> CurPacketOutCollector = new();

    public ISessionContext Current => Cur.Value!;
    public PacketOutCollector CurrentPacketOutCollector => CurPacketOutCollector.Value ??= new PacketOutCollector();

    private class ScopeToken : IDisposable
    {
        private readonly ISessionContext? _prev;
        private readonly PacketOutCollector? _prevPacketOutCollector;

        public ScopeToken(ISessionContext sessionContext)
        {
            _prev = Cur.Value;
            _prevPacketOutCollector = CurPacketOutCollector.Value;

            Cur.Value = sessionContext;
            CurPacketOutCollector.Value = new PacketOutCollector();
        }

        public void Dispose()
        {
            Cur.Value = _prev;
            CurPacketOutCollector.Value = _prevPacketOutCollector;
        }
    }
}