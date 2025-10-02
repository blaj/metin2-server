namespace Metin2Server.Shared.Protocol.Codecs;

public interface IPacketOutCodec<T>
{
    ReadOnlyMemory<byte> Write(T packet);
}