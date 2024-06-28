using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class AddingVassalTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserVassalTag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Title = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Color = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVassalTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserVassalTag_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserVassalTagVassal",
                columns: table => new
                {
                    TagsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    VassalsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVassalTagVassal", x => new { x.TagsId, x.VassalsId });
                    table.ForeignKey(
                        name: "FK_UserVassalTagVassal_UserVassalTag_TagsId",
                        column: x => x.TagsId,
                        principalTable: "UserVassalTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserVassalTagVassal_Vassals_VassalsId",
                        column: x => x.VassalsId,
                        principalTable: "Vassals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserVassalTag_Title",
                table: "UserVassalTag",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_UserVassalTag_UserId_Title",
                table: "UserVassalTag",
                columns: new[] { "UserId", "Title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserVassalTagVassal_VassalsId",
                table: "UserVassalTagVassal",
                column: "VassalsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserVassalTagVassal");

            migrationBuilder.DropTable(
                name: "UserVassalTag");
        }
    }
}
