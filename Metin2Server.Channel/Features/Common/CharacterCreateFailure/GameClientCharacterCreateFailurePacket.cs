namespace Metin2Server.Channel.Features.Common.CharacterCreateFailure;

public record GameClientCharacterCreateFailurePacket(byte Type)
{
    public static int Size() => sizeof(byte);
}