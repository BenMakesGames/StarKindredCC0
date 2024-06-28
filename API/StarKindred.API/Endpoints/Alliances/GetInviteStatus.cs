using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.Alliances;

[ApiController]
public sealed class GetInviteStatus
{
    [HttpGet("alliances/inviteStatus")]
    public async Task<ApiResponse<ResponseDto>> _(
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var recruitStatus = await db.AllianceRecruitStatuses
            .FirstOrDefaultAsync(s => s.Alliance!.Members!.Any(m => m.UserId == session.UserId), cToken)
            ?? throw new AccessDeniedException("You're not a member of any Alliance.");

        return new(new(recruitStatus.InviteCodeActive, recruitStatus.InviteCode, recruitStatus.OpenInvitationActive, recruitStatus.OpenInvitationMinLevel, recruitStatus.OpenInvitationMaxLevel));
    }

    public sealed record ResponseDto(bool UsingInviteCode, string InviteCode, bool UsingOpenInvitation, int OpenInvitationMinLevel, int OpenInvitationMaxLevel);
}