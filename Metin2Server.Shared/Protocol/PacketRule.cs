namespace Metin2Server.Network;

public record PacketRule(
    ClientGameHeader ClientGameHeader,
    PacketSizeKind PacketSizeKind,
    SequenceBehavior SequenceBehavior,
    SessionPhase AllowedSessionPhases,
    int? ExactPayloadSize,
    int? MinPayloadSize);