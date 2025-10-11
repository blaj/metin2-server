using System.ComponentModel.DataAnnotations.Schema;
using Metin2Server.Database.Domain.Entities.Owneds;
using Metin2Server.Shared.Enums;

namespace Metin2Server.Database.Domain.Entities;

[Table("item", Schema = "character")]
public class CharacterItem : AuditingEntity
{
    public required WindowType WindowType { get; set; }

    public required ushort Position { get; set; }

    public uint Count { get; set; } = 1;

    public required ItemDefinition ItemDefinition { get; set; }

    public List<int> Sockets { get; set; } = [0, 0, 0, 0, 0, 0];

    public List<ItemAttribute> Attributes { get; set; } = [new(), new(), new(), new(), new(), new(), new()];

    public required Character Character { get; set; }
}