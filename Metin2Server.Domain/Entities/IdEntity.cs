using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metin2Server.Domain.Entities;

public abstract class IdEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        return obj is IdEntity other && other.Id == Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}