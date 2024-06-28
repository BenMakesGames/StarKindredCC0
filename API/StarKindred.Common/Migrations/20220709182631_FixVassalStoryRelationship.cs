using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class FixVassalStoryRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vassals_AdventureSteps_AdventureStepInProgressId",
                table: "Vassals");

            migrationBuilder.DropIndex(
                name: "IX_Vassals_AdventureStepInProgressId",
                table: "Vassals");

            migrationBuilder.DropColumn(
                name: "AdventureStepInProgressId",
                table: "Vassals");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AdventureStepInProgressId",
                table: "Vassals",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Vassals_AdventureStepInProgressId",
                table: "Vassals",
                column: "AdventureStepInProgressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vassals_AdventureSteps_AdventureStepInProgressId",
                table: "Vassals",
                column: "AdventureStepInProgressId",
                principalTable: "AdventureSteps",
                principalColumn: "Id");
        }
    }
}
