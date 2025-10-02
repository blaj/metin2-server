namespace Metin2Server.Shared.Protocol.Codecs;

public interface IPacketInCodec<T>
{
    T Read(ReadOnlySpan<byte> payload);
}