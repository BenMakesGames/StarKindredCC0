using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class AddAllianceRanks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AllianceRankId",
                table: "UserAlliances",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "AllianceRanks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AllianceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CanRecruit = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanKick = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanTrackGiants = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllianceRanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllianceRanks_Alliances_AllianceId",
                        column: x => x.AllianceId,
                        principalTable: "Alliances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AllianceSeekingMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AllianceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    InviteCode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LevelRange_Min = table.Column<int>(type: "int", nullable: true),
                    LevelRange_Max = table.Column<int>(type: "int", nullable: true),
                    InvitesRemaining = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllianceSeekingMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllianceSeekingMembers_Alliances_AllianceId",
                        column: x => x.AllianceId,
                        principalTable: "Alliances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserAlliances_AllianceRankId",
                table: "UserAlliances",
                column: "AllianceRankId");

            migrationBuilder.CreateIndex(
                name: "IX_AllianceRanks_AllianceId",
                table: "AllianceRanks",
                column: "AllianceId");

            migrationBuilder.CreateIndex(
                name: "IX_AllianceSeekingMembers_AllianceId",
                table: "AllianceSeekingMembers",
                column: "AllianceId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAlliances_AllianceRanks_AllianceRankId",
                table: "UserAlliances",
                column: "AllianceRankId",
                principalTable: "AllianceRanks",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAlliances_AllianceRanks_AllianceRankId",
                table: "UserAlliances");

            migrationBuilder.DropTable(
                name: "AllianceRanks");

            migrationBuilder.DropTable(
                name: "AllianceSeekingMembers");

            migrationBuilder.DropIndex(
                name: "IX_UserAlliances_AllianceRankId",
                table: "UserAlliances");

            migrationBuilder.DropColumn(
                name: "AllianceRankId",
                table: "UserAlliances");
        }
    }
}
