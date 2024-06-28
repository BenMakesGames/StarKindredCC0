using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class RelationshipTableUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RelationshipVassal_Relationship_RelationshipsId",
                table: "RelationshipVassal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Relationship",
                table: "Relationship");

            migrationBuilder.RenameTable(
                name: "Relationship",
                newName: "Relationships");

            migrationBuilder.RenameColumn(
                name: "Progress",
                table: "Relationships",
                newName: "Minutes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Relationships",
                table: "Relationships",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RelationshipVassal_Relationships_RelationshipsId",
                table: "RelationshipVassal",
                column: "RelationshipsId",
                principalTable: "Relationships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RelationshipVassal_Relationships_RelationshipsId",
                table: "RelationshipVassal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Relationships",
                table: "Relationships");

            migrationBuilder.RenameTable(
                name: "Relationships",
                newName: "Relationship");

            migrationBuilder.RenameColumn(
                name: "Minutes",
                table: "Relationship",
                newName: "Progress");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Relationship",
                table: "Relationship",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RelationshipVassal_Relationship_RelationshipsId",
                table: "RelationshipVassal",
                column: "RelationshipsId",
                principalTable: "Relationship",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
