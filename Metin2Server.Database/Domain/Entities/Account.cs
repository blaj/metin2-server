using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Metin2Server.Shared.Enums;

namespace Metin2Server.Database.Domain.Entities;

[Table("account", Schema = "account")]
public class Account : AuditingEntity
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public required string Login { get; set; }

    [Required] [StringLength(256)] public required string Password { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 3)]
    public required string Email { get; set; }

    [Required] public required bool Active { get; set; } = false;

    [Column(TypeName = "timestamp")] public DateTime? LastLogin { get; set; }
    
    [Required]
    [StringLength(7, MinimumLength = 7)]
    public required string PrivateCode { get; set; }

    public Empire? Empire { get; set; } = null;
    
    public List<Character> Characters { get; set; } = [];
}