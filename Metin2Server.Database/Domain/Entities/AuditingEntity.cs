using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metin2Server.Database.Domain.Entities;

public class AuditingEntity : IdEntity
{
    [Required]
    [Column(TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Column(TypeName = "timestamp")] public DateTime? UpdatedAt { get; set; }

    [Column(TypeName = "timestamp")] public DateTime? DeletedAt { get; set; }

    [Required] public bool Deleted { get; set; } = false;
}