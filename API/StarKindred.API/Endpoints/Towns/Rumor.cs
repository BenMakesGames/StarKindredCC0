using BenMakesGames.RandomHelpers;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Towns;

[ApiController]
public sealed class Rumor
{
    [HttpPost("/towns/rumor")]
    public async Task<ApiResponse<Response>> _(
        [FromServices] Db db, CancellationToken cToken,
        [FromServices] ICurrentUser currentUser, [FromServices] Random rng
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        var town = await db.Towns.FirstAsync(t => t.UserId == session.UserId, cToken);

        var timedMissionsCount = await db.TimedMissions.CountAsync(t => t.UserId == session.UserId, cToken);

        var maxRumors = await MissionMath.MaxRumors(db, session.UserId, cToken);

        if(timedMissionsCount >= maxRumors)
            throw new UnprocessableEntity($"You may only track {maxRumors} Rumors at a time.");

        var now = DateTimeOffset.UtcNow;

        if(town.NextRumor > now)
            throw new NotFoundException("There are no rumors, currently.");

        town.NextRumor = now.AddDays(1).Date;

        var type = GetMissionType(rng);

        var level = await TimedMissionHelper.GetMissionLevel(db, rng, session.UserId, cToken);
        var levelBonus = 0;

        if (type == MissionType.Settlers)
        {
            var tourismIIAndIIICount = await db.UserTechnologies
                .CountAsync(t => t.UserId == session.UserId && (t.Technology == TechnologyType.TourismII || t.Technology == TechnologyType.TourismIII), cToken);

            levelBonus = tourismIIAndIIICount * 10;
        }

        var timedMission = type switch
        {
            MissionType.Settlers => TimedMissionHelper.CreateSettlersMission(rng, session.UserId, level / 2 + levelBonus),
            MissionType.TreasureHunt => TimedMissionHelper.CreateTreasureHunt(rng, session.UserId, level + levelBonus),
            MissionType.WanderingMonster => TimedMissionHelper.CreateWanderingMonster(rng, session.UserId, level + levelBonus),
            _ => throw new Exception($"Unhandled timed mission type: {type}")
        };

        timedMission.Location = rng.Next(await TimedMissionHelper.GetAvailableLandLocations(db, session.UserId, cToken));

        // add new mission
        db.TimedMissions.Add(timedMission);

        await db.SaveChangesAsync(cToken);

        return new(new(timedMission.Description));
    }

    private static MissionType GetMissionType(Random rng) => rng.Next(new[]
    {
        MissionType.WanderingMonster, MissionType.WanderingMonster, MissionType.WanderingMonster,
        MissionType.TreasureHunt, MissionType.TreasureHunt,
        MissionType.Settlers
    });

    public sealed record Response(string Message);
}