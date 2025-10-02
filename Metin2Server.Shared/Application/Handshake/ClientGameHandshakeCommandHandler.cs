using MediatR;
using Metin2Server.Shared.Application.Phase;
using Metin2Server.Shared.Application.TimeSync;
using Metin2Server.Shared.Common;
using Metin2Server.Shared.Protocol;
using Metin2Server.Shared.Utils;
using Microsoft.Extensions.Logging;

namespace Metin2Server.Shared.Application.Handshake;

public abstract class ClientGameHandshakeCommandHandler : IRequestHandler<ClientGameHandshakeCommand>
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

    public abstract SessionPhase SuccessHandshakePhase();

    public Task<Unit> Handle(ClientGameHandshakeCommand command, CancellationToken cancellationToken)
    {
        var currentSession = _sessionAccessor.Current;
        var currentPacketOutCollector = _sessionAccessor.CurrentPacketOutCollector;

        if (currentSession is not { State.HandshakeCrc: not null } ||
            command.Handshake != currentSession.State.HandshakeCrc!.Value)
        {
            _logger.LogError(
                $"[{currentSession.Id}] Invalid handshake: packet={command.Handshake} session={currentSession.State.HandshakeCrc}");

            currentSession.Phase = SessionPhase.Closing;
            currentPacketOutCollector.Add(new GameClientPhasePacket(PhaseWireMapper.Map(currentSession.Phase)));

            return Task.FromResult(Unit.Value);
        }


        if (command.Delta < 0)
        {
            _logger.LogError($"[{currentSession.Id}] Delta < 0");

            currentSession.Phase = SessionPhase.Closing;
            currentPacketOutCollector.Add(new GameClientPhasePacket(PhaseWireMapper.Map(currentSession.Phase)));

            return Task.FromResult(Unit.Value);
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

                currentPacketOutCollector.Add(new GameClientTimeSyncPacket());

                return Task.FromResult(Unit.Value);
            }

            currentSession.State.ClientTime = currentTime;
            currentSession.State.IsHandshaking = false;

            currentSession.Phase = SuccessHandshakePhase();
            currentPacketOutCollector.Add(new GameClientPhasePacket(PhaseWireMapper.Map(currentSession.Phase)));

            return Task.FromResult(Unit.Value);
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
                currentPacketOutCollector.Add(new GameClientPhasePacket(PhaseWireMapper.Map(currentSession.Phase)));

                return Task.FromResult(Unit.Value);
            }
        }

        currentSession.State.IsHandshaking = true;
        currentSession.State.HandshakeSentTime = DateTimeUtils.GetUnixTime();

        currentPacketOutCollector.Add(
            new GameClientHandshakePacket(
                currentSession.State.HandshakeCrc!.Value,
                currentTime,
                newDelta));

        return Task.FromResult(Unit.Value);
    }
}