using Metin2Server.Shared.Enums;

namespace Metin2Server.Database.Domain.Entities.Owneds;

public class ItemLimit
{
    public ItemLimitType Type { get; set; } = ItemLimitType.None;

    public byte Value { get; set; } = 0;
}