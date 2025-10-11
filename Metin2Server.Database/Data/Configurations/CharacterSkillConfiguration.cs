using Metin2Server.Database.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Metin2Server.Database.Data.Configurations;

public class CharacterSkillConfiguration : IEntityTypeConfiguration<CharacterSkill>
{
    public void Configure(EntityTypeBuilder<CharacterSkill> builder)
    {
        builder
            .Property(characterSkill => characterSkill.MasterType)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}