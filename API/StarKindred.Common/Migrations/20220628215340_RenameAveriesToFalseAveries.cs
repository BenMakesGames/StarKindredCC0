using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class RenameAveriesToFalseAveries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE TownDecorations SET Type = 'FalseAveries' WHERE Type = 'Averies'");
            migrationBuilder.Sql(@"UPDATE Decorations SET Type = 'FalseAveries' WHERE Type = 'Averies'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
