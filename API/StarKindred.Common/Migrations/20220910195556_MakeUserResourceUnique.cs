using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class MakeUserResourceUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE Resources AS r SET r.Quantity=(
                    SELECT SUM(r2.Quantity) FROM Resources AS r2 WHERE r2.UserId=r.UserId AND r2.Type=r.Type
                );
            ");

            migrationBuilder.Sql(@"
                DELETE FROM Resources WHERE Id IN (
	                SELECT t1.Id FROM Resources t1
	                WHERE t1.Id >
	                (
	                    SELECT MIN(t2.Id) FROM Resources AS t2
		                WHERE t1.UserId = t2.UserId AND t1.Type = t2.Type
	                )
                );
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_UserId_Type",
                table: "Resources",
                columns: new[] { "UserId", "Type" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Resources_UserId_Type",
                table: "Resources");
        }
    }
}