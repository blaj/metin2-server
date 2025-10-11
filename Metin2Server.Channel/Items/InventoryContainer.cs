using Metin2Server.Shared.Enums;

namespace Metin2Server.Channel.Items;

public class InventoryContainer : GridContainer
{
    public override WindowType WindowType { get; } = WindowType.Inventory;
    public override ushort Width { get; } = 5;
    public override ushort Height { get; } = 2 * 9;
    
    
}