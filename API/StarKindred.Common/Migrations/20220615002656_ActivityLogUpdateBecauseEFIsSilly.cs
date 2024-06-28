using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class ActivityLogUpdateBecauseEFIsSilly : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AllianceLog_Alliances_AllianceId",
                table: "AllianceLog");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalLog_Users_UserId",
                table: "PersonalLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonalLog",
                table: "PersonalLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AllianceLog",
                table: "AllianceLog");

            migrationBuilder.RenameTable(
                name: "PersonalLog",
                newName: "PersonalLogs");

            migrationBuilder.RenameTable(
                name: "AllianceLog",
                newName: "AllianceLogs");

            migrationBuilder.RenameIndex(
                name: "IX_PersonalLog_UserId",
                table: "PersonalLogs",
                newName: "IX_PersonalLogs_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PersonalLog_CreatedOn",
                table: "PersonalLogs",
                newName: "IX_PersonalLogs_CreatedOn");

            migrationBuilder.RenameIndex(
                name: "IX_AllianceLog_CreatedOn",
                table: "AllianceLogs",
                newName: "IX_AllianceLogs_CreatedOn");

            migrationBuilder.RenameIndex(
                name: "IX_AllianceLog_AllianceId",
                table: "AllianceLogs",
                newName: "IX_AllianceLogs_AllianceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonalLogs",
                table: "PersonalLogs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AllianceLogs",
                table: "AllianceLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AllianceLogs_Alliances_AllianceId",
                table: "AllianceLogs",
                column: "AllianceId",
                principalTable: "Alliances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalLogs_Users_UserId",
                table: "PersonalLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AllianceLogs_Alliances_AllianceId",
                table: "AllianceLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalLogs_Users_UserId",
                table: "PersonalLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonalLogs",
                table: "PersonalLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AllianceLogs",
                table: "AllianceLogs");

            migrationBuilder.RenameTable(
                name: "PersonalLogs",
                newName: "PersonalLog");

            migrationBuilder.RenameTable(
                name: "AllianceLogs",
                newName: "AllianceLog");

            migrationBuilder.RenameIndex(
                name: "IX_PersonalLogs_UserId",
                table: "PersonalLog",
                newName: "IX_PersonalLog_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PersonalLogs_CreatedOn",
                table: "PersonalLog",
                newName: "IX_PersonalLog_CreatedOn");

            migrationBuilder.RenameIndex(
                name: "IX_AllianceLogs_CreatedOn",
                table: "AllianceLog",
                newName: "IX_AllianceLog_CreatedOn");

            migrationBuilder.RenameIndex(
                name: "IX_AllianceLogs_AllianceId",
                table: "AllianceLog",
                newName: "IX_AllianceLog_AllianceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonalLog",
                table: "PersonalLog",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AllianceLog",
                table: "AllianceLog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AllianceLog_Alliances_AllianceId",
                table: "AllianceLog",
                column: "AllianceId",
                principalTable: "Alliances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalLog_Users_UserId",
                table: "PersonalLog",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
