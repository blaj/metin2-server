using System.ComponentModel.DataAnnotations.Schema;

namespace Metin2Server.Database.Domain.Entities;

[Table("skill", Schema = "character")]
public class CharacterSkill : IdEntity
{
    public required SkillMasterType MasterType { get; set; }

    public required byte Level { get; set; }

    public required int NextRead { get; set; }
    
    public required Character Character { get; set; }
    
    public enum SkillMasterType : byte
    {
        Normal,
        Master,
        GrandMaster,
        PerfectMaster,
    }
}