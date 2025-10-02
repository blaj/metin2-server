using Metin2Server.Shared.Application;
using Metin2Server.Shared.Protocol;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Auth.Features.Handshake;

public class AuthHandshakeStartup : ISessionStartup
{
    private readonly GameClientHandshakeOutCodec _gameClientHandshakeOutCodec;

    public AuthHandshakeStartup(GameClientHandshakeOutCodec gameClientHandshakeOutCodec)
    {
        _gameClientHandshakeOutCodec = gameClientHandshakeOutCodec;
    }

    public async Task RunAsync(
        ISessionContext session,
        Func<GameClientHeader, ReadOnlyMemory<byte>, CancellationToken, Task> send,
        CancellationToken ct)
    {
        var crcHandshake = Shared.Encryption.Handshake.CreateHandshake();
        var currentTime = DateTimeUtils.GetUnixTime();

        session.State.HandshakeCrc = crcHandshake;
        session.State.IsHandshaking = true;
        session.State.HandshakeSentTime = currentTime;

        session.Phase = SessionPhase.Handshake;

        var payload =
            _gameClientHandshakeOutCodec.Write(
                new GameClientHandshakePacket(crcHandshake, currentTime, 0));

        await send(GameClientHeader.Handshake, payload, ct);
    }
}