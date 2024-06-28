using BenMakesGames.RandomHelpers;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Treasures;

[ApiController]
public sealed class UseTreasureMap
{
    [HttpPost("treasures/use/treasureMap")]
    public async Task<ApiResponse> _(
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Random rng,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        
        var weaponChests = await db.Treasures
            .Where(t => t.Type == TreasureType.TreasureMap && t.UserId == session.UserId && t.Quantity > 0)
            .ToListAsync(cToken)
            ?? throw new NotFoundException("You don't have any Treasure Maps.");

        TreasureHelper.UseOrThrow(weaponChests, TreasureType.TreasureMap);

        var timedMissionsCount = await db.TimedMissions.CountAsync(t => t.UserId == session.UserId, cToken);

        var maxRumors = await MissionMath.MaxRumors(db, session.UserId, cToken);

        if(timedMissionsCount >= maxRumors)
            throw new UnprocessableEntity($"You may only track {maxRumors} Rumors at a time.");

        var level = await TimedMissionHelper.GetMissionLevel(db, rng, session.UserId, cToken);

        var timedMission = TimedMissionHelper.CreateTreasureHunt(rng, session.UserId, level);

        timedMission.Location = rng.Next(await TimedMissionHelper.GetAvailableLandLocations(db, session.UserId, cToken));

        // add new mission
        db.TimedMissions.Add(timedMission);

        await db.SaveChangesAsync(cToken);

        return new()
        {
            Messages = new()
            {
                ApiMessage.Info("A new Treasure-hunt has been added to the map.")
            }
        };
    }
}