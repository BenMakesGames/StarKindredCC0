using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class ChangingAllianceIndicies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Alliances_Level",
                table: "Alliances");

            migrationBuilder.CreateIndex(
                name: "IX_Alliances_LastActiveOn",
                table: "Alliances",
                column: "LastActiveOn");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Alliances_LastActiveOn",
                table: "Alliances");

            migrationBuilder.CreateIndex(
                name: "IX_Alliances_Level",
                table: "Alliances",
                column: "Level");
        }
    }
}
