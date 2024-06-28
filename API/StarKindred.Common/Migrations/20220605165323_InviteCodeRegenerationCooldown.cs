﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class InviteCodeRegenerationCooldown : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CodeGeneratedOn",
                table: "UsersSeekingAlliances",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeGeneratedOn",
                table: "UsersSeekingAlliances");
        }
    }
}