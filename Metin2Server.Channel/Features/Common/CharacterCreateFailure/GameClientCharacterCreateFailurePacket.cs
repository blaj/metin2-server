namespace Metin2Server.Channel.Features.Common.CreateCharacterFailure;

public record GameClientCreateCharacterFailurePacket(byte Type)
{
    public static int Size() => sizeof(byte);
}