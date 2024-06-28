using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class RecalcUserLevels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE Users SET Users.Level=
IFNULL((SELECT Vassals.Level FROM Vassals WHERE Vassals.UserId=Users.Id ORDER BY Vassals.Level DESC LIMIT 0,1),0)+
IFNULL((SELECT Vassals.Level FROM Vassals WHERE Vassals.UserId=Users.Id ORDER BY Vassals.Level DESC LIMIT 1,1),0)+ 
IFNULL((SELECT Vassals.Level FROM Vassals WHERE Vassals.UserId=Users.Id ORDER BY Vassals.Level DESC LIMIT 2,1),0)
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
