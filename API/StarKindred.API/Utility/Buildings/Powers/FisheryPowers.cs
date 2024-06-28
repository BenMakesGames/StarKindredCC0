using BenMakesGames.RandomHelpers;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;

namespace StarKindred.API.Utility.Buildings.Powers;

public static class FisheryPowers
{
    public static async Task<(string, ResourceQuantity?, bool)> DoBoatRide(
        Db db,
        Random rng,
        ICurrentUser.CurrentSessionDto session, CancellationToken cToken)
    {
        // put a friendship mission on the map
        var timedMissionsCount = await db.TimedMissions.CountAsync(t => t.UserId == session.UserId, cToken);

        var maxRumors = await MissionMath.MaxRumors(db, session.UserId, cToken);

        if(timedMissionsCount >= maxRumors)
            throw new UnprocessableEntity($"You may only track {maxRumors} Rumors at a time.");

        var timedMission = TimedMissionHelper.CreateBoatDate(session.UserId);

        timedMission.Location = rng.Next(await TimedMissionHelper.GetAvailableSeaLocations(db, session.UserId, cToken));

        db.TimedMissions.Add(timedMission);

        return ("The fisherfolk of the city have arranged a short boat ride for your Vassals!", null, false);
    }

    public static async Task<(string, ResourceQuantity?, bool)> DoFishMarket(
        Db db,
        ICurrentUser.CurrentSessionDto session,
        List<TechnologyType> researchedTechnologies,
        CancellationToken cToken
    )
    {
        var applicableTechnologies = new[]
        {
            TechnologyType.CupronickleII,
            TechnologyType.CupronickleIII
        };
        
        var boostingTechs = researchedTechnologies.Count(t => applicableTechnologies.Contains(t));

        var gold = new ResourceQuantity(ResourceType.Gold, 200 + boostingTechs * 50);
        
        await ResourceHelper.CollectResources(db, session.UserId, new() { gold }, cToken);

        return ($"{gold.Quantity} Gold is earned by the Fish Market.", gold, false);
    }
}