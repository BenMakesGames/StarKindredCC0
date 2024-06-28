using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class DropAllianceSeekingMembersTable1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AllianceSeekingMembers_Alliances_AllianceId",
                table: "AllianceSeekingMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AllianceSeekingMembers",
                table: "AllianceSeekingMembers");

            migrationBuilder.DropIndex(
                name: "IX_AllianceSeekingMembers_AllianceId",
                table: "AllianceSeekingMembers");

            migrationBuilder.DropColumn(
                name: "AllianceId",
                table: "AllianceSeekingMembers");

            migrationBuilder.DropColumn(
                name: "InviteCode",
                table: "AllianceSeekingMembers");

            migrationBuilder.DropColumn(
                name: "InvitesRemaining",
                table: "AllianceSeekingMembers");

            migrationBuilder.DropColumn(
                name: "LevelRange_Max",
                table: "AllianceSeekingMembers");

            migrationBuilder.DropColumn(
                name: "LevelRange_Min",
                table: "AllianceSeekingMembers");

            migrationBuilder.RenameTable(
                name: "AllianceSeekingMembers",
                newName: "AlliancesSeekingMembers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AlliancesSeekingMembers",
                table: "AlliancesSeekingMembers",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AlliancesSeekingMembers",
                table: "AlliancesSeekingMembers");

            migrationBuilder.RenameTable(
                name: "AlliancesSeekingMembers",
                newName: "AllianceSeekingMembers");

            migrationBuilder.AddColumn<Guid>(
                name: "AllianceId",
                table: "AllianceSeekingMembers",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "InviteCode",
                table: "AllianceSeekingMembers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "InvitesRemaining",
                table: "AllianceSeekingMembers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LevelRange_Max",
                table: "AllianceSeekingMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LevelRange_Min",
                table: "AllianceSeekingMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AllianceSeekingMembers",
                table: "AllianceSeekingMembers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AllianceSeekingMembers_AllianceId",
                table: "AllianceSeekingMembers",
                column: "AllianceId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AllianceSeekingMembers_Alliances_AllianceId",
                table: "AllianceSeekingMembers",
                column: "AllianceId",
                principalTable: "Alliances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
