using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.Common.CharacterDeleteFailure;

public class GameClientCharacterDeleteFailurePacketOutCodec : IPacketOutCodec<GameClientCharacterDeleteFailurePacket>
{
    public ReadOnlyMemory<byte> Write(GameClientCharacterDeleteFailurePacket packet)
    {
        throw new NotImplementedException();
    }
}