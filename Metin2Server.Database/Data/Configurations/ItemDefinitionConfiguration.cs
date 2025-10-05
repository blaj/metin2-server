using System.Text.Json;
using Metin2Server.Domain.Entities;
using Metin2Server.Domain.Entities.Owneds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Metin2Server.Database.Data.Configurations;

public class ItemDefinitionConfiguration : IEntityTypeConfiguration<ItemDefinition>
{
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.General);

    private static ValueComparer<List<T>> JsonListComparer<T>() => new(
        (a, b) => JsonSerializer.Serialize(a, JsonOpts) == JsonSerializer.Serialize(b, JsonOpts),
        v => v == null ? 0 : JsonSerializer.Serialize(v, JsonOpts).GetHashCode(),
        v => v == null
            ? new List<T>()
            : JsonSerializer.Deserialize<List<T>>(JsonSerializer.Serialize(v, JsonOpts), JsonOpts)!);

    public void Configure(EntityTypeBuilder<ItemDefinition> builder)
    {
        builder.AddArchiveEntityConfiguration();

        builder
            .Property(itemDefinition => itemDefinition.Type)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder
            .Property(itemDefinition => itemDefinition.SubType)
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(itemDefinition => itemDefinition.Size).HasDefaultValue(1);
        builder.Property(itemDefinition => itemDefinition.Price).HasDefaultValue(0);
        builder.Property(itemDefinition => itemDefinition.ShopBuyPrice).HasDefaultValue(0);

        builder.Property(itemDefinition => itemDefinition.Limits)
            .HasColumnName("limits")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'[{\"Type\":0,\"Value\":0},{\"Type\":0,\"Value\":0}]'::jsonb")
            .Metadata.SetValueComparer(JsonListComparer<ItemLimit>());

        builder.Property(itemDefinition => itemDefinition.Attributes)
            .HasColumnName("attributes")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'[{\"Type\":0,\"Value\":0},{\"Type\":0,\"Value\":0},{\"Type\":0,\"Value\":0}]'::jsonb")
            .Metadata.SetValueComparer(JsonListComparer<ItemAttribute>());

        builder.Property(itemDefinition => itemDefinition.Values)
            .HasColumnName("values")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'[0,0,0,0,0,0]'::jsonb")
            .Metadata.SetValueComparer(JsonListComparer<int>());

        builder.Property(itemDefinition => itemDefinition.Sockets)
            .HasColumnName("sockets")
            .HasColumnType("jsonb")
            .HasDefaultValueSql("'[-1,-1,-1,-1,-1,-1]'::jsonb")
            .Metadata.SetValueComparer(JsonListComparer<int>());

        builder.Property(itemDefinition => itemDefinition.MagicPct).HasDefaultValue(0);
        builder.Property(itemDefinition => itemDefinition.Specular).HasDefaultValue(0);
        builder.Property(itemDefinition => itemDefinition.SocketPct).HasDefaultValue(0);
        builder.Property(itemDefinition => itemDefinition.AddonType).HasDefaultValue(0);

        builder
            .HasMany(itemDefinition => itemDefinition.CharacterItems)
            .WithOne(characterItem => characterItem.ItemDefinition)
            .IsRequired();

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_item_definition_limits_len", "jsonb_array_length(limits) = 2");
            table.HasCheckConstraint("ck_item_definition_attrs_len", "jsonb_array_length(attributes) = 3");
            table.HasCheckConstraint("ck_item_definition_values_len", "jsonb_array_length(values) = 6");
            table.HasCheckConstraint("ck_item_definition_sockets_len", "jsonb_array_length(sockets) = 6");
        });
    }
}