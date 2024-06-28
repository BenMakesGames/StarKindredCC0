using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class DeletingOldMissionLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MissionReportVassal");

            migrationBuilder.DropTable(
                name: "MissionReports");

            migrationBuilder.Sql("DELETE FROM Missions WHERE IsCompleted=1");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Missions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Missions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "MissionReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CompletedOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    Message = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Outcome = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MissionReports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MissionReportVassal",
                columns: table => new
                {
                    MissionReportsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    VassalsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionReportVassal", x => new { x.MissionReportsId, x.VassalsId });
                    table.ForeignKey(
                        name: "FK_MissionReportVassal_MissionReports_MissionReportsId",
                        column: x => x.MissionReportsId,
                        principalTable: "MissionReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MissionReportVassal_Vassals_VassalsId",
                        column: x => x.VassalsId,
                        principalTable: "Vassals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MissionReports_UserId",
                table: "MissionReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MissionReportVassal_VassalsId",
                table: "MissionReportVassal",
                column: "VassalsId");
        }
    }
}
