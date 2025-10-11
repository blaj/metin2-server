using Metin2Server.Database.Domain.Entities.Owneds;
using Metin2Server.Shared.DbContracts.Common;
using Metin2Server.Shared.Enums;
using Metin2Server.Shared.Extensions;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Database.Extensions;

public static class ItemAttributeExtensions
{
    public static ItemAttributeGrpc? ToProto(this ItemAttribute? entity)
    {
        if (entity == null)
            return null;

        return new ItemAttributeGrpc
        {
            Type = entity.Type.ToProto() ?? ItemAttributeTypeGrpc.None,
            Value = entity.Value
        };
    }

    public static ItemAttribute? ToEntity(this ItemAttributeGrpc? dto)
    {
        if (dto == null)
            return null;

        return new ItemAttribute
        {
            Type = dto.Type.ToEntity() ?? ItemAttributeType.None,
            Value = GrpcUtils.ToByteChecked(dto.Value)
        };
    }
}