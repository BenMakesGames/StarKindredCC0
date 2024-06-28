using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class AddAllianceLeaderRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Alliances_LeaderId",
                table: "Alliances",
                column: "LeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alliances_Users_LeaderId",
                table: "Alliances",
                column: "LeaderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alliances_Users_LeaderId",
                table: "Alliances");

            migrationBuilder.DropIndex(
                name: "IX_Alliances_LeaderId",
                table: "Alliances");
        }
    }
}
