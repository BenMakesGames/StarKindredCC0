using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class UpdatingGiantContributions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GiantContributions_Vassals_VassalId",
                table: "GiantContributions");

            migrationBuilder.DropIndex(
                name: "IX_GiantContributions_VassalId",
                table: "GiantContributions");

            migrationBuilder.DropColumn(
                name: "VassalId",
                table: "GiantContributions");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AttackDate",
                table: "GiantContributions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttackDate",
                table: "GiantContributions");

            migrationBuilder.AddColumn<Guid>(
                name: "VassalId",
                table: "GiantContributions",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_GiantContributions_VassalId",
                table: "GiantContributions",
                column: "VassalId");

            migrationBuilder.AddForeignKey(
                name: "FK_GiantContributions_Vassals_VassalId",
                table: "GiantContributions",
                column: "VassalId",
                principalTable: "Vassals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
