using Metin2Server.Shared.Protocol;

namespace Metin2Server.Network;

public class PacketRuleRegistryOut
{
    private readonly Dictionary<GameClientHeader, PacketRule> _rules = new();

    public void RegisterRule(GameClientHeader header, PacketRule rule) => _rules[header] = rule;

    public bool TryGetRule(GameClientHeader header, out PacketRule rule) => _rules.TryGetValue(header, out rule!);

    public bool Validate(GameClientHeader header, int payloadLength)
    {
        if (!TryGetRule(header, out var packetRule))
        {
            return false;
        }
        
        return packetRule.PacketSizeKind switch
        {
            PacketSizeKind.TotalSize =>
                (!packetRule.ExactPayloadSize.HasValue || payloadLength == packetRule.ExactPayloadSize.Value) &&
                (!packetRule.MinPayloadSize.HasValue || payloadLength >= packetRule.MinPayloadSize.Value),
            
            PacketSizeKind.NoneFixed => packetRule.ExactPayloadSize.HasValue && payloadLength == packetRule.ExactPayloadSize.Value,
            
            PacketSizeKind.Flexible => true,
            
            _ => false
        };
    }
}