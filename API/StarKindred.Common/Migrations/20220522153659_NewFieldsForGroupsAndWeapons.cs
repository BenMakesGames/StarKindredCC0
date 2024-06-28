using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class NewFieldsForGroupsAndWeapons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Durability",
                table: "Weapons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxDurability",
                table: "Weapons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "Groups",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "IsRecruiting",
                table: "Groups",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "LeaderId",
                table: "Groups",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

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

            migrationBuilder.Sql("UPDATE Weapons SET Durability=15,MaxDurability=15");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Durability",
                table: "Weapons");

            migrationBuilder.DropColumn(
                name: "MaxDurability",
                table: "Weapons");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "IsRecruiting",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "LeaderId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "MaxLevel",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "MinLevel",
                table: "Groups");
        }
    }
}
