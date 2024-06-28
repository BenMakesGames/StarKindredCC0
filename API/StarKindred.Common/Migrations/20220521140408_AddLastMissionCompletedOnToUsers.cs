using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class AddLastMissionCompletedOnToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastMissionCompletedOn",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_LastMissionCompletedOn",
                table: "Users",
                column: "LastMissionCompletedOn");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_LastMissionCompletedOn",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastMissionCompletedOn",
                table: "Users");
        }
    }
}
