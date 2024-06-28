using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarKindred.Common.Migrations
{
    public partial class AddAllianceRecruitStatuses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO AllianceRecruitStatuses (Id, AllianceId, InviteCodeActive, InviteCode, InviteCodeGeneratedOn, OpenInvitationActive, OpenInvitationMinLevel, OpenInvitationMaxLevel)
SELECT
    UUID() AS Id,
    Id AS AllianceId,
    0 as InviteCodeActive,
    SUBSTR(MD5(RAND()), 1, 7) AS InviteCode,
    UTC_TIMESTAMP AS InviteCodeGeneratedOn,
    0 AS OpenInvitationActive,
    0 AS OpenInvitationMinLevel,
    330 AS OpenInvitationMaxLevel
FROM Alliances
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
