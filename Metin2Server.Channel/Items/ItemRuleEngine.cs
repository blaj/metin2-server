using Metin2Server.Shared.Dto;
using Metin2Server.Shared.Enums;

namespace Metin2Server.Channel.Items;

public class ItemRuleEngine
{
    // public bool CanEquipToSlot(GameItem item, EquipSlot slot)
    // {
    //     var w = item.Def.Wear;
    //
    //     return slot switch
    //     {
    //         EquipSlot.Weapon => w.HasFlag(ItemWearFlag.Weapon),
    //         EquipSlot.Head => w.HasFlag(ItemWearFlag.Head),
    //         EquipSlot.Body => w.HasFlag(ItemWearFlag.Body),
    //         EquipSlot.Wrist => w.HasFlag(ItemWearFlag.Wrist),
    //         EquipSlot.Foots => w.HasFlag(ItemWearFlag.Foots),
    //         EquipSlot.Neck => w.HasFlag(ItemWearFlag.Neck),
    //         EquipSlot.Ear => w.HasFlag(ItemWearFlag.Ear),
    //         EquipSlot.Shield => w.HasFlag(ItemWearFlag.Shield),
    //         EquipSlot.Arrow => w.HasFlag(ItemWearFlag.Arrow),
    //         EquipSlot.Belt => w.HasFlag(ItemWearFlag.Belt),
    //         _ => false
    //     };
    // }
    //
    // public bool CanMove(WindowType from, WindowType to, GameItem item)
    // {
    //     // ewentualne ograniczenia per okno
    //     return true;
    // }
    //
    // public bool CanStack(GameItem a, GameItem b)
    // {
    //     if (!a.IsStackable || !b.IsStackable) return false;
    //     if (a.Vnum != b.Vnum) return false;
    //     // tu można dodać porównanie socketów/attr jeśli wymagane 1:1
    //     return true;
    // }
}