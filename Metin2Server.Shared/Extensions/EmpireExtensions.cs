using Metin2Server.Shared.Dto;
using Metin2Server.Shared.Enums;

namespace Metin2Server.Shared.Extensions;

public static class EmpireExtensions
{
    public static Coords GetStartPosition(this Empire empire) => empire switch
    {
        Empire.Shinsoo => new Coords { X = 459800, Y = 953900, Z = 0 },
        Empire.Chunjo => new Coords { X = 52070, Y = 166600, Z = 0 },
        Empire.Jinno => new Coords { X = 957300, Y = 255200, Z = 0 },
        _ => throw new ArgumentOutOfRangeException(nameof(empire), empire, null)
    };

    public static DbContracts.Common.Empire ToProto(this Empire empire) => empire switch
    {
        Empire.Shinsoo => DbContracts.Common.Empire.Shinsoo,
        Empire.Chunjo => DbContracts.Common.Empire.Chunjo,
        Empire.Jinno => DbContracts.Common.Empire.Jinno,
        _ => DbContracts.Common.Empire.Unknown
    };

    public static Empire ToEntity(this DbContracts.Common.Empire value) => value switch
    {
        DbContracts.Common.Empire.Shinsoo => Empire.Shinsoo,
        DbContracts.Common.Empire.Chunjo => Empire.Chunjo,
        DbContracts.Common.Empire.Jinno => Empire.Jinno,
        _ => Empire.Shinsoo
    };
}