using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class RemovingGuildIsRecruitingAndAddingUserIsLookingForGuild : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRecruiting",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "MaxLevel",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "MinLevel",
                table: "Groups");

            migrationBuilder.AddColumn<bool>(
                name: "IsLookingForGuild",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLookingForGuild",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "IsRecruiting",
                table: "Groups",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxLevel",
                table: "Groups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinLevel",
                table: "Groups",
                type: "int",
                nullable: true);
        }
    }
}
