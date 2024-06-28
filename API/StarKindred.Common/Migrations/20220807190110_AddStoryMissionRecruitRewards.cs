using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class AddStoryMissionRecruitRewards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RecruitId",
                table: "AdventureSteps",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "VassalTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Portrait = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Element = table.Column<int>(type: "int", nullable: true),
                    Species = table.Column<int>(type: "int", nullable: true),
                    Sign = table.Column<int>(type: "int", nullable: true),
                    Nature = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VassalTemplate", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AdventureSteps_RecruitId",
                table: "AdventureSteps",
                column: "RecruitId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdventureSteps_VassalTemplate_RecruitId",
                table: "AdventureSteps",
                column: "RecruitId",
                principalTable: "VassalTemplate",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdventureSteps_VassalTemplate_RecruitId",
                table: "AdventureSteps");

            migrationBuilder.DropTable(
                name: "VassalTemplate");

            migrationBuilder.DropIndex(
                name: "IX_AdventureSteps_RecruitId",
                table: "AdventureSteps");

            migrationBuilder.DropColumn(
                name: "RecruitId",
                table: "AdventureSteps");
        }
    }
}
