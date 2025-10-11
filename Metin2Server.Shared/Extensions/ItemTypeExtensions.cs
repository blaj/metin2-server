using Metin2Server.Shared.DbContracts.Common;
using Metin2Server.Shared.Enums;

namespace Metin2Server.Shared.Extensions;

public static class ItemTypeExtensions
{
    public static ItemTypeGrpc? ToProto(this ItemType itemType) =>
        Enum.IsDefined(typeof(ItemTypeGrpc), (int)itemType)
            ? (ItemTypeGrpc)(int)itemType
            : null;

    public static ItemType? ToEntity(this ItemTypeGrpc itemType) =>
        Enum.IsDefined(typeof(ItemType), (int)itemType)
            ? (ItemType)(int)itemType
            : null;
}