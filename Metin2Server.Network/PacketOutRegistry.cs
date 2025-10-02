using Metin2Server.Shared.Protocol;
using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Network;

public class PacketOutRegistry
{
    private readonly Dictionary<Type, (byte header, Func<object, ReadOnlyMemory<byte>> write)> _routes = new();

    public void Map<TDto>(GameClientHeader header, IPacketOutCodec<TDto> codec)
    {
        _routes[typeof(TDto)] = ((byte)header, o => codec.Write((TDto)o));
    }

    public bool TrySerialize(object dto, out byte header, out ReadOnlyMemory<byte> payload)
    {
        if (_routes.TryGetValue(dto.GetType(), out var t))
        {
            header = t.header;
            payload = t.write(dto);
            return true;
        }

        header = 0;
        payload = default;
        return false;
    }
}