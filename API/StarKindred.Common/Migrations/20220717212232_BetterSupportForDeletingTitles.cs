using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class BetterSupportForDeletingTitles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAlliances_AllianceRanks_AllianceRankId",
                table: "UserAlliances");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAlliances_AllianceRanks_AllianceRankId",
                table: "UserAlliances",
                column: "AllianceRankId",
                principalTable: "AllianceRanks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAlliances_AllianceRanks_AllianceRankId",
                table: "UserAlliances");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAlliances_AllianceRanks_AllianceRankId",
                table: "UserAlliances",
                column: "AllianceRankId",
                principalTable: "AllianceRanks",
                principalColumn: "Id");
        }
    }
}
