using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.Common.CreateCharacterFailure;

public class GameClientCreateCharacterFailureOutCodec : IPacketOutCodec<GameClientCreateCharacterFailurePacket>
{
    public ReadOnlyMemory<byte> Write(GameClientCreateCharacterFailurePacket gameClientCreateCharacterFailurePacket)
    {
        return new[] { gameClientCreateCharacterFailurePacket.Type };
    }
}