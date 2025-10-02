using MediatR;
using Metin2Server.Shared.Protocol;
using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Network;

public class PacketInRegistry
{
    private readonly Dictionary<ClientGameHeader, Func<ReadOnlyMemory<byte>, IRequest>> _routes = new();

    public void Map<TDto>(ClientGameHeader header, IPacketInCodec<TDto> codec, Func<TDto, IRequest> toRequest)
    {
        _routes[header] = p => toRequest(codec.Read(p.Span));
    }

    public IRequest? ToRequest(ClientGameHeader header, ReadOnlyMemory<byte> payload)
        => _routes.TryGetValue(header, out var f) ? f(payload) : null;
}