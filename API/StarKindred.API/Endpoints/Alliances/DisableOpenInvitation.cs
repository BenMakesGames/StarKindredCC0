using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Alliances;

[ApiController]
public sealed class DisableOpenInvitation
{
    [HttpPost("alliances/disableOpenInvitation")]
    public async Task<ApiResponse> _(
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var allianceMembership = await db.UserAlliances
            .Include(ua => ua.Alliance!)
            .Include(ua => ua.AllianceRank)
            .Where(a => a.UserId == session.UserId)
            .FirstOrDefaultAsync(cToken)
            ?? throw new UnprocessableEntity("You're not in an Alliance.");

        var rights = AllianceRightsHelper.GetRights(allianceMembership.Alliance!.LeaderId, allianceMembership);

        if(!rights.Contains(AllianceRight.Recruit))
            throw new AccessDeniedException("You don't have permission to Recruit.");

        var seekingMembers = db.AllianceRecruitStatuses.FirstOrDefault(s => s.AllianceId == allianceMembership.AllianceId);

        if (seekingMembers == null)
            throw new NotFoundException("This Alliance does not have Open Invitations active.");

        seekingMembers.OpenInvitationActive = false;

        await db.SaveChangesAsync(cToken);

        return new();
    }
}