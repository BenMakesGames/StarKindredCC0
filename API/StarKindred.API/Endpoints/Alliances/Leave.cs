using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.Alliances;

[ApiController]
public sealed class Leave
{
    [HttpPost("/alliances/leave")]
    public async Task<ApiResponse> _(
        CancellationToken cToken,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var membership = await db.UserAlliances
            .Include(ua => ua.Alliance!)
                .ThenInclude(a => a.Members!)
                    .ThenInclude(m => m.User)
            .AsSingleQuery() // TODO: not profiled
            .FirstOrDefaultAsync(ua => ua.UserId == session.UserId, cToken)
            ?? throw new UnprocessableEntity("You are not in an Alliance.");

        if(membership.Alliance!.Members!.Count == 1)
            db.Alliances.Remove(membership.Alliance!);
        else
        {
            db.AllianceLogs.Add(new()
            {
                ActivityType = AllianceLogActivityType.MemberLeft,
                AllianceId = membership.AllianceId,
                Message = $"{session.Name} left the Alliance.",
            });

            if(membership.Alliance!.LeaderId == session.UserId)
            {
                var newLeader = membership.Alliance!.Members
                    .Where(m => m.UserId != session.UserId)
                    .OrderBy(m => m.JoinedOn)
                    .First();
                
                membership.Alliance!.LeaderId = newLeader.UserId;

                db.AllianceLogs.Add(new()
                {
                    ActivityType = AllianceLogActivityType.NewLeader,
                    AllianceId = membership.AllianceId,
                    Message = $"{newLeader.User!.Name} is the new Alliance Leader!",
                });
            }
        }
        
        db.UserAlliances.Remove(membership);

        await db.SaveChangesAsync(cToken);

        return new();
    }
}