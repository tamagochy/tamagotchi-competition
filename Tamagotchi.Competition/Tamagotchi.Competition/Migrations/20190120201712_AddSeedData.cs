using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tamagotchi.Competition.Migrations
{
    public partial class AddSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EVENTS",
                columns: new[] { "ID", "ACTION_CODE", "DESEASE_CODE", "FINISH", "ROOM_CODE", "START", "VALUE" },
                values: new object[,]
                {
                    { 1L, "brush", null, new TimeSpan(0, 23, 0, 0, 0), "playRoom", new TimeSpan(0, 0, 0, 0, 0), 5 },
                    { 16L, "apple", null, new TimeSpan(0, 23, 0, 0, 0), "kitchen", new TimeSpan(0, 0, 0, 0, 0), 5 },
                    { 15L, "oatCookies", null, new TimeSpan(0, 23, 0, 0, 0), "kitchen", new TimeSpan(0, 0, 0, 0, 0), 2 },
                    { 14L, "yogurt", null, new TimeSpan(0, 23, 0, 0, 0), "kitchen", new TimeSpan(0, 0, 0, 0, 0), 15 },
                    { 13L, "porridge", null, new TimeSpan(0, 23, 0, 0, 0), "kitchen", new TimeSpan(0, 0, 0, 0, 0), 20 },
                    { 12L, "soup", null, new TimeSpan(0, 23, 0, 0, 0), "kitchen", new TimeSpan(0, 0, 0, 0, 0), 25 },
                    { 11L, "paste", null, new TimeSpan(0, 23, 0, 0, 0), "kitchen", new TimeSpan(0, 0, 0, 0, 0), 30 },
                    { 10L, "pizza", null, new TimeSpan(0, 23, 0, 0, 0), "kitchen", new TimeSpan(0, 0, 0, 0, 0), 45 },
                    { 9L, "dumplings", null, new TimeSpan(0, 23, 0, 0, 0), "kitchen", new TimeSpan(0, 0, 0, 0, 0), 50 },
                    { 8L, "giveAntihistamine", "allergy", new TimeSpan(0, 23, 0, 0, 0), "hospital", new TimeSpan(0, 0, 0, 0, 0), 100 },
                    { 7L, "stripCandy", "inflammationTricks", new TimeSpan(0, 23, 0, 0, 0), "hospital", new TimeSpan(0, 0, 0, 0, 0), 100 },
                    { 6L, "pshikIngalipt", "angina", new TimeSpan(0, 23, 0, 0, 0), "hospital", new TimeSpan(0, 0, 0, 0, 0), 100 },
                    { 5L, "giveAntiviral", "cold", new TimeSpan(0, 23, 0, 0, 0), "hospital", new TimeSpan(0, 0, 0, 0, 0), 100 },
                    { 4L, "playTicTacToe", null, new TimeSpan(0, 23, 0, 0, 0), "playRoom", new TimeSpan(0, 0, 0, 0, 0), 15 },
                    { 3L, "playBall", null, new TimeSpan(0, 23, 0, 0, 0), "playRoom", new TimeSpan(0, 0, 0, 0, 0), 15 },
                    { 2L, "takePicture", null, new TimeSpan(0, 23, 0, 0, 0), "playRoom", new TimeSpan(0, 0, 0, 0, 0), 10 },
                    { 17L, "water", null, new TimeSpan(0, 23, 0, 0, 0), "kitchen", new TimeSpan(0, 0, 0, 0, 0), 3 },
                    { 18L, "tea", null, new TimeSpan(0, 23, 0, 0, 0), "kitchen", new TimeSpan(0, 0, 0, 0, 0), 3 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EVENTS",
                keyColumn: "ID",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "EVENTS",
                keyColumn: "ID",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "EVENTS",
                keyColumn: "ID",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "EVENTS",
                keyColumn: "ID",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "EVENTS",
                keyColumn: "ID",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "EVENTS",
                keyColumn: "ID",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "EVENTS",
                keyColumn: "ID",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "EVENTS",
                keyColumn: "ID",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "EVENTS",
                keyColumn: "ID",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "EVENTS",
                keyColumn: "ID",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "EVENTS",
                keyColumn: "ID",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "EVENTS",
                keyColumn: "ID",
                keyValue: 12L);

            migrationBuilder.DeleteData(
                table: "EVENTS",
                keyColumn: "ID",
                keyValue: 13L);

            migrationBuilder.DeleteData(
                table: "EVENTS",
                keyColumn: "ID",
                keyValue: 14L);

            migrationBuilder.DeleteData(
                table: "EVENTS",
                keyColumn: "ID",
                keyValue: 15L);

            migrationBuilder.DeleteData(
                table: "EVENTS",
                keyColumn: "ID",
                keyValue: 16L);

            migrationBuilder.DeleteData(
                table: "EVENTS",
                keyColumn: "ID",
                keyValue: 17L);

            migrationBuilder.DeleteData(
                table: "EVENTS",
                keyColumn: "ID",
                keyValue: 18L);
        }
    }
}
