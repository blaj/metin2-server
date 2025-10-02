using Metin2Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Metin2Server.Database.Data.Configurations;

public static class AuditingEntityConfiguration
{
    public static void AddAuditingEntityConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : AuditingEntity
    {
        builder.HasQueryFilter(entity => !entity.Deleted);

        builder
            .Property(entity => entity.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder
            .Property(entity => entity.Deleted)
            .HasDefaultValue(false);
    }
}