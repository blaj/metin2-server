using Metin2Server.Shared.Protocol;
using Microsoft.Extensions.Logging;

namespace Metin2Server.Network;

public class PacketTransport
{
    private readonly PacketRuleRegistryIn _inRules;
    private readonly PacketRuleRegistryOut _outRules;
    private readonly PacketSequencer _sequencer;
    private readonly ILogger<PacketFramer> _packetFramerLogger;

    public PacketTransport(
        PacketRuleRegistryIn inRules,
        PacketRuleRegistryOut outRules,
        PacketSequencer sequencer,
        ILogger<PacketFramer> packetFramerLogger)
    {
        _inRules = inRules;
        _outRules = outRules;
        _sequencer = sequencer;
        _packetFramerLogger = packetFramerLogger;
    }

    public IAsyncEnumerable<Packet> ReadAsync(Session session, CancellationToken cancellationToken)
        => PacketFramer.ReadPacketsAsync(
            session.Socket,
            _inRules,
            session,
            _sequencer,
            _packetFramerLogger,
            cancellationToken);

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

        await PacketFramer.WriteFrameAsync(
            session.Socket,
            _outRules,
            header,
            payload,
            session,
            _sequencer,
            _packetFramerLogger,
            cancellationToken);
    }
}