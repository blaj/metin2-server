using System.Buffers.Binary;
using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.Common.ChannelStatus;

public class GameClientChannelStatusOutCodec : IPacketOutCodec<GameClientChannelStatusPacket>
{
    public ReadOnlyMemory<byte> Write(GameClientChannelStatusPacket gameClientChannelStatusPacket)
    {
        var buffer = new byte[3];
        
        BinaryPrimitives.WriteUInt16LittleEndian(buffer.AsSpan(0, 2), gameClientChannelStatusPacket.Port);
        buffer[2] = (byte)gameClientChannelStatusPacket.Status;
        
        return buffer;
    }
}