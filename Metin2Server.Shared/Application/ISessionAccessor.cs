namespace Metin2Server.Shared.Application;

public interface ISessionAccessor
{
    ISessionContext Current { get; }
    PacketOutCollector CurrentPacketOutCollector { get; }
}