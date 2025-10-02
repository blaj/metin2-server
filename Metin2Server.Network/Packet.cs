namespace Metin2Server.Network;

public readonly record struct Packet(byte Header, byte[]? Payload);