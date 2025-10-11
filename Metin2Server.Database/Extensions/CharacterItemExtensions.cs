using Metin2Server.Database.Domain.Entities;
using Metin2Server.Shared.DbContracts.Common;
using Metin2Server.Shared.Extensions;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Database.Extensions;

public static class CharacterItemExtensions
{
    public static CharacterItemGrpc? ToProto(this CharacterItem? characterItem)
    {
        if (characterItem == null)
        {
            return null;
        }

        var dto = new CharacterItemGrpc
        {
            Id = characterItem.Id,
            WindowType = characterItem.WindowType.ToProto()!.Value,
            Position = characterItem.Position,
            Count = characterItem.Count,
            ItemDefinition = characterItem.ItemDefinition.ToProto(),
        };

        if (characterItem.Sockets is { Count: > 0 })
        {
            dto.Sockets.AddRange(characterItem.Sockets);
        }

        if (characterItem.Attributes is { Count: > 0 })
        {
            dto.Attributes
                .AddRange(characterItem.Attributes
                    .Select(itemAttribute => itemAttribute.ToProto())
                    .Where(itemAttribute => itemAttribute != null)!);
        }

        return dto;
    }

    public static CharacterItem? ToEntity(this CharacterItemGrpc? dto, Character character)
    {
        if (dto == null)
        {
            return null;
        }

        return new CharacterItem
        {
            WindowType = dto.WindowType.ToEntity()!.Value,
            Position = GrpcUtils.ToUShortChecked(dto.Position),
            Count = GrpcUtils.ToUIntChecked(dto.Count),
            ItemDefinition = dto.ItemDefinition.ToEntity()!,
            Sockets = dto.Sockets?.ToList() ?? [],
            Attributes =
                (dto.Attributes?
                    .Select(itemAttribute => itemAttribute.ToEntity())
                    .Where(itemAttribute => itemAttribute != null)!.ToList() ?? [])!,
            Character = character
        };
    }
}