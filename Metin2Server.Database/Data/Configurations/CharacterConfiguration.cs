using Metin2Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Metin2Server.Database.Data.Configurations;

public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.AddAuditingEntityConfiguration();

        builder
            .Property(character => character.Race)
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(character => character.Index).HasDefaultValue(0);
        builder.Property(character => character.PartBase).HasDefaultValue(0);
        builder.Property(character => character.PartMain).HasDefaultValue(0);
        builder.Property(character => character.PartHair).HasDefaultValue(0);
        builder.Property(character => character.SkillGroup).HasDefaultValue(0);

        builder.OwnsOne(character => character.Coordinates, coordinates =>
        {
            coordinates.Property(c => c.Dir).HasDefaultValue(0);
            coordinates.Property(c => c.X).HasDefaultValue(0);
            coordinates.Property(c => c.Y).HasDefaultValue(0);
            coordinates.Property(c => c.Z).HasDefaultValue(0);
        });

        builder.OwnsOne(character => character.Statistics, statistics =>
        {
            statistics.Property(s => s.Hp).HasDefaultValue(0);
            statistics.Property(s => s.Sp).HasDefaultValue(0);
            statistics.Property(s => s.Stamina).HasDefaultValue(0);
            statistics.Property(s => s.RandomHp).HasDefaultValue(0);
            statistics.Property(s => s.RandomSp).HasDefaultValue(0);
            statistics.Property(s => s.PlayTime).HasDefaultValue(0);
            statistics.Property(s => s.Level).HasDefaultValue(0);
            statistics.Property(s => s.LevelStep).HasDefaultValue(0);
            statistics.Property(s => s.St).HasDefaultValue(0);
            statistics.Property(s => s.Ht).HasDefaultValue(0);
            statistics.Property(s => s.Dx).HasDefaultValue(0);
            statistics.Property(s => s.Iq).HasDefaultValue(0);
            statistics.Property(s => s.Exp).HasDefaultValue(0);
            statistics.Property(s => s.Gold).HasDefaultValue(0);
            statistics.Property(s => s.StatPoint).HasDefaultValue(0);
            statistics.Property(s => s.SkillPoint).HasDefaultValue(0);
            statistics.Property(s => s.SubSkillPoint).HasDefaultValue(0);
            statistics.Property(s => s.HorseSkillPoint).HasDefaultValue(0);
            statistics.Property(s => s.Alignment).HasDefaultValue(0);
            statistics.Property(s => s.StatResetCount).HasDefaultValue(0);
        });

        builder.OwnsOne(character => character.HorseStatistics, horseStatistics =>
        {
            horseStatistics.Property(hs => hs.Level).HasDefaultValue(0);
            horseStatistics.Property(hs => hs.Riding).HasDefaultValue(false);
            horseStatistics.Property(hs => hs.Stamina).HasDefaultValue(0);
            horseStatistics.Property(hs => hs.Health).HasDefaultValue(0);
            horseStatistics.Property(hs => hs.HealthDropTime).HasDefaultValue(0);
        });

        builder
            .HasMany(character => character.Skills)
            .WithOne(characterSkill => characterSkill.Character)
            .IsRequired();

        builder
            .HasMany(character => character.QuickSlots)
            .WithOne(characterQuickSlot => characterQuickSlot.Character)
            .IsRequired();

        builder
            .HasMany(character => character.Items)
            .WithOne(characterItem => characterItem.Character)
            .IsRequired();
    }
}