using BenMakesGames.RandomHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.Common.Services;

namespace StarKindred.API.Utility.Buildings.Powers;

[ApiController]
public static class HunterPowers
{
    public static async Task<(string, ResourceQuantity?, bool)> DoMonsterHunter(
        Db db,
        ICurrentUser.CurrentSessionDto session,
        Random rng,
        CancellationToken cToken
    )
    {
        // put a random monster on the map
        var timedMissionsCount = await db.TimedMissions.CountAsync(t => t.UserId == session.UserId, cToken);

        var maxRumors = await MissionMath.MaxRumors(db, session.UserId, cToken);

        if(timedMissionsCount >= maxRumors)
            throw new UnprocessableEntity($"You may only track {maxRumors} Rumors at a time.");

        var level = await TimedMissionHelper.GetMissionLevel(db, rng, session.UserId, cToken);

        var timedMission = TimedMissionHelper.CreateWanderingMonster(rng, session.UserId, level);

        timedMission.Location = rng.Next(await TimedMissionHelper.GetAvailableLandLocations(db, session.UserId, cToken));

        db.TimedMissions.Add(timedMission);

        return (timedMission.Description, null, false);
    }
}