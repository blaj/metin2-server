namespace Metin2Server.Channel.Features.Common.Channel;

public record GameClientChannelPacket(byte Channel)
{
    public static int Size() => sizeof(byte);
}