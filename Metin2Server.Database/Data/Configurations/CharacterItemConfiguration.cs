using System.Text.Json;
using Metin2Server.Database.Domain.Entities;
using Metin2Server.Database.Domain.Entities.Owneds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Metin2Server.Database.Data.Configurations;

public class CharacterItemConfiguration : IEntityTypeConfiguration<CharacterItem>
{
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.General);

    private static ValueComparer<List<T>> JsonListComparer<T>() => new(
        (a, b) => JsonSerializer.Serialize(a, JsonOpts) == JsonSerializer.Serialize(b, JsonOpts),
        v => v == null ? 0 : JsonSerializer.Serialize(v, JsonOpts).GetHashCode(),
        v => v == null
            ? new List<T>()
            : JsonSerializer.Deserialize<List<T>>(JsonSerializer.Serialize(v, JsonOpts), JsonOpts)!);


    public void Configure(EntityTypeBuilder<CharacterItem> builder)
    {
        builder
            .Property(characterItem => characterItem.WindowType)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(characterItem => characterItem.Count).HasDefaultValue(1);
        
        builder.Property(characterItem => characterItem.Sockets)
            .HasColumnName("sockets")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'[-1,-1,-1,-1,-1,-1]'::jsonb")
            .Metadata.SetValueComparer(JsonListComparer<int>());
        
        builder.Property(characterItem => characterItem.Attributes)
            .HasColumnName("attributes")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'[{\"Type\":0,\"Value\":0},{\"Type\":0,\"Value\":0},{\"Type\":0,\"Value\":0},{\"Type\":0,\"Value\":0},{\"Type\":0,\"Value\":0},{\"Type\":0,\"Value\":0},{\"Type\":0,\"Value\":0}]'::jsonb")
            .Metadata.SetValueComparer(JsonListComparer<ItemAttribute>());
        
        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_character_item_sockets_len", "jsonb_array_length(sockets) = 6");
            table.HasCheckConstraint("ck_character_attributes_len", "jsonb_array_length(attributes) = 7");
        });
    }
}