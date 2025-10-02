using Metin2Server.Shared.Application.Phase;
using Metin2Server.Shared.Protocol;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Shared.Application.Handshake;

public class AuthHandshakeStartup : ISessionStartup
{
    private readonly GameClientHandshakeOutCodec _gameClientHandshakeOutCodec;
    private readonly GameClientPhaseOutCodec _gameClientPhaseOutCodec;

    public AuthHandshakeStartup(
        GameClientHandshakeOutCodec gameClientHandshakeOutCodec,
        GameClientPhaseOutCodec gameClientPhaseOutCodec)
    {
        _gameClientHandshakeOutCodec = gameClientHandshakeOutCodec;
        _gameClientPhaseOutCodec = gameClientPhaseOutCodec;
    }

    public async Task RunAsync(
        ISessionContext session,
        Func<GameClientHeader, ReadOnlyMemory<byte>, CancellationToken, Task> send,
        CancellationToken cancellationToken)
    {
        var crcHandshake = Shared.Encryption.Handshake.CreateHandshake();
        var currentTime = DateTimeUtils.GetUnixTime();

        session.State.HandshakeCrc = crcHandshake;
        session.State.IsHandshaking = true;
        session.State.HandshakeSentTime = currentTime;

        session.Phase = SessionPhase.Handshake;

        var payload = _gameClientPhaseOutCodec.Write(new GameClientPhasePacket(PhaseWireMapper.Map(session.Phase)));

        await send(GameClientHeader.Phase, payload, cancellationToken);
        
        payload =
            _gameClientHandshakeOutCodec.Write(
                new GameClientHandshakePacket(crcHandshake, currentTime, 0));

        await send(GameClientHeader.Handshake, payload, cancellationToken);
    }
}