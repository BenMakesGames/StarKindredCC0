using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Alliances;

[ApiController]
public sealed class KickMember
{
    [HttpPost("alliances/members/{memberId:guid}/kick")]
    public async Task<ApiResponse> _(
        Guid memberId,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        if(memberId == session.UserId)
            throw new SillyException("Stop kicking yourself. Stop kicking yourself.");
        
        var alliance = await db.UserAlliances
            .Include(ua => ua.Alliance!)
                .ThenInclude(a => a.Members!)
                    .ThenInclude(m => m.User)
            .Include(ua => ua.Alliance!)
                .ThenInclude(a => a.Members!)
                    .ThenInclude(m => m.AllianceRank)
            .AsSingleQuery() // TODO: not profiled
            .Where(a => a.UserId == session.UserId)
            .Select(a => a.Alliance)
            .FirstOrDefaultAsync(cToken)
            ?? throw new UnprocessableEntity("You're not in an Alliance.");

        var myMembership = alliance.Members!.First(m => m.UserId == session.UserId);
        
        var rights = AllianceRightsHelper.GetRights(alliance.LeaderId, myMembership);
        var myRank = AllianceRightsHelper.GetRank(alliance.LeaderId, myMembership);

        if(!rights.Contains(AllianceRight.Kick))
            throw new AccessDeniedException("You don't have permission to kick alliance members.");

        if(memberId == alliance.LeaderId)
            throw new AccessDeniedException("The leader of the Alliance cannot be kicked! (Mutiny, is it?!)");
        
        var memberToKick = alliance.Members!
            .FirstOrDefault(m => m.UserId == memberId)
            ?? throw new NotFoundException("That member does not exist.");

        if(memberToKick.AllianceRank != null && (memberToKick.AllianceRank?.Rank ?? 0) >= myRank)
            throw new AccessDeniedException("You can only kick members with a lower-level Rank than your own.");

        db.UserAlliances.Remove(memberToKick);

        db.AllianceLogs.Add(new()
        {
            ActivityType = AllianceLogActivityType.MemberKicked,
            AllianceId = alliance.Id,
            Message = $"{memberToKick.User!.Name} was kicked from the Alliance; they were kicked by {session.Name}.",
        });
        
        await db.SaveChangesAsync(cToken);
        
        return new();
    }
}