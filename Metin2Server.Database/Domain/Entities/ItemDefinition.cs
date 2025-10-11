using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Metin2Server.Database.Domain.Entities.Owneds;
using Metin2Server.Shared.Enums;

namespace Metin2Server.Database.Domain.Entities;

[Table("item_definition", Schema = "dictionary")]
public class ItemDefinition : ArchiveEntity
{
    [Required] [StringLength(24)] public required string Name { get; set; }
    
    public required ItemType Type { get; set; }
    
    public required ItemSubType SubType { get; set; }

    public byte Size { get; set; } = 1;
    
    public required ItemAntiFlag AntiFlag { get; set; }
    
    public required ItemFlag Flag { get; set; }
    
    public required ItemWearFlag WearFlag { get; set; }

    public uint Price { get; set; } = 0;
    
    public uint ShopBuyPrice { get; set; } = 0;
    
    public List<ItemLimit> Limits { get; set; } = [new(), new()];

    public List<ItemAttribute> Attributes { get; set; } = [new(), new(), new()];

    public List<int> Values { get; set; } = [0, 0, 0, 0, 0, 0];

    public List<int> Sockets { get; set; } = [-1, -1, -1, -1, -1, -1];

    public byte MagicPct { get; set; } = 0;

    public byte Specular { get; set; } = 0;

    public byte SocketPct { get; set; } = 0;

    public ushort AddonType { get; set; } = 0;

    public List<CharacterItem> CharacterItems { get; set; } = new();
}