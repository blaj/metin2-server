using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metin2Server.Database.Domain.Entities;

[Table("banned_word", Schema = "dictionary")]
public class BannedWord : ArchiveEntity
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public required string Word { get; set; }
}