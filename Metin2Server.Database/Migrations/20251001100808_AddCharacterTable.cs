using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Metin2Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "character",
                schema: "character",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    job = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    part_base = table.Column<byte>(type: "smallint", nullable: false),
                    part_main = table.Column<int>(type: "integer", nullable: false),
                    part_hair = table.Column<int>(type: "integer", nullable: false),
                    skill_group = table.Column<byte>(type: "smallint", nullable: false),
                    coordinates_dir = table.Column<byte>(type: "smallint", nullable: false),
                    coordinates_x = table.Column<int>(type: "integer", nullable: false),
                    coordinates_y = table.Column<int>(type: "integer", nullable: false),
                    coordinates_z = table.Column<int>(type: "integer", nullable: false),
                    coordinates_map_index = table.Column<long>(type: "bigint", nullable: true),
                    coordinates_exit_x = table.Column<int>(type: "integer", nullable: true),
                    coordinates_exit_y = table.Column<int>(type: "integer", nullable: true),
                    coordinates_exit_map_index = table.Column<long>(type: "bigint", nullable: true),
                    statistics_hp = table.Column<int>(type: "integer", nullable: false),
                    statistics_sp = table.Column<int>(type: "integer", nullable: false),
                    statistics_stamina = table.Column<int>(type: "integer", nullable: false),
                    statistics_random_hp = table.Column<int>(type: "integer", nullable: false),
                    statistics_random_sp = table.Column<int>(type: "integer", nullable: false),
                    statistics_play_time = table.Column<long>(type: "bigint", nullable: false),
                    statistics_level = table.Column<byte>(type: "smallint", nullable: false),
                    statistics_level_step = table.Column<byte>(type: "smallint", nullable: false),
                    statistics_st = table.Column<int>(type: "integer", nullable: false),
                    statistics_ht = table.Column<int>(type: "integer", nullable: false),
                    statistics_dx = table.Column<int>(type: "integer", nullable: false),
                    statistics_iq = table.Column<int>(type: "integer", nullable: false),
                    statistics_exp = table.Column<long>(type: "bigint", nullable: false),
                    statistics_gold = table.Column<long>(type: "bigint", nullable: false),
                    statistics_stat_point = table.Column<int>(type: "integer", nullable: false),
                    statistics_skill_point = table.Column<int>(type: "integer", nullable: false),
                    statistics_sub_skill_point = table.Column<int>(type: "integer", nullable: false),
                    statistics_horse_skill_point = table.Column<int>(type: "integer", nullable: false),
                    statistics_alignment = table.Column<int>(type: "integer", nullable: false),
                    statistics_stat_reset_count = table.Column<int>(type: "integer", nullable: false),
                    horse_statistics_level = table.Column<byte>(type: "smallint", nullable: false),
                    horse_statistics_riding = table.Column<bool>(type: "boolean", nullable: false),
                    horse_statistics_stamina = table.Column<short>(type: "smallint", nullable: false),
                    horse_statistics_health = table.Column<short>(type: "smallint", nullable: false),
                    horse_statistics_health_drop_time = table.Column<int>(type: "integer", nullable: false),
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
                name: "character_quick_slot",
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
                    table.PrimaryKey("pk_character_quick_slot", x => x.id);
                    table.ForeignKey(
                        name: "fk_character_quick_slot_character_character_id",
                        column: x => x.character_id,
                        principalSchema: "character",
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_skill",
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
                    table.PrimaryKey("pk_character_skill", x => x.id);
                    table.ForeignKey(
                        name: "fk_character_skill_character_character_id",
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
                name: "ix_character_quick_slot_character_id",
                table: "character_quick_slot",
                column: "character_id");

            migrationBuilder.CreateIndex(
                name: "ix_character_skill_character_id",
                table: "character_skill",
                column: "character_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_quick_slot");

            migrationBuilder.DropTable(
                name: "character_skill");

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
