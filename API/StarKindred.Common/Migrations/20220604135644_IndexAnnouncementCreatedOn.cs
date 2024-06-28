using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class IndexAnnouncementCreatedOn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Announcements_CreatedOn",
                table: "Announcements",
                column: "CreatedOn");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Announcements_CreatedOn",
                table: "Announcements");
        }
    }
}
