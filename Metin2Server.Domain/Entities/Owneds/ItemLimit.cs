using Metin2Server.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Metin2Server.Domain.Entities.Owneds;

public class ItemLimit
{
    public ItemLimitType Type { get; set; } = ItemLimitType.None;

    public byte Value { get; set; } = 0;
}