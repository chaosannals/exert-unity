using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace NetTankServer.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "nt_player",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    account = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    nickname = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    password = table.Column<string>(type: "CHAR(64)", nullable: true),
                    create_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    create_by = table.Column<long>(type: "bigint", nullable: true),
                    update_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    update_by = table.Column<long>(type: "bigint", nullable: true),
                    last_login_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nt_player", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "nt_player_tank",
                columns: table => new
                {
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    TankId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nt_player_tank", x => new { x.PlayerId, x.TankId });
                });

            migrationBuilder.CreateTable(
                name: "nt_tank",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(767)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nt_tank", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ACCOUNT_UNIQUE",
                table: "nt_player",
                column: "account",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "NAME_UNIQUE",
                table: "nt_tank",
                column: "name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nt_player");

            migrationBuilder.DropTable(
                name: "nt_player_tank");

            migrationBuilder.DropTable(
                name: "nt_tank");
        }
    }
}
