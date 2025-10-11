using Metin2Server.Shared.Enums;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Shared.Extensions;

public static class ItemWearFlagExtensions
{
    public static ulong ToProto(this ItemWearFlag value) => (ulong)(uint)value;
    
    public static ItemWearFlag ToWearFlag(this ulong value) =>
        (ItemWearFlag)GrpcUtils.ToUIntChecked((long)value);
}