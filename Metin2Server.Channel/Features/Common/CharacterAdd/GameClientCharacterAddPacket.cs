using Metin2Server.Shared.Enums;

namespace Metin2Server.Channel.Features.Common.CharacterAdd;

public record GameClientCharacterAddPacket(
    uint Vid,
    float Angle,
    int X,
    int Y,
    int Z,
    CharacterType CharacterType,
    Race Race,
    byte MovingSpeed,
    byte AttackSpeed,
    byte StateFlag,
    uint[] AffectFlags)
{
    public static int Size() => 
        sizeof(uint) +
        sizeof(float) + 
        sizeof(int) + 
        sizeof(int) + 
        sizeof(int) +
        sizeof(CharacterType) +
        sizeof(Race) +
        sizeof(byte) +
        sizeof(byte) +
        sizeof(byte) +
        (sizeof(uint) * 2);
}