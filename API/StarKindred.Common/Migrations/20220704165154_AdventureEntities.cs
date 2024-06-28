using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class AdventureEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AdventureStepInProgressId",
                table: "Vassals",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UserAdventureStepInProgressId",
                table: "Vassals",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "Adventures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Title = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Summary = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReleaseNumber = table.Column<int>(type: "int", nullable: false),
                    ReleaseYear = table.Column<int>(type: "int", nullable: false),
                    ReleaseMonth = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adventures", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AdventureSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AdventureId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Title = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Step = table.Column<int>(type: "int", nullable: false),
                    PreviousStep = table.Column<int>(type: "int", nullable: true),
                    X = table.Column<float>(type: "float", nullable: false),
                    Y = table.Column<float>(type: "float", nullable: false),
                    MinVassals = table.Column<int>(type: "int", nullable: false),
                    MaxVassals = table.Column<int>(type: "int", nullable: false),
                    RequiredElement = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DurationInMinutes = table.Column<int>(type: "int", nullable: false),
                    Narrative = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdventureSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdventureSteps_Adventures_AdventureId",
                        column: x => x.AdventureId,
                        principalTable: "Adventures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserAdventures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AdventureId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAdventures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAdventures_Adventures_AdventureId",
                        column: x => x.AdventureId,
                        principalTable: "Adventures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAdventures_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserAdventureStepCompleted",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AdventureStepId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CompletedOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAdventureStepCompleted", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAdventureStepCompleted_AdventureSteps_AdventureStepId",
                        column: x => x.AdventureStepId,
                        principalTable: "AdventureSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAdventureStepCompleted_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserAdventureStepInProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AdventureStepId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StartedOn = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAdventureStepInProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAdventureStepInProgress_AdventureSteps_AdventureStepId",
                        column: x => x.AdventureStepId,
                        principalTable: "AdventureSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAdventureStepInProgress_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Vassals_AdventureStepInProgressId",
                table: "Vassals",
                column: "AdventureStepInProgressId");

            migrationBuilder.CreateIndex(
                name: "IX_Vassals_UserAdventureStepInProgressId",
                table: "Vassals",
                column: "UserAdventureStepInProgressId");

            migrationBuilder.CreateIndex(
                name: "IX_Adventures_ReleaseNumber",
                table: "Adventures",
                column: "ReleaseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Adventures_ReleaseYear_ReleaseMonth",
                table: "Adventures",
                columns: new[] { "ReleaseYear", "ReleaseMonth" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Adventures_Title",
                table: "Adventures",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdventureSteps_AdventureId_Step",
                table: "AdventureSteps",
                columns: new[] { "AdventureId", "Step" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAdventures_AdventureId",
                table: "UserAdventures",
                column: "AdventureId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAdventures_UserId",
                table: "UserAdventures",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAdventureStepCompleted_AdventureStepId",
                table: "UserAdventureStepCompleted",
                column: "AdventureStepId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAdventureStepCompleted_UserId",
                table: "UserAdventureStepCompleted",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAdventureStepInProgress_AdventureStepId",
                table: "UserAdventureStepInProgress",
                column: "AdventureStepId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAdventureStepInProgress_UserId",
                table: "UserAdventureStepInProgress",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vassals_AdventureSteps_AdventureStepInProgressId",
                table: "Vassals",
                column: "AdventureStepInProgressId",
                principalTable: "AdventureSteps",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vassals_UserAdventureStepInProgress_UserAdventureStepInProgr~",
                table: "Vassals",
                column: "UserAdventureStepInProgressId",
                principalTable: "UserAdventureStepInProgress",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vassals_AdventureSteps_AdventureStepInProgressId",
                table: "Vassals");

            migrationBuilder.DropForeignKey(
                name: "FK_Vassals_UserAdventureStepInProgress_UserAdventureStepInProgr~",
                table: "Vassals");

            migrationBuilder.DropTable(
                name: "UserAdventures");

            migrationBuilder.DropTable(
                name: "UserAdventureStepCompleted");

            migrationBuilder.DropTable(
                name: "UserAdventureStepInProgress");

            migrationBuilder.DropTable(
                name: "AdventureSteps");

            migrationBuilder.DropTable(
                name: "Adventures");

            migrationBuilder.DropIndex(
                name: "IX_Vassals_AdventureStepInProgressId",
                table: "Vassals");

            migrationBuilder.DropIndex(
                name: "IX_Vassals_UserAdventureStepInProgressId",
                table: "Vassals");

            migrationBuilder.DropColumn(
                name: "AdventureStepInProgressId",
                table: "Vassals");

            migrationBuilder.DropColumn(
                name: "UserAdventureStepInProgressId",
                table: "Vassals");
        }
    }
}
