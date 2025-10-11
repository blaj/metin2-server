using Metin2Server.Shared.DbContracts.Common;
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

    public static EmpireGrpc ToProto(this Empire empire) => empire switch
    {
        Empire.Shinsoo => EmpireGrpc.Shinsoo,
        Empire.Chunjo => EmpireGrpc.Chunjo,
        Empire.Jinno => EmpireGrpc.Jinno,
        _ => EmpireGrpc.Unknown
    };

    public static Empire ToEntity(this EmpireGrpc value) => value switch
    {
        EmpireGrpc.Shinsoo => Empire.Shinsoo,
        EmpireGrpc.Chunjo => Empire.Chunjo,
        EmpireGrpc.Jinno => Empire.Jinno,
        _ => Empire.Shinsoo
    };
}