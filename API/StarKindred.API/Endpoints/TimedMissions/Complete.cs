using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;
using StarKindred.API.Utility.TimedMissions;

namespace StarKindred.API.Endpoints.TimedMissions;

[ApiController]
public sealed class Complete
{
    [HttpPost("/timedMissions/{id:guid}/complete")]
    public async Task<ApiResponse<ResponseDto>> _(
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Random rng,
        CancellationToken cToken,
        Guid id
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        var user = await db.Users.FirstAsync(u => u.Id == session.UserId, cToken);

        var mission = await db.TimedMissions
            .Include(m => m.Vassals!)
                .ThenInclude(v => v.StatusEffects)
            .Include(m => m.Vassals!)
                .ThenInclude(v => v.Weapon)
            .AsSplitQuery() // TODO: not profiled
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("That mission does not exist.");

        if(mission.CompletesOn > DateTimeOffset.UtcNow)
            throw new UnprocessableEntity("That mission has not yet completed.");

        if(mission.Vassals!.Count == 0)
            throw new Exception($"Timed mission {mission.Id} has no Vassals. This should never be. Something is wrong.");

        var result = mission.Type switch
        {
            MissionType.Settlers => await Settlers.Do(db, rng, user, mission.Species!.Value, mission.Level / 2, mission.Vassals!, cToken),
            MissionType.WanderingMonster => await WanderingMonster.Do(db, rng, mission.Level, mission.Element!.Value, mission.Treasure, mission.Weapon, mission.Vassals!, cToken),
            MissionType.TreasureHunt => await TreasureHunt.Do(db, rng, mission.Level, mission.Treasure, mission.Weapon, mission.Vassals!, cToken),
            MissionType.BoatDate => await BoatDate.Do(db, rng, mission.Vassals!, cToken),
            _ => throw new Exception("Unsupported mission type.")
        };

        var durationInMinutes = (int)(mission.CompletesOn!.Value - mission.CreatedOn).TotalMinutes;

        if (MissionProgressesRelationship(mission.Type))
        {
            var decoration = await RelationshipHelper.AdvanceRelationshipsAndMaybeGetLoot(db, rng, mission.Vassals!, durationInMinutes, cToken);

            if (decoration != null)
            {
                var upgraded = await TownHelpers.MakeDecorable(db, session.UserId, cToken);
                var message = result.Message + $"\n\nDuring some downtime, {decoration.Vassal.Name} made {decoration.Decoration.Type.ToNameWithArticle()}.";

                if (decoration.Quintessence > 0)
                    message += $"\n\n(Artistic Visions provided {decoration.Quintessence}!)";

                if (upgraded)
                    message += "\n\n(You can now place Decorations in your Town!)";

                result = result with { Message = message };
            }
        }

        PersonalLogHelper.Create(db, session.UserId, result.Message, new[]
        {
            PersonalLogActivityType.CompleteMission,
            PersonalLogHelper.TagFromMissionType(mission.Type),
            PersonalLogHelper.TagFromMissionOutcome(result.Outcome)
        });

        MissionMath.UpdateVassalsAfterMissionCompletion(mission.Vassals!, mission.Type, result.Outcome, mission.Weapon.HasValue);

        if(result.Complete)
            db.TimedMissions.Remove(mission);

        user.LastMissionCompletedOn = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(cToken);

        return new(result);
    }

    private static bool MissionProgressesRelationship(MissionType type) => type switch
    {
        MissionType.BoatDate => false,
        _ => true,
    };

    public sealed record ResponseDto(MissionOutcome Outcome, bool Complete, string Message, List<MissionReward> Rewards);
}