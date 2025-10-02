using Metin2Server.Shared.Protocol;

namespace Metin2Server.Network;

public class PacketRuleRegistryOut
{
    private readonly Dictionary<GameClientHeader, PacketRule> _rules = new();

    public void RegisterRule(GameClientHeader header, PacketRule rule) => _rules[header] = rule;

    public bool TryGetRule(GameClientHeader header, out PacketRule rule) => _rules.TryGetValue(header, out rule!);

    public bool Validate(GameClientHeader header, int payloadLength)
    {
        if (!TryGetRule(header, out var r))
        {
            return false;
        }
        
        return r.PacketSizeKind switch
        {
            PacketSizeKind.TotalSize =>
                (!r.ExactPayloadSize.HasValue || payloadLength == r.ExactPayloadSize.Value) &&
                (!r.MinPayloadSize.HasValue || payloadLength >= r.MinPayloadSize.Value),
            
            PacketSizeKind.NoneFixed => r.ExactPayloadSize.HasValue && payloadLength == r.ExactPayloadSize.Value,
            
            _ => false
        };
    }
}