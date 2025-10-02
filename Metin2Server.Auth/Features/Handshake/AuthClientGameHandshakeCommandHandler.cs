using Metin2Server.Shared.Application;
using Metin2Server.Shared.Application.Handshake;
using Metin2Server.Shared.Protocol;
using Microsoft.Extensions.Logging;

namespace Metin2Server.Auth.Features.Handshake;

public class AuthClientGameHandshakeCommandHandler : ClientGameHandshakeCommandHandler
{
    public AuthClientGameHandshakeCommandHandler(
        ISessionAccessor sessionAccessor,
        ILogger<ClientGameHandshakeCommandHandler> logger) : base(sessionAccessor, logger)
    {
    }

    public override SessionPhase SuccessHandshakePhase() => SessionPhase.Auth;
}