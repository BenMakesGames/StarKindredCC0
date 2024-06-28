using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class RenameOldBloodToIchor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE Treasures SET Type=""Ichor"" WHERE Type=""OldBlood""");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
