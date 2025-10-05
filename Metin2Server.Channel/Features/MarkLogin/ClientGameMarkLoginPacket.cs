namespace Metin2Server.Channel.Features.MarkLogin;

public record ClientGameMarkLoginPacket(uint Handle, uint RandomKey)
{
    public static int Size() => sizeof(uint) + sizeof(uint);
}