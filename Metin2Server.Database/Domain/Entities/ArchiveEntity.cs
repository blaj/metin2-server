using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metin2Server.Database.Domain.Entities;

public abstract class ArchiveEntity : IdEntity
{
    [Required]
    [Column(TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column(TypeName = "timestamp")] public DateTime? UpdatedAt { get; set; }

    [Column(TypeName = "timestamp")] public DateTime? ArchivedAt { get; set; }

    [Required] public bool Archived { get; set; } = false;
}