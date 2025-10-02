using Metin2Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Metin2Server.Database.Data.Configurations;

public class CharacterQuickSlotConfiguration : IEntityTypeConfiguration<CharacterQuickSlot>
{
    public void Configure(EntityTypeBuilder<CharacterQuickSlot> builder)
    {
        builder
            .Property(characterQuickSlot => characterQuickSlot.Type)
            .HasConversion<string>()
            .HasMaxLength(10);
    }
}