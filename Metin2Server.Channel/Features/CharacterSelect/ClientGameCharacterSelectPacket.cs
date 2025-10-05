namespace Metin2Server.Channel.Features.CharacterSelect;

public record ClientGameCharacterSelectPacket(byte Index)
{
    public static int Size() => sizeof(byte);
}