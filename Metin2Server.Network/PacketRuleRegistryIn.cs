using Metin2Server.Shared.Protocol;

namespace Metin2Server.Network;

public class PacketRuleRegistryIn
{
    private readonly Dictionary<ClientGameHeader, PacketRule> _rules = new();

    public void RegisterRule(ClientGameHeader header, PacketRule rule) => _rules[header] = rule;

    public bool TryGetRule(ClientGameHeader clientGameHeader, out PacketRule rule) =>
        _rules.TryGetValue(clientGameHeader, out rule!);

    public bool Validate(ClientGameHeader clientGameHeader, int payloadLength)
    {
        if (!TryGetRule(clientGameHeader, out var rule))
        {
            return false;
        }

        return rule.PacketSizeKind switch
        {
            PacketSizeKind.TotalSize => 
                (!rule.ExactPayloadSize.HasValue || payloadLength == rule.ExactPayloadSize.Value) &&
                (!rule.MinPayloadSize.HasValue   || payloadLength >= rule.MinPayloadSize.Value),

            PacketSizeKind.NoneFixed => rule.ExactPayloadSize.HasValue && payloadLength == rule.ExactPayloadSize.Value,

            _ => false
        };
    }
}