using Metin2Server.Shared.Enums;

namespace Metin2Server.Database.Domain.Entities.Owneds;

public class ItemAttribute
{
    public ItemAttributeType Type { get; set; } = ItemAttributeType.None;

    public byte Value { get; set; } = 0;
}