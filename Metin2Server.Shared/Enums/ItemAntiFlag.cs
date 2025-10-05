namespace Metin2Server.Shared.Enums;

[Flags]
public enum ItemAntiFlag : uint
{
    Female = (1 << 0),
    Male = (1 << 1),
    Warrior = (1 << 2),
    Assassin = (1 << 3),
    Sura = (1 << 4),
    Shaman = (1 << 5),
    Get = (1 << 6),
    Drop = (1 << 7),
    Sell = (1 << 8),
    EmpireA = (1 << 9),
    EmpireB = (1 << 10),
    EmpireC = (1 << 11),
    Save = (1 << 12),
    Give = (1 << 13),
    PkDrop = (1 << 14),
    Stack = (1 << 15),
    MyShop = (1 << 16),
    SafeBox = (1 << 17),
}