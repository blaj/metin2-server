namespace Metin2Server.Channel.Features.Common.Time;

public record GameClientTimePacket(uint Time)
{
    public static int Size() => sizeof(uint);
}