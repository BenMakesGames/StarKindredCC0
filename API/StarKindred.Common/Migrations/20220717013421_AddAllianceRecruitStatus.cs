using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class AddAllianceRecruitStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllianceRecruitStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AllianceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InviteCodeActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InviteCode = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InviteCodeGeneratedOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    OpenInvitationActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    OpenInvitationMinLevel = table.Column<int>(type: "int", nullable: false),
                    OpenInvitationMaxLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllianceRecruitStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllianceRecruitStatuses_Alliances_AllianceId",
                        column: x => x.AllianceId,
                        principalTable: "Alliances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AllianceRecruitStatuses_AllianceId",
                table: "AllianceRecruitStatuses",
                column: "AllianceId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllianceRecruitStatuses");
        }
    }
}
