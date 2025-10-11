using Metin2Server.Shared.Enums;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Shared.Extensions;

public static class ItemAntiFlagExtensions
{
    public static ulong ToProto(this ItemAntiFlag value) => (ulong)(uint)value;

    public static ItemAntiFlag ToAntiFlag(this ulong value) =>
        (ItemAntiFlag)GrpcUtils.ToUIntChecked((long)value);
}