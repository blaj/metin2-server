namespace Metin2Server.Network;

[Flags]
public enum SequenceBehavior
{
    None            = 0,
    ExpectInbound   = 1 << 0,
    PrependOutbound = 1 << 1
}