using System;
using System.Collections.Generic;
using Metin2Server.Database.Domain.Entities.Owneds;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Metin2Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddItemDefinitionTableAndCharacterItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "item_definition",
                schema: "dictionary",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    sub_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    size = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)1),
                    anti_flag = table.Column<long>(type: "bigint", nullable: false),
                    flag = table.Column<long>(type: "bigint", nullable: false),
                    wear_flag = table.Column<long>(type: "bigint", nullable: false),
                    price = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    shop_buy_price = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    limits = table.Column<List<ItemLimit>>(type: "jsonb", nullable: false, defaultValueSql: "'[{\"Type\":0,\"Value\":0},{\"Type\":0,\"Value\":0}]'::jsonb"),
                    attributes = table.Column<List<ItemAttribute>>(type: "jsonb", nullable: false, defaultValueSql: "'[{\"Type\":0,\"Value\":0},{\"Type\":0,\"Value\":0},{\"Type\":0,\"Value\":0}]'::jsonb"),
                    values = table.Column<List<int>>(type: "jsonb", nullable: false, defaultValueSql: "'[0,0,0,0,0,0]'::jsonb"),
                    sockets = table.Column<List<int>>(type: "jsonb", nullable: false, defaultValueSql: "'[-1,-1,-1,-1,-1,-1]'::jsonb"),
                    magic_pct = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                    specular = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                    socket_pct = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                    addon_type = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    archived_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    archived = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_definition", x => x.id);
                    table.CheckConstraint("ck_item_definition_attrs_len", "jsonb_array_length(attributes) = 3");
                    table.CheckConstraint("ck_item_definition_limits_len", "jsonb_array_length(limits) = 2");
                    table.CheckConstraint("ck_item_definition_sockets_len", "jsonb_array_length(sockets) = 6");
                    table.CheckConstraint("ck_item_definition_values_len", "jsonb_array_length(values) = 6");
                });

            migrationBuilder.CreateTable(
                name: "item",
                schema: "character",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    window_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    count = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    item_definition_id = table.Column<long>(type: "bigint", nullable: false),
                    sockets = table.Column<List<int>>(type: "jsonb", nullable: false, defaultValueSql: "'[-1,-1,-1,-1,-1,-1]'::jsonb"),
                    attributes = table.Column<List<ItemAttribute>>(type: "jsonb", nullable: false, defaultValueSql: "'[{\"Type\":0,\"Value\":0},{\"Type\":0,\"Value\":0},{\"Type\":0,\"Value\":0},{\"Type\":0,\"Value\":0},{\"Type\":0,\"Value\":0},{\"Type\":0,\"Value\":0},{\"Type\":0,\"Value\":0}]'::jsonb"),
                    character_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item", x => x.id);
                    table.CheckConstraint("ck_character_attributes_len", "jsonb_array_length(attributes) = 7");
                    table.CheckConstraint("ck_character_item_sockets_len", "jsonb_array_length(sockets) = 6");
                    table.ForeignKey(
                        name: "fk_item_character_character_id",
                        column: x => x.character_id,
                        principalSchema: "character",
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_item_item_definition_item_definition_id",
                        column: x => x.item_definition_id,
                        principalSchema: "dictionary",
                        principalTable: "item_definition",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_item_character_id",
                schema: "character",
                table: "item",
                column: "character_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_item_definition_id",
                schema: "character",
                table: "item",
                column: "item_definition_id");

            migrationBuilder.Sql("GRANT USAGE ON SCHEMA character TO metin_app_user;");
            migrationBuilder.Sql("GRANT INSERT, SELECT, UPDATE ON character.item TO metin_app_user;");
            migrationBuilder.Sql("GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA character TO metin_app_user;");

            migrationBuilder.Sql("GRANT USAGE ON SCHEMA dictionary TO metin_app_user;");
            migrationBuilder.Sql("GRANT INSERT, SELECT, UPDATE ON dictionary.item_definition TO metin_app_user;");
            migrationBuilder.Sql("GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA dictionary TO metin_app_user;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "item",
                schema: "character");

            migrationBuilder.DropTable(
                name: "item_definition",
                schema: "dictionary");
        }
    }
}
