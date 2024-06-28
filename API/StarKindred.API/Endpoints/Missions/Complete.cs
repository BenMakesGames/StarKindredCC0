using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;
using StarKindred.API.Utility.Missions;

namespace StarKindred.API.Endpoints.Missions;

[ApiController]
public sealed class Complete
{
    [HttpPost("/missions/{id:guid}/complete")]
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

        var mission = await db.Missions
            .Include(m => m.Vassals!)
                .ThenInclude(v => v.StatusEffects)
            .Include(m => m.Vassals!)
                .ThenInclude(v => v.Weapon)
            .AsSplitQuery() // TODO: not profiled
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("That mission does not exist.");

        var durationInMinutes = MissionMath.DurationInMinutes(mission.Type, 0, mission.Vassals!);
        
        if (mission.CreatedOn.AddMinutes(durationInMinutes) > DateTimeOffset.UtcNow)
            throw new UnprocessableEntity("That mission is still in-progress.");

        if(mission.Vassals!.Count == 0)
            throw new Exception($"Basic mission {mission.Id} has no Vassals. This should never be. Something is wrong.");

        var maxVassals = MissionMath.MaxVassals(mission.Type, 0);
        
        var result = mission.Type switch
        {
            MissionType.RecruitTown => await Recruit.Do(db, rng, user, Species.Human, mission.Vassals!, cToken),
            MissionType.Oracle => await Oracle.Do(db, rng, mission.Vassals!.First(), cToken),
            MissionType.HuntLevel0 => await AnimalHunt.Do(db, rng, 0, maxVassals, mission.Vassals!, cToken),
            MissionType.HuntLevel10 => await AnimalHunt.Do(db, rng, 10, maxVassals, mission.Vassals!, cToken),
            MissionType.HuntLevel20 => await AnimalHunt.Do(db, rng, 20, maxVassals, mission.Vassals!, cToken),
            MissionType.HuntLevel50 => await AnimalHunt.Do(db, rng, 50, maxVassals, mission.Vassals!, cToken),
            MissionType.HuntLevel80 => await AnimalHunt.Do(db, rng, 80, maxVassals, mission.Vassals!, cToken),
            MissionType.HuntLevel120 => await AnimalHunt.Do(db, rng, 120, maxVassals, mission.Vassals!, cToken),
            MissionType.HuntLevel200 => await AnimalHunt.Do(db, rng, 200, maxVassals, mission.Vassals!, cToken),
            _ => throw new Exception("Unsupported mission type.")
        };

        user.LastMissionCompletedOn = DateTimeOffset.UtcNow;

        var decoration = await RelationshipHelper.AdvanceRelationshipsAndMaybeGetLoot(db, rng, mission.Vassals!, durationInMinutes, cToken);

        if (decoration != null)
        {
            var upgraded = await TownHelpers.MakeDecorable(db, session.UserId, cToken);
            var message = result.Message + $"\n\nDuring some downtime, {decoration.Vassal.Name} made {decoration.Decoration.Type.ToNameWithArticle()}.";
            var rewards = result.Rewards;

            rewards.Add(new($"decorations/{decoration.Decoration.Type.ToString().ToLower()}"));

            if (decoration.Quintessence > 0)
            {
                message += $"\n\n(Artistic Visions provided {decoration.Quintessence}!)";
                rewards.Add(new($"resources/quintessence", decoration.Quintessence));
            }

            if (upgraded)
                message += "\n\n(You can now place Decorations in your Town!)";

            result = result with { Message = message, Rewards = rewards };
        }
        
        PersonalLogHelper.Create(db, session.UserId, result.Message, new[]
        {
            PersonalLogActivityType.CompleteMission,
            PersonalLogHelper.TagFromMissionType(mission.Type),
            PersonalLogHelper.TagFromMissionOutcome(result.Outcome)
        });

        MissionMath.UpdateVassalsAfterMissionCompletion(mission.Vassals!, mission.Type, result.Outcome, false);

        db.Missions.Remove(mission);
        
        await db.SaveChangesAsync(cToken);

        if (mission.Type == MissionType.RecruitTown)
        {
            await UserHelper.ComputeLevel(db, user, cToken);

            await db.SaveChangesAsync(cToken);
        }

        return new(result);
    }

    public sealed record ResponseDto(MissionOutcome Outcome, string Message, List<MissionReward> Rewards);
}