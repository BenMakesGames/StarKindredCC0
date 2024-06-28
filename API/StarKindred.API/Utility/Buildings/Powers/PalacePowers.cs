using BenMakesGames.RandomHelpers;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;

namespace StarKindred.API.Utility.Buildings.Powers;

public static class PalacePowers
{
    public static async Task<(string, ResourceQuantity?, bool)> DoKnighting(
        Db db,
        ICurrentUser.CurrentSessionDto session,
        Random rng,
        CancellationToken cToken
    )
    {
        // of your 12 highest-level vassals that are below level 100, one of them (at random) gains a level
        var eligibleVassals = await db.Vassals
            .Where(v => v.UserId == session.UserId && v.Level < 100)
            .OrderByDescending(v => v.Level)
            .Take(12)
            .ToListAsync(cToken);

        if (eligibleVassals.Count == 0)
            throw new UnprocessableEntity("There are no eligible Vassals.");

        var vassal = rng.Next(eligibleVassals);

        vassal.Level++;

        return ($"{vassal.Name} has gained a Level; they are now Level {vassal.Level}.", null, false);
    }

    public static async Task<(string, ResourceQuantity?, bool)> DoAttractSettlers(
        Db db,
        ICurrentUser.CurrentSessionDto session,
        Random rng,
        CancellationToken cToken
    )
    {
        var level = await TimedMissionHelper.GetMissionLevel(db, rng, session.UserId, cToken);

        var tourismIIAndIIICount = await db.UserTechnologies
            .CountAsync(t => t.UserId == session.UserId && (t.Technology == TechnologyType.TourismII || t.Technology == TechnologyType.TourismIII), cToken);

        var levelBonus = tourismIIAndIIICount * 10;

        var settlers = TimedMissionHelper.CreateSettlersMission(rng, session.UserId, level / 2 + levelBonus);

        db.TimedMissions.Add(settlers);

        return (settlers.Description, null, false);
    }
}