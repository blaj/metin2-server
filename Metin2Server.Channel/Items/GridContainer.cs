using Metin2Server.Shared.Dto;
using Metin2Server.Shared.Enums;

namespace Metin2Server.Channel.Items;

public abstract class GridContainer
{
    public Dictionary<ushort, GameItem> Items { get; } = new();
    
    public abstract WindowType WindowType { get; }
    public abstract ushort Width { get; }
    public abstract ushort Height { get; }
    
    
}