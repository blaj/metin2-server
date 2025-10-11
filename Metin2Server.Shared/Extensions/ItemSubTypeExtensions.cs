using Metin2Server.Shared.DbContracts.Common;
using Metin2Server.Shared.Enums;

namespace Metin2Server.Shared.Extensions;

public static class ItemSubTypeExtensions
{
    public static ItemSubTypeGrpc? ToProto(this ItemSubType itemSubType) =>
        Enum.IsDefined(typeof(ItemSubTypeGrpc), (int)itemSubType)
            ? (ItemSubTypeGrpc)(int)itemSubType
            : null;

    public static ItemSubType? ToEntity(this ItemSubTypeGrpc itemSubType) =>
        Enum.IsDefined(typeof(ItemSubType), (int)itemSubType)
            ? (ItemSubType)(int)itemSubType
            : null;
}