using Metin2Server.Shared.Dto;
using Metin2Server.Shared.Enums;

namespace Metin2Server.Channel.Items;

public class EquipmentContainer
{
    public WindowType Window { get; } = WindowType.Equipment;
    public Dictionary<EquipSlot, GameItem> EquippedItems { get; } = new Dictionary<EquipSlot, GameItem>();
}