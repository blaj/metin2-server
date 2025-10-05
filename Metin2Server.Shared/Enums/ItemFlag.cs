namespace Metin2Server.Shared.Enums;

[Flags]
public enum ItemFlag : uint
{
    Refineable = (1 << 0),
    Save = (1 << 1),
    Stackable = (1 << 2),
    CountPer1Gold = (1 << 3),
    SlowQuery = (1 << 4),
    Unused01 = (1 << 5),
    Unique = (1 << 6),
    Makecount = (1 << 7),
    Irremovable = (1 << 8),
    ConfirmWhenUse = (1 << 9),
    QuestUse = (1 << 10),
    QuestUseMultiple = (1 << 11),
    QuestGive = (1 << 12),
    Log = (1 << 13),
    Applicable = (1 << 14),
}