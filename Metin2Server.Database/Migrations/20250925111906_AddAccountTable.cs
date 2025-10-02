using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Metin2Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "account");

            migrationBuilder.CreateTable(
                name: "account",
                schema: "account",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    last_login = table.Column<DateTime>(type: "timestamp", nullable: true),
                    private_code = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_account", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_account_email",
                schema: "account",
                table: "account",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_account_login",
                schema: "account",
                table: "account",
                column: "login",
                unique: true);
            
            migrationBuilder.Sql("GRANT USAGE ON SCHEMA account TO metin_app_user;");
            migrationBuilder.Sql("GRANT INSERT, SELECT, UPDATE ON account.account TO metin_app_user;");
            migrationBuilder.Sql("GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA aquarium TO metin_app_user;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account",
                schema: "account");
        }
    }
}
