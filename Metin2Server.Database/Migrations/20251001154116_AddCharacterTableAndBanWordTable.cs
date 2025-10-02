using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Metin2Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterTableAndBanWordTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dictionary");

            migrationBuilder.EnsureSchema(
                name: "character");

            migrationBuilder.AddColumn<string>(
                name: "empire",
                schema: "account",
                table: "account",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "banned_word",
                schema: "dictionary",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    word = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    archived_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    archived = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_banned_word", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "character",
                schema: "character",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    job = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    index = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                    part_base = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                    part_main = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    part_hair = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    skill_group = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                    coordinates_dir = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                    coordinates_x = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    coordinates_y = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    coordinates_z = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    coordinates_map_index = table.Column<long>(type: "bigint", nullable: true),
                    coordinates_exit_x = table.Column<int>(type: "integer", nullable: true),
                    coordinates_exit_y = table.Column<int>(type: "integer", nullable: true),
                    coordinates_exit_map_index = table.Column<long>(type: "bigint", nullable: true),
                    statistics_hp = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_sp = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_stamina = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_random_hp = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_random_sp = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_play_time = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    statistics_level = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                    statistics_level_step = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                    statistics_st = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_ht = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_dx = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_iq = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_exp = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    statistics_gold = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    statistics_stat_point = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_skill_point = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_sub_skill_point = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_horse_skill_point = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_alignment = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    statistics_stat_reset_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    horse_statistics_level = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                    horse_statistics_riding = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    horse_statistics_stamina = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    horse_statistics_health = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    horse_statistics_health_drop_time = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_character", x => x.id);
                    table.ForeignKey(
                        name: "fk_character_account_account_id",
                        column: x => x.account_id,
                        principalSchema: "account",
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quick_slot",
                schema: "character",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    position = table.Column<byte>(type: "smallint", nullable: false),
                    character_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quick_slot", x => x.id);
                    table.ForeignKey(
                        name: "fk_quick_slot_character_character_id",
                        column: x => x.character_id,
                        principalSchema: "character",
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "skill",
                schema: "character",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    master_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    level = table.Column<byte>(type: "smallint", nullable: false),
                    next_read = table.Column<int>(type: "integer", nullable: false),
                    character_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_skill", x => x.id);
                    table.ForeignKey(
                        name: "fk_skill_character_character_id",
                        column: x => x.character_id,
                        principalSchema: "character",
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_character_account_id",
                schema: "character",
                table: "character",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_quick_slot_character_id",
                schema: "character",
                table: "quick_slot",
                column: "character_id");

            migrationBuilder.CreateIndex(
                name: "ix_skill_character_id",
                schema: "character",
                table: "skill",
                column: "character_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "banned_word",
                schema: "dictionary");

            migrationBuilder.DropTable(
                name: "quick_slot",
                schema: "character");

            migrationBuilder.DropTable(
                name: "skill",
                schema: "character");

            migrationBuilder.DropTable(
                name: "character",
                schema: "character");

            migrationBuilder.DropColumn(
                name: "empire",
                schema: "account",
                table: "account");
        }
    }
}
