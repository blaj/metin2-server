using Metin2Server.Database.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Metin2Server.Database.Data.Configurations;

public static class ArchiveEntityConfiguration
{
    public static void AddArchiveEntityConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : ArchiveEntity
    {
        builder
            .Property(entity => entity.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder
            .Property(entity => entity.Archived)
            .HasDefaultValue(false);
    }
}