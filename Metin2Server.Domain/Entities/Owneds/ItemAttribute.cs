using Metin2Server.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Metin2Server.Domain.Entities.Owneds;

public class ItemAttribute
{
    public ItemAttributeType Type { get; set; } = ItemAttributeType.None;

    public byte Value { get; set; } = 0;
}