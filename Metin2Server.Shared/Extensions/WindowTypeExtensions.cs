using Metin2Server.Shared.DbContracts.Common;
using Metin2Server.Shared.Enums;

namespace Metin2Server.Shared.Extensions;

public static class WindowTypeExtensions
{
    public static WindowTypeGrpc? ToProto(this WindowType type) =>
        Enum.IsDefined(typeof(WindowTypeGrpc), (int)type)
            ? (WindowTypeGrpc)(int)type
            : null;

    public static WindowType? ToEntity(this WindowTypeGrpc type) =>
        Enum.IsDefined(typeof(WindowType), (int)type)
            ? (WindowType)(int)type
            : null;
}