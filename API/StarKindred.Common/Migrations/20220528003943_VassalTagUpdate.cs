using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class VassalTagUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserVassalTag_Users_UserId",
                table: "UserVassalTag");

            migrationBuilder.DropForeignKey(
                name: "FK_UserVassalTagVassal_UserVassalTag_TagsId",
                table: "UserVassalTagVassal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserVassalTag",
                table: "UserVassalTag");

            migrationBuilder.RenameTable(
                name: "UserVassalTag",
                newName: "UserVassalTags");

            migrationBuilder.RenameIndex(
                name: "IX_UserVassalTag_UserId_Title",
                table: "UserVassalTags",
                newName: "IX_UserVassalTags_UserId_Title");

            migrationBuilder.RenameIndex(
                name: "IX_UserVassalTag_Title",
                table: "UserVassalTags",
                newName: "IX_UserVassalTags_Title");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserVassalTags",
                table: "UserVassalTags",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserVassalTags_Users_UserId",
                table: "UserVassalTags",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserVassalTagVassal_UserVassalTags_TagsId",
                table: "UserVassalTagVassal",
                column: "TagsId",
                principalTable: "UserVassalTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserVassalTags_Users_UserId",
                table: "UserVassalTags");

            migrationBuilder.DropForeignKey(
                name: "FK_UserVassalTagVassal_UserVassalTags_TagsId",
                table: "UserVassalTagVassal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserVassalTags",
                table: "UserVassalTags");

            migrationBuilder.RenameTable(
                name: "UserVassalTags",
                newName: "UserVassalTag");

            migrationBuilder.RenameIndex(
                name: "IX_UserVassalTags_UserId_Title",
                table: "UserVassalTag",
                newName: "IX_UserVassalTag_UserId_Title");

            migrationBuilder.RenameIndex(
                name: "IX_UserVassalTags_Title",
                table: "UserVassalTag",
                newName: "IX_UserVassalTag_Title");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserVassalTag",
                table: "UserVassalTag",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserVassalTag_Users_UserId",
                table: "UserVassalTag",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserVassalTagVassal_UserVassalTag_TagsId",
                table: "UserVassalTagVassal",
                column: "TagsId",
                principalTable: "UserVassalTag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
