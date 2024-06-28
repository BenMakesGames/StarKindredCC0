using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class AddAllianceGiantReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AllianceId",
                table: "Giants",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Giants_AllianceId",
                table: "Giants",
                column: "AllianceId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Giants_Alliances_AllianceId",
                table: "Giants",
                column: "AllianceId",
                principalTable: "Alliances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Giants_Alliances_AllianceId",
                table: "Giants");

            migrationBuilder.DropIndex(
                name: "IX_Giants_AllianceId",
                table: "Giants");

            migrationBuilder.DropColumn(
                name: "AllianceId",
                table: "Giants");
        }
    }
}
