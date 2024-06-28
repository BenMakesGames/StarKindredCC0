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
public sealed class TrackGiant
{
    [HttpPost("/alliances/trackGiant")]
    public async Task<ApiResponse<GiantDto>> _(
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Random rng,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        
        var alliance = await db.UserAlliances
            .Where(ua => ua.UserId == session.UserId)
            .Include(ua => ua.Alliance!)
                .ThenInclude(a => a.Giant)
            .Include(ua => ua.Alliance!)
                .ThenInclude(a => a.Members!)
                    .ThenInclude(m => m.AllianceRank)
            .AsSplitQuery() // TODO: not profiled
            .Select(ua => ua.Alliance)
            .FirstOrDefaultAsync(cToken)
            ?? throw new NotFoundException("You are not in an Alliance.");

        var myRank = alliance.Members!.First(m => m.UserId == session.UserId);

        var rights = AllianceRightsHelper.GetRights(alliance.LeaderId, myRank);
        
        if(!rights.Contains(AllianceRight.TrackGiants))
            throw new AccessDeniedException("You don't have permission to track Giants.");

        var messages = new List<string>();
            
        if (alliance.Giant != null)
        {
            if (alliance.Giant.ExpiresOn > DateTime.UtcNow)
                throw new NotFoundException("The Giant can still be attacked.");

            var allianceMemberIds = alliance.Members!.Select(m => m.UserId).ToList();

            var contributions = await db.GiantContributions
                .Where(c => allianceMemberIds.Contains(c.UserId))
                .ToListAsync(cToken);

            if (alliance.Giant.Damage >= alliance.Giant.Health)
            {
                var participantIds = contributions.Select(c => c.UserId).Distinct().ToList();

                var rewards = GiantHelper.GiantRewards(alliance.Giant.Element, alliance.Level);

                foreach (var userId in participantIds)
                {
                    await ResourceHelper.CollectResources(db, userId, rewards.Resources, cToken);
                    
                    foreach(var treasure in rewards.Treasures)
                        await TreasureHelper.CollectTreasure(db, userId, treasure.Type, treasure.Quantity, cToken);
                }

                var oldLevel = alliance.Level;
                
                alliance.Level++;

                // 3, 9, 27, 81, 243, ...
                for(int b = 3;; b *= 3)
                {
                    if(alliance.Giant.Damage >= GiantHelper.GiantHealth(alliance.Level + b))
                        alliance.Level++;
                    else
                        break;
                }

                messages.Add($"The giant was defeated! The Alliance's level has been increased by {alliance.Level - oldLevel}.");
                messages.Add("Everyone who participated received " + rewards.GetDescription() + "!");

                db.AllianceLogs.Add(new()
                {
                    AllianceId = alliance.Id,
                    ActivityType = AllianceLogActivityType.GiantTracked,
                    Message = $"The Giant was defeated! The Alliance's level increased from {oldLevel} to {alliance.Level}. {session.Name} tracked a new Giant.",
                });

                db.AllianceLogs.Add(new()
                {
                    AllianceId = alliance.Id,
                    ActivityType = AllianceLogActivityType.GiantRewards,
                    Message = $"{session.Name} distributed the spoils to everyone who participated in the Giant hunt:\n\n{rewards.GetMarkdownList()}"
                });
            }
            else
            {
                if (alliance.Level > 1)
                {
                    alliance.Level--;
                    messages.Add("The Giant was not defeated. The Alliance's level has been decreased by 1.");

                    db.AllianceLogs.Add(new()
                    {
                        AllianceId = alliance.Id,
                        ActivityType = AllianceLogActivityType.GiantTracked,
                        Message = $"The Giant escaped! The Alliance's level decreased to {alliance.Level}. {session.Name} tracked a new Giant.",
                    });
                }
                else
                {
                    messages.Add("The Giant was not defeated.");

                    db.AllianceLogs.Add(new()
                    {
                        AllianceId = alliance.Id,
                        ActivityType = AllianceLogActivityType.GiantTracked,
                        Message = $"The Giant escaped! {session.Name} tracked a new Giant.",
                    });
                }
            }

            var lastGiantExpiredOn = alliance.Giant.ExpiresOn;

            db.GiantContributions.RemoveRange(contributions);
            db.Giants.Remove(alliance.Giant);

            alliance.Giant = GiantHelper.CreateGiant(rng, alliance.Level, lastGiantExpiredOn);
            alliance.LastActiveOn = DateTimeOffset.UtcNow;

            messages.Add("A new Giant is being tracked.");
        }
        else
        {
            alliance.Giant = GiantHelper.CreateGiant(rng, alliance.Level, null);
            messages.Add("A new Giant is being tracked. It can be attacked from the Mission map.");

            db.AllianceLogs.Add(new()
            {
                AllianceId = alliance.Id,
                ActivityType = AllianceLogActivityType.GiantTracked,
                Message = $"{session.Name} tracked the Alliance's first Giant.",
            });
        }

        await db.SaveChangesAsync(cToken);
        
        return new(new(
            alliance.Giant.StartsOn,
            alliance.Giant.ExpiresOn,
            alliance.Giant.Element,
            alliance.Giant.Health,
            alliance.Giant.Damage
        ))
        {
            Messages = messages.Select(ApiMessage.Info).ToList()
        };
    }

    public sealed record GiantDto(DateTimeOffset StartsOn, DateTimeOffset ExpiresOn, Element Element, int Health, int Damage);
}