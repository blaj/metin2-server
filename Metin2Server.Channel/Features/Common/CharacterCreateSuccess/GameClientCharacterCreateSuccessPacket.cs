namespace Metin2Server.Channel.Features.Common.CharacterCreateSuccess;

public record GameClientCharacterCreateSuccessPacket(
    byte Index,
    Common.SimpleCharacter.Dto.SimpleCharacter SimpleCharacter)
{
    public static int Size() =>
        sizeof(byte) +
        Common.SimpleCharacter.Dto.SimpleCharacter.Size();
}