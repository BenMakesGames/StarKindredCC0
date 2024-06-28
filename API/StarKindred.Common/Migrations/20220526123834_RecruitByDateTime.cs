using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class RecruitByDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_IsLookingForGuild",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsLookingForGuild",
                table: "Users");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "IsLookingForGuildSince",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsLookingForGuildSince",
                table: "Users",
                column: "IsLookingForGuildSince");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_IsLookingForGuildSince",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsLookingForGuildSince",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "IsLookingForGuild",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsLookingForGuild",
                table: "Users",
                column: "IsLookingForGuild");
        }
    }
}
