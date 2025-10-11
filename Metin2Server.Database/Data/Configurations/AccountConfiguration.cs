using Metin2Server.Database.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Metin2Server.Database.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.AddAuditingEntityConfiguration();

        builder
            .Property(account => account.Active)
            .HasDefaultValue(false);

        builder
            .HasIndex(account => account.Login)
            .IsUnique();

        builder
            .HasIndex(account => account.Email)
            .IsUnique();

        builder
            .Property(account => account.Empire)
            .HasConversion<string>()
            .HasMaxLength(10);

        builder
            .HasMany(account => account.Characters)
            .WithOne(character => character.Account)
            .IsRequired();
    }
}