using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class IndexAllianceColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Alliances_CreatedOn",
                table: "Alliances",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Alliances_Level",
                table: "Alliances",
                column: "Level");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Alliances_CreatedOn",
                table: "Alliances");

            migrationBuilder.DropIndex(
                name: "IX_Alliances_Level",
                table: "Alliances");
        }
    }
}
