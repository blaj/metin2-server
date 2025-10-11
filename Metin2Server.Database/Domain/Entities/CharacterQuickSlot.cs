using System.ComponentModel.DataAnnotations.Schema;

namespace Metin2Server.Database.Domain.Entities;

[Table("quick_slot", Schema = "character")]
public class CharacterQuickSlot : IdEntity
{
    public required QuickSlotType Type { get; set; }

    public required byte Position { get; set; }
    
    public required Character Character { get; set; }
    
    public enum QuickSlotType: byte
    {
        None,
        Item,
        Skill,
        Command,
    }
}