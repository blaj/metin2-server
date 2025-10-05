using System.Buffers.Binary;
using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.Common.ChannelStatus;

public class GameClientChannelStatusOutCodec : IPacketOutCodec<GameClientChannelStatusPacket>
{
    public ReadOnlyMemory<byte> Write(GameClientChannelStatusPacket gameClientChannelStatusPacket)
    {
        var length = sizeof(uint) +
                     (gameClientChannelStatusPacket.Size * (sizeof(ushort) + sizeof(Shared.Enums.ChannelStatus)));

        var buffer = new byte[length];
        var span = buffer.AsSpan();
        var offset = 0;

        BinaryPrimitives.WriteUInt32LittleEndian(
            buffer.AsSpan(offset, sizeof(uint)),
            gameClientChannelStatusPacket.Size);
        offset += sizeof(uint);

        foreach (var (port, status) in gameClientChannelStatusPacket.Channels)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(buffer.AsSpan(offset, sizeof(ushort)), port);
            offset += sizeof(ushort);

            span[offset++] = (byte)status;
        }

        return buffer;
    }
}