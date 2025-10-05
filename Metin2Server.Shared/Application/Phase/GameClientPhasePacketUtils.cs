using MediatR;
using Metin2Server.Shared.Protocol;

namespace Metin2Server.Shared.Application.Phase;

public static class GameClientPhasePacketUtils
{
    public static Unit AddToPacketCollector(
        ISessionContext sessionContext,
        PacketOutCollector packetOutCollector,
        SessionPhase sessionPhase)
    {
        sessionContext.Phase = sessionPhase;
        packetOutCollector.Add(new GameClientPhasePacket(PhaseWireMapper.Map(sessionContext.Phase)));
        
        return Unit.Value;
    }
}