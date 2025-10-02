namespace Metin2Server.Channel.Features.Common.CharacterDeleteSuccess;

public record GameClientCharacterDeleteSuccessPacket(byte Index)
{
    public static int Size() => sizeof(byte);
}