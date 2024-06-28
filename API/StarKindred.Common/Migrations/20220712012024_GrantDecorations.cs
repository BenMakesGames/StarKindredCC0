using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class GrantDecorations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO Decorations (Id, UserId, Type, Quantity)
SELECT UUID() AS Id,UserId,'WoodenBridge' AS Type,2 AS Quantity
FROM Towns WHERE Level>=2;

INSERT INTO TownDecorations (Id, TownId, Type, Scale, X, Y)
SELECT UUID() AS Id,Towns.Id AS TownId,'WoodenBridge' AS Type,100 AS Scale,52.1 AS X,22.1 AS Y
FROM Towns WHERE Level>=2;

INSERT INTO TownDecorations (Id, TownId, Type, Scale, X, Y)
SELECT UUID() AS Id,Towns.Id AS TownId,'WoodenBridge' AS Type,130 AS Scale,67.1 AS X,55 AS Y
FROM Towns WHERE Level>=2;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
