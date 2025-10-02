using Metin2Server.Shared.Dto;
using Metin2Server.Shared.Enums;

namespace Metin2Server.Shared.Extensions;

public static class JobExtensions
{
    public static JobInitialPoint GetInitialPoint(this Job job) => job switch
    {
        Job.Warrior => new JobInitialPoint(
            6,
            4,
            3,
            3,
            600,
            200,
            40,
            20,
            36,
            44,
            18,
            22,
            800,
            5,
            1,
            3
        ),

        Job.Assassin => new JobInitialPoint(
            4,
            3,
            6,
            3,
            650,
            200,
            40,
            20,
            36,
            44,
            18,
            22,
            800,
            5,
            1,
            3
        ),

        Job.Sura => new JobInitialPoint(
            5,
            3,
            3,
            5,
            650,
            200,
            40,
            20,
            36,
            44,
            18,
            22,
            800,
            5,
            1,
            3
        ),

        Job.Shaman => new JobInitialPoint(
            3,
            4,
            3,
            6,
            700,
            200,
            40,
            20,
            36,
            44,
            18,
            22,
            800,
            5,
            1,
            3
        ),

        _ => throw new ArgumentOutOfRangeException(nameof(job), job, null)
    };

    public static DbContracts.Common.Job ToProto(this Job job) => job switch
    {
        Job.Warrior => DbContracts.Common.Job.Warrior,
        Job.Assassin => DbContracts.Common.Job.Assassin,
        Job.Sura => DbContracts.Common.Job.Sura,
        Job.Shaman => DbContracts.Common.Job.Shaman,
        _ => DbContracts.Common.Job.Warrior
    };

    public static Job ToEntity(this DbContracts.Common.Job value) => value switch
    {
        DbContracts.Common.Job.Warrior => Job.Warrior,
        DbContracts.Common.Job.Assassin => Job.Assassin,
        DbContracts.Common.Job.Sura => Job.Sura,
        DbContracts.Common.Job.Shaman => Job.Shaman,
        _ => Job.Warrior
    };
}