using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.Common.CharacterDeleteFailure;

public class GameClientCharacterDeleteFailureOutCodec : IPacketOutCodec<GameClientCharacterDeleteFailurePacket>
{
    public ReadOnlyMemory<byte> Write(GameClientCharacterDeleteFailurePacket packet)
    {
        var buffer = new byte[1];
        buffer[0] = packet.Type;
        return buffer;
    }
}