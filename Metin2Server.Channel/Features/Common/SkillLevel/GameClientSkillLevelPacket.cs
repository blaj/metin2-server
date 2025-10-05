using Metin2Server.Shared.Common;

namespace Metin2Server.Channel.Features.Common.SkillLevel;

public record GameClientSkillLevelPacket(int[] Skills)
{
    public static int Size() => sizeof(int) * Constants.SkillMaxNum;
};