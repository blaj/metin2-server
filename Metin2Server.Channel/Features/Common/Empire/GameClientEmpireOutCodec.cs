using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.Common.Empire;

public class GameClientEmpireOutCodec : IPacketOutCodec<GameClientEmpirePacket>
{
    public ReadOnlyMemory<byte> Write(GameClientEmpirePacket gameClientEmpirePacket)
    {
        return new[] { (byte)gameClientEmpirePacket.Empire };
    }
}