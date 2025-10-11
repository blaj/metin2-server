using Metin2Server.Shared.DbContracts.Common;
using Metin2Server.Shared.Enums;

namespace Metin2Server.Shared.Extensions;

public static class ItemAttributeTypeExtensions
{
    public static ItemAttributeTypeGrpc? ToProto(this ItemAttributeType attributeType) =>
        Enum.IsDefined(typeof(ItemAttributeTypeGrpc), (int)attributeType)
            ? (ItemAttributeTypeGrpc)(int)attributeType
            : null;

    public static ItemAttributeType? ToEntity(this ItemAttributeTypeGrpc attributeType) =>
        Enum.IsDefined(typeof(ItemAttributeType), (int)attributeType)
            ? (ItemAttributeType)(int)attributeType
            : null;
}