using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class AddingAllianceInviteCodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_IsLookingForGuildSince",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsLookingForGuildSince",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "UsersSeekingAlliances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Code = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PublicSince = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersSeekingAlliances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersSeekingAlliances_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UsersSeekingAlliances_Code",
                table: "UsersSeekingAlliances",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersSeekingAlliances_PublicSince",
                table: "UsersSeekingAlliances",
                column: "PublicSince");

            migrationBuilder.CreateIndex(
                name: "IX_UsersSeekingAlliances_UserId",
                table: "UsersSeekingAlliances",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersSeekingAlliances");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "IsLookingForGuildSince",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsLookingForGuildSince",
                table: "Users",
                column: "IsLookingForGuildSince");
        }
    }
}
