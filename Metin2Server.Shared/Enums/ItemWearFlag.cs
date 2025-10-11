namespace Metin2Server.Shared.Enums;

[Flags]
public enum ItemWearFlag : uint
{
    Body = (1 << 0),
    Head = (1 << 1),
    Foots = (1 << 2),
    Wrist = (1 << 3),
    Weapon = (1 << 4),
    Neck = (1 << 5),
    Ear = (1 << 6),
    Unique = (1 << 7),
    Shield = (1 << 8),
    Arrow = (1 << 9),
    Hair = (1 << 10),
    Ability = (1 << 11),
    CostumeBody = (1 << 12),
    Belt = (1 << 13),
}