using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class MergingKalbiAndMidine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE Vassals SET Species=""Midine"" WHERE Species=""Kalbi""");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
