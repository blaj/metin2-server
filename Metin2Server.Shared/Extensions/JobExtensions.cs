using Metin2Server.Shared.DbContracts.Common;
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

    public static JobGrpc ToProto(this Job job) => job switch
    {
        Job.Warrior => JobGrpc.Warrior,
        Job.Assassin => JobGrpc.Assassin,
        Job.Sura => JobGrpc.Sura,
        Job.Shaman => JobGrpc.Shaman,
        _ => JobGrpc.Warrior
    };

    public static Job ToEntity(this JobGrpc value) => value switch
    {
        JobGrpc.Warrior => Job.Warrior,
        JobGrpc.Assassin => Job.Assassin,
        JobGrpc.Sura => Job.Sura,
        JobGrpc.Shaman => Job.Shaman,
        _ => Job.Warrior
    };
}