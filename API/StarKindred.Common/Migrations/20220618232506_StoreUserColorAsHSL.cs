using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class StoreUserColorAsHSL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "Color_Hue",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Color_Luminosity",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Color_Saturation",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
            
            migrationBuilder.Sql("UPDATE Users SET Color_Hue=210, Color_Saturation=75, Color_Luminosity=53;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color_Hue",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Color_Luminosity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Color_Saturation",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Users",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
