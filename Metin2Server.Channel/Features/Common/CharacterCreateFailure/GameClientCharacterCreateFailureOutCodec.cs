using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.Common.CharacterCreateFailure;

public class GameClientCharacterCreateFailureOutCodec : IPacketOutCodec<GameClientCharacterCreateFailurePacket>
{
    public ReadOnlyMemory<byte> Write(GameClientCharacterCreateFailurePacket gameClientCharacterCreateFailurePacket)
    {
        return new[] { gameClientCharacterCreateFailurePacket.Type };
    }
}