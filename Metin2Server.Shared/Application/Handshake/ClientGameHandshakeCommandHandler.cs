using MediatR;
using Metin2Server.Shared.Application;
using Metin2Server.Shared.Application.Phase;
using Metin2Server.Shared.Application.TimeSync;
using Metin2Server.Shared.Common;
using Metin2Server.Shared.Protocol;
using Metin2Server.Shared.Utils;
using Microsoft.Extensions.Logging;

namespace Metin2Server.Auth.Features.Handshake;

public class ClientGameHandshakeCommandHandler : IRequestHandler<ClientGameHandshakeCommand, object?>
{
    private const int BiasAcceptMax = 50;

    private readonly ISessionAccessor _sessionAccessor;
    private readonly ILogger<ClientGameHandshakeCommandHandler> _logger;

    public ClientGameHandshakeCommandHandler(
        ISessionAccessor sessionAccessor,
        ILogger<ClientGameHandshakeCommandHandler> logger)
    {
        _sessionAccessor = sessionAccessor;
        _logger = logger;
    }

    public Task<object?> Handle(ClientGameHandshakeCommand command, CancellationToken cancellationToken)
    {
        var currentSession = _sessionAccessor.Current;

        if (currentSession is not { State.HandshakeCrc: not null } ||
            command.Handshake != currentSession.State.HandshakeCrc!.Value)
        {
            _logger.LogError(
                $"[{currentSession.Id}] Invalid handshake: packet={command.Handshake} session={currentSession.State.HandshakeCrc}");

            currentSession.Phase = SessionPhase.Closing;

            return Task.FromResult<object?>(new GameClientPhasePacket(PhaseWireMapper.Map(currentSession.Phase)));
        }


        if (command.Delta < 0)
        {
            _logger.LogError($"[{currentSession.Id}] Delta < 0");

            currentSession.Phase = SessionPhase.Closing;

            return Task.FromResult<object?>(new GameClientPhasePacket(PhaseWireMapper.Map(currentSession.Phase)));
        }

        var currentTime = DateTimeUtils.GetUnixTime();
        var bias = (int)(currentTime - (command.CurrentTime + command.Delta));

        var inHandshakePhase = currentSession.Phase == SessionPhase.Handshake;
        var infiniteRetry = !inHandshakePhase;

        if (bias >= 0 && bias <= BiasAcceptMax)
        {
            _logger.LogInformation(
                $"[{currentSession.Id}] Handshake OK. server={currentTime} client={currentSession.State.ClientTime}");

            if (infiniteRetry)
            {
                _logger.LogInformation($"[{currentSession.Id}] Send time sync");

                return Task.FromResult<object?>(new GameClientTimeSyncPacket());
            }

            currentSession.State.ClientTime = currentTime;
            currentSession.State.IsHandshaking = false;

            currentSession.Phase = SessionPhase.Auth;
            return Task.FromResult<object?>(new GameClientPhasePacket(PhaseWireMapper.Map(currentSession.Phase)));
        }

        var newDelta = (int)(currentTime - command.CurrentTime) / 2;
        if (newDelta < 0)
        {
            _logger.LogWarning($"[{currentSession.Id}] newDelta < 0; fallback to sentTime");
            newDelta = (int)((currentTime - currentSession.State.HandshakeSentTime) / 2);
        }

        _logger.LogInformation(
            $"[{currentSession.Id}] Handshake retry. now={currentTime}, delta={command.Delta}, newDelta={newDelta}, sentTime={currentSession.State.HandshakeSentTime}, clientTime={currentSession.State.ClientTime}");

        if (!infiniteRetry)
        {
            if (++currentSession.State.HandshakeRetryCount > Constants.HandshakeRetryLimit)
            {
                _logger.LogError($"[{currentSession.Id}] Handshake retry limit reached");

                currentSession.Phase = SessionPhase.Closing;

                return Task.FromResult<object?>(new GameClientPhasePacket(PhaseWireMapper.Map(currentSession.Phase)));
            }
        }

        currentSession.State.IsHandshaking = true;
        currentSession.State.HandshakeSentTime = DateTimeUtils.GetUnixTime();

        return Task.FromResult<object?>(
            new GameClientHandshakePacket(
                currentSession.State.HandshakeCrc!.Value,
                currentTime,
                newDelta));
    }
}