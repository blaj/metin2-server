namespace Metin2Server.Channel.Features.Common.CharacterDeleteFailure;

public record GameClientCharacterDeleteFailurePacket(byte Type)
{
    public static int Size() => sizeof(byte);
}