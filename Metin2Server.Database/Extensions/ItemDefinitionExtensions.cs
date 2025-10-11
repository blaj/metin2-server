using Metin2Server.Database.Domain.Entities;
using Metin2Server.Shared.DbContracts.Common;
using Metin2Server.Shared.Extensions;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Database.Extensions;

public static class ItemDefinitionExtensions
{
    public static ItemDefinition? ToEntity(this ItemDefinitionGrpc? dto)
    {
        if (dto == null)
        {
            return null;
        }


        return new ItemDefinition
        {
            Name = dto.Name,
            Type = dto.Type.ToEntity()!.Value,
            SubType = dto.SubType.ToEntity()!.Value,
            Size = GrpcUtils.ToByteChecked(dto.Size),
            AntiFlag = dto.AntiFlag.ToAntiFlag(),
            Flag = dto.Flag.ToFlag(),
            WearFlag = dto.WearFlag.ToWearFlag(),
            Price = GrpcUtils.ToUIntChecked(dto.Price),
            ShopBuyPrice = GrpcUtils.ToUIntChecked(dto.ShopBuyPrice),
            Limits = (dto.Limits.Select(x => x.ToEntity()).Where(x => x != null)!.ToList() ?? [])!,
            Attributes = (dto.Attributes?.Select(x => x.ToEntity()).Where(x => x != null)!.ToList() ?? [])!,
            Values = dto.Values?.ToList() ?? [],
            Sockets = dto.Sockets?.ToList() ?? [],
            MagicPct = GrpcUtils.ToByteChecked(dto.MagicPct),
            Specular = GrpcUtils.ToByteChecked(dto.Specular),
            SocketPct = GrpcUtils.ToByteChecked(dto.SocketPct),
            AddonType = GrpcUtils.ToUShortChecked(dto.AddonType),
        };
    }

    public static ItemDefinitionGrpc ToProto(this ItemDefinition itemDefinition)
    {
        var dto = new ItemDefinitionGrpc
        {
            Id = itemDefinition.Id,
            Name = itemDefinition.Name,
            Type = itemDefinition.Type.ToProto()!.Value,
            SubType = itemDefinition.SubType.ToProto()!.Value,
            Size = itemDefinition.Size,
            AntiFlag = itemDefinition.AntiFlag.ToProto(),
            Flag = itemDefinition.Flag.ToProto(),
            WearFlag = itemDefinition.WearFlag.ToProto(),
            Price = itemDefinition.Price,
            ShopBuyPrice = itemDefinition.ShopBuyPrice,
            MagicPct = itemDefinition.MagicPct,
            Specular = itemDefinition.Specular,
            SocketPct = itemDefinition.SocketPct,
            AddonType = itemDefinition.AddonType,
        };

        if (itemDefinition.Limits is { Count: > 0 })
        {
            dto.Limits.AddRange(
                itemDefinition.Limits
                    .Select(itemLimit => itemLimit.ToProto())
                    .Where(itemLimit => itemLimit != null)!);
        }

        if (itemDefinition.Attributes is { Count: > 0 })
        {
            dto.Attributes.AddRange(
                itemDefinition.Attributes
                    .Select(itemAttribute => itemAttribute.ToProto())
                    .Where(itemAttribute => itemAttribute != null)!);
        }

        if (itemDefinition.Values is { Count: > 0 })
        {
            dto.Values.AddRange(itemDefinition.Values);
        }

        if (itemDefinition.Sockets is { Count: > 0 })
        {
            dto.Sockets.AddRange(itemDefinition.Sockets);
        }

        return dto;
    }
}