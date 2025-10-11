using Metin2Server.Shared.DbContracts.Common;
using Metin2Server.Shared.Enums;

namespace Metin2Server.Shared.Extensions;

public static class ItemLimitTypeExtensions
{
    public static ItemLimitTypeGrpc? ToProto(this ItemLimitType limitType) =>
        Enum.IsDefined(typeof(ItemLimitTypeGrpc), (int)limitType)
            ? (ItemLimitTypeGrpc)(int)limitType
            : null;

    public static ItemLimitType? ToEntity(this ItemLimitTypeGrpc limitType) =>
        Enum.IsDefined(typeof(ItemLimitType), (int)limitType)
            ? (ItemLimitType)(int)limitType
            : null;
}