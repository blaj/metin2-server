using Metin2Server.Shared.DbContracts.Common;
using Metin2Server.Shared.Enums;

namespace Metin2Server.Shared.Extensions;

public static class RaceExtensions
{
    public static Job ToJob(this Race race) => race switch
    {
        Race.WarriorM => Job.Warrior,
        Race.WarriorW => Job.Warrior,
        Race.AssassinM => Job.Assassin,
        Race.AssassinW => Job.Assassin,
        Race.SuraM => Job.Sura,
        Race.SuraW => Job.Sura,
        Race.ShamanM => Job.Shaman,
        Race.ShamanW => Job.Shaman,
        _ => throw new ArgumentOutOfRangeException(nameof(race), race, null)
    };

    public static RaceGrpc ToProto(this Race race) => race switch
    {
        Race.WarriorM => RaceGrpc.WarriorM,
        Race.WarriorW => RaceGrpc.WarriorW,
        Race.AssassinM => RaceGrpc.AssassinM,
        Race.AssassinW => RaceGrpc.AssassinW,
        Race.SuraM => RaceGrpc.SuraM,
        Race.SuraW => RaceGrpc.SuraW,
        Race.ShamanM => RaceGrpc.ShamanM,
        Race.ShamanW => RaceGrpc.ShamanW,
        _ => RaceGrpc.WarriorM
    };

    public static Race ToEntity(this RaceGrpc race) => race switch
    {
        RaceGrpc.WarriorM => Race.WarriorM,
        RaceGrpc.WarriorW => Race.WarriorW,
        RaceGrpc.AssassinM => Race.AssassinM,
        RaceGrpc.AssassinW => Race.AssassinW,
        RaceGrpc.SuraM => Race.SuraM,
        RaceGrpc.SuraW => Race.SuraW,
        RaceGrpc.ShamanM => Race.ShamanM,
        RaceGrpc.ShamanW => Race.ShamanW,
        _ => Race.WarriorM
    };
}