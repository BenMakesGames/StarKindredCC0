using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class UnlockDatesForUnlockableAvatars : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UnlockedOn",
                table: "UserUnlockedAvatars",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_UserUnlockedAvatars_UnlockedOn",
                table: "UserUnlockedAvatars",
                column: "UnlockedOn");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserUnlockedAvatars_UnlockedOn",
                table: "UserUnlockedAvatars");

            migrationBuilder.DropColumn(
                name: "UnlockedOn",
                table: "UserUnlockedAvatars");
        }
    }
}
