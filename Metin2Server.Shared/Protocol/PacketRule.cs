namespace Metin2Server.Shared.Protocol;

public record PacketRule(
    PacketSizeKind PacketSizeKind,
    SequenceBehavior SequenceBehavior,
    SessionPhase AllowedSessionPhases,
    int? ExactPayloadSize,
    int? MinPayloadSize);