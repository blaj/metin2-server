using Metin2Server.Shared.Protocol;

namespace Metin2Server.Network;

public class PacketTransport
{
    private readonly PacketRuleRegistryIn _inRules;
    private readonly PacketRuleRegistryOut _outRules;
    private readonly PacketSequencer _sequencer;

    public PacketTransport(
        PacketRuleRegistryIn inRules,
        PacketRuleRegistryOut outRules,
        PacketSequencer sequencer)
    {
        _inRules = inRules;
        _outRules = outRules;
        _sequencer = sequencer;
    }

    public IAsyncEnumerable<Packet> ReadAsync(Session session, CancellationToken cancellationToken)
        => PacketFramer.ReadPacketsAsync(session.Socket, _inRules, session, _sequencer, cancellationToken);

    public async Task SendAsync(
        Session session,
        GameClientHeader header,
        ReadOnlyMemory<byte> payload,
        CancellationToken cancellationToken)
    {
        if (!_outRules.Validate(header, payload.Length))
        {
            throw new InvalidOperationException($"Invalid outbound payload length for header: {header}");
        }

        await PacketFramer.WriteFrameAsync(session.Socket, _outRules, header, payload, session, _sequencer,
            cancellationToken);
    }
}