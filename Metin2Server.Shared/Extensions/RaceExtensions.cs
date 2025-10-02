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

    public static DbContracts.Common.Race ToProto(this Race race) => race switch
    {
        Race.WarriorM => DbContracts.Common.Race.WarriorM,
        Race.WarriorW => DbContracts.Common.Race.WarriorW,
        Race.AssassinM => DbContracts.Common.Race.AssassinM,
        Race.AssassinW => DbContracts.Common.Race.AssassinW,
        Race.SuraM => DbContracts.Common.Race.SuraM,
        Race.SuraW => DbContracts.Common.Race.SuraW,
        Race.ShamanM => DbContracts.Common.Race.ShamanM,
        Race.ShamanW => DbContracts.Common.Race.ShamanW,
        _ => DbContracts.Common.Race.WarriorM
    };

    public static Race ToEntity(this DbContracts.Common.Race race) => race switch
    {
        DbContracts.Common.Race.WarriorM => Race.WarriorM,
        DbContracts.Common.Race.WarriorW => Race.WarriorW,
        DbContracts.Common.Race.AssassinM => Race.AssassinM,
        DbContracts.Common.Race.AssassinW => Race.AssassinW,
        DbContracts.Common.Race.SuraM => Race.SuraM,
        DbContracts.Common.Race.SuraW => Race.SuraW,
        DbContracts.Common.Race.ShamanM => Race.ShamanM,
        DbContracts.Common.Race.ShamanW => Race.ShamanW,
        _ => Race.WarriorM
    };
}