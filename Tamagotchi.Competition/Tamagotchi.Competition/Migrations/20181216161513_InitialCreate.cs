using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Tamagotchi.Competition.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EVENTS",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ACTION_CODE = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
                    ROOM_CODE = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
                    DESEASE_CODE = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    START = table.Column<TimeSpan>(type: "time", nullable: false),
                    FINISH = table.Column<TimeSpan>(type: "time", nullable: false),
                    VALUE = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVENTS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SCORE",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    USER_ID = table.Column<long>(nullable: false),
                    VALUE = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCORE", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EVENTS");

            migrationBuilder.DropTable(
                name: "SCORE");
        }
    }
}
