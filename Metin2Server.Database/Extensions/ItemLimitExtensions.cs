using Metin2Server.Database.Domain.Entities.Owneds;
using Metin2Server.Shared.DbContracts.Common;
using Metin2Server.Shared.Enums;
using Metin2Server.Shared.Extensions;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Database.Extensions;

public static class ItemLimitExtensions
{
    public static ItemLimitGrpc? ToProto(this ItemLimit? itemLimit)
    {
        if (itemLimit == null)
        {
            return null;
        }

        return new ItemLimitGrpc
        {
            Type = itemLimit.Type.ToProto() ?? ItemLimitTypeGrpc.None,
            Value = itemLimit.Value
        };
    }

    public static ItemLimit? ToEntity(this ItemLimitGrpc? dto)
    {
        if (dto == null)
            return null;

        return new ItemLimit
        {
            Type = dto.Type.ToEntity() ?? ItemLimitType.None,
            Value = GrpcUtils.ToByteChecked(dto.Value)
        };
    }
}