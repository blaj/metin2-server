using Metin2Server.Shared.Enums;

namespace Metin2Server.Channel.Items;

public class ItemPosition
{
    public required WindowType WindowType { get; set; }
    public required ushort Cell { get; set; }
}