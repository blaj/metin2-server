using Metin2Server.Database.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Metin2Server.Database.Data.Configurations;

public class BannedWordConfiguration : IEntityTypeConfiguration<BannedWord>
{
    public void Configure(EntityTypeBuilder<BannedWord> builder)
    {
        builder.AddArchiveEntityConfiguration();
    }
}