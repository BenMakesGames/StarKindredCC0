using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Vassals;

[ApiController]
public sealed class LevelUp
{
    [HttpPost("/vassals/{vassalId:guid}/levelUp")]
    public async Task<ApiResponse> _(
        CancellationToken cToken,
        Guid vassalId,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        var user = await db.Users.FirstAsync(u => u.Id == session.UserId, cToken);

        var vassal = await db.Vassals
            .Include(v => v.StatusEffects)
            .FirstOrDefaultAsync(v => v.Id == vassalId && v.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("There is no such Vassal.");

        if(vassal.IsOnAMission)
            throw new UnprocessableEntity($"{vassal.Name} is currently busy.");
        
        if(vassal.Level >= VassalMath.MaxLevel(vassal))
            throw new UnprocessableEntity($"{vassal.Name} is at max level!");

        var technologies = await db.UserTechnologies
            .Where(t => t.UserId == session.UserId && (t.Technology == TechnologyType.FreeTrade || t.Technology == TechnologyType.Divination))
            .Select(t => t.Technology)
            .ToListAsync(cToken);

        var hasFreeTrade = technologies.Contains(TechnologyType.FreeTrade);
        var hasDivination = technologies.Contains(TechnologyType.Divination);

        var levelUpCost = VassalMath.ResourcesToLevelUp(vassal, hasFreeTrade);

        var resources = await db.Resources
            .Where(r => r.UserId == session.UserId)
            .ToListAsync(cToken);

        ResourceHelper.PayOrThrow(resources, levelUpCost);

        if (hasDivination && !vassal.StatusEffects!.Any(se => se.Type == StatusEffectType.Focused))
            StatusEffectsHelper.AddStatusEffect(vassal, StatusEffectType.Focused, 1);

        PersonalLogHelper.Create(db, session.UserId, $"You leveled-up **{vassal.Name}** from level {vassal.Level} to {vassal.Level + 1}.", new[]
        {
            PersonalLogActivityType.Vassal,
            PersonalLogActivityType.LeveledUpVassal
        });

        vassal.Level++;

        await db.SaveChangesAsync(cToken);

        await UserHelper.ComputeLevel(db, user, cToken);

        await db.SaveChangesAsync(cToken);

        return new ApiResponse();
    }
}