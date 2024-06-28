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
public sealed class ChangeMemberTitle
{
    [HttpPost("alliances/members/{memberId:guid}/changeTitle")]
    public async Task<ApiResponse> _(
        Guid memberId,
        Request request,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        if (memberId == session.UserId)
            throw new AccessDeniedException("You can't change your own title, sneaky-pants.");
        
        var alliance = await db.UserAlliances
            .Include(ua => ua.Alliance!)
                .ThenInclude(a => a.Members!)
                    .ThenInclude(m => m.User)
            .Include(ua => ua.Alliance!)
                .ThenInclude(a => a.Members!)
                    .ThenInclude(m => m.AllianceRank)
            .Include(ua => ua.Alliance!)
                .ThenInclude(a => a.AllianceRanks)
            .AsSplitQuery() // TODO: not profiled
            .Where(a => a.UserId == session.UserId)
            .Select(a => a.Alliance)
            .FirstOrDefaultAsync(cToken)
            ?? throw new UnprocessableEntity("You're not in an Alliance.");

        var myMembership = alliance.Members!.First(m => m.UserId == session.UserId);
        
        var rights = AllianceRightsHelper.GetRights(alliance.LeaderId, myMembership);
        var myRank = AllianceRightsHelper.GetRank(alliance.LeaderId, myMembership);

        if(!rights.Contains(AllianceRight.PromoteDemote))
            throw new AccessDeniedException("You don't have permission to kick alliance members.");

        if(memberId == alliance.LeaderId)
            throw new AccessDeniedException("The leader of the Alliance cannot have their title changed! (Mutiny, is it?!)");
        
        var memberToChangeTitleOf = alliance.Members!
            .FirstOrDefault(m => m.UserId == memberId)
            ?? throw new NotFoundException("That member does not exist.");

        if(memberToChangeTitleOf.AllianceRank != null && (memberToChangeTitleOf.AllianceRank?.Rank ?? 0) >= myRank)
            throw new AccessDeniedException("You can only change the Title of members with a lower-level Rank than your own.");

        var oldTitle = memberToChangeTitleOf.AllianceRank?.Title ?? "No Title";
        string newTitle;

        if (request.TitleId != null)
        {
            var targetRank = alliance.AllianceRanks!
                .FirstOrDefault(r => r.Id == request.TitleId)
                ?? throw new NotFoundException("That Title does not exist.");

            if(targetRank.Rank >= myRank)
                throw new AccessDeniedException("You cannot promote a member to, or above, your own Rank.");

            newTitle = targetRank.Title;
        }
        else
            newTitle = "No Title";

        memberToChangeTitleOf.AllianceRankId = request.TitleId;

        db.AllianceLogs.Add(new()
        {
            ActivityType = AllianceLogActivityType.MemberTitleChanged,
            AllianceId = alliance.Id,
            Message = $"{memberToChangeTitleOf.User!.Name}'s Title was changed from `{oldTitle}` to `{newTitle}` by {session.Name}.",
        });
        
        await db.SaveChangesAsync(cToken);
        
        return new();
    }

    public sealed record Request(Guid? TitleId);
}