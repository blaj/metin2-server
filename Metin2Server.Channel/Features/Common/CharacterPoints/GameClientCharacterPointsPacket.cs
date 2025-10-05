using Metin2Server.Shared.Common;

namespace Metin2Server.Channel.Features.Common.CharacterPoints;

public record GameClientCharacterPointsPacket(int[] Points)
{
    public static int Size() => sizeof(int) * Constants.PointMaxNum;
}