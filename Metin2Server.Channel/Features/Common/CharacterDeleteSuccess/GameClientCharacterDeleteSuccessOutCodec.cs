using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.Common.CharacterDeleteSuccess;

public class GameClientCharacterDeleteSuccessOutCodec : IPacketOutCodec<GameClientCharacterDeleteSuccessPacket>
{
    public ReadOnlyMemory<byte> Write(GameClientCharacterDeleteSuccessPacket packet)
    {
        var buffer = new byte[1];
        buffer[0] = packet.Index;
        return buffer;
    }
}