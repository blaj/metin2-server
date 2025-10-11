using Metin2Server.Shared.Enums;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Shared.Extensions;

public static class ItemFlagExtensions
{
    public static ulong ToProto(this ItemFlag value) => (ulong)(uint)value;

    public static ItemFlag ToFlag(this ulong value) =>
        (ItemFlag)GrpcUtils.ToUIntChecked((long)value);
}