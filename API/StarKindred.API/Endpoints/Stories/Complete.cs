using BenMakesGames.RandomHelpers;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;
using StarKindred.API.Utility.Adventures;
using StarKindred.API.Utility.Missions;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Utility.Technologies;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Extensions;

namespace StarKindred.API.Endpoints.Stories;

[ApiController]
public sealed class Complete
{
    [HttpPost("/stories/{id:guid}/complete")]
    public async Task<ApiResponse<ResultDto>> _(
        Guid id,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Random rng,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        var user = await db.Users.FirstAsync(u => u.Id == session.UserId, cToken);

        var progress = await db.UserAdventureStepInProgress
            .Include(a => a.AdventureStep!)
                .ThenInclude(s => s.Recruit)
            .Include(a => a.Vassals!)
                .ThenInclude(v => v.Weapon)
            .Include(a => a.Vassals!)
                .ThenInclude(v => v.StatusEffects)
            .AsSplitQuery() // TODO: not profiled
            .FirstOrDefaultAsync(a => a.UserId == session.UserId && a.Id == id, cToken)
            ?? throw new NotFoundException("Adventure not found")
        ;

        var now = DateTimeOffset.UtcNow;

        if(progress.CompletesOn >= now)
            throw new UnprocessableEntity("Adventure hasn't been completed, yet");

        if(progress.Vassals!.Count == 0)
            throw new Exception($"Story mission {progress.Id} has no Vassals. This should never be. Something is wrong.");

        var durationInMinutes = (int) (progress.CompletesOn - progress.StartedOn).TotalMinutes;
        var hasMilitarization = await TechTree.HasTechnology(db, session.UserId, TechnologyType.Militarization, cToken);

        var result = progress.AdventureStep!.Type switch
        {
            MissionType.Story => Story.Do(progress.AdventureStep.Narrative!),
            MissionType.HuntAutoScaling => await Hunt.Do(db, progress.Vassals!, progress.AdventureStep.Narrative, progress.AdventureStep!.Treasure, progress.AdventureStep!.Decoration, cToken),
            MissionType.Gather => await Gather.Do(db, progress.Vassals!, progress.AdventureStep.Narrative, cToken),
            MissionType.WanderingMonster => await Fight.Do(db, progress.Vassals!, progress.AdventureStep.Narrative, cToken),
            MissionType.TreasureHunt => await TreasureHunt.Do(db, progress.Vassals!, progress.AdventureStep.Narrative, cToken),
            MissionType.RecruitTown => await Recruit.DoAdventure(db, rng, user, progress.Vassals!, progress.AdventureStep.Narrative, durationInMinutes, hasMilitarization, cToken),
            MissionType.CollectStone => await CollectStone.Do(db, progress.Vassals!, progress.AdventureStep.Narrative, cToken),
            MissionType.MineGold => await MineGold.Do(db, progress.Vassals!, progress.AdventureStep.Narrative, cToken),
            _ => throw new Exception("Unknown adventure type")
        };

        bool exciting = false;

        if (progress.AdventureStep.Treasure is TreasureType treasure)
        {
            await TreasureHelper.CollectTreasure(db, session.UserId, treasure, 1, cToken);
            result.Collected.Add(treasure.ToNameWithArticle());
            result.Rewards.Add(new($"treasures/{treasure.ToString().ToLower()}"));
            exciting = true;
        }

        var upgradedTown = false;

        if (progress.AdventureStep.Decoration is DecorationType decoration)
        {
            upgradedTown = await TownHelpers.MakeDecorable(db, session.UserId, cToken);

            await DecorationHelper.CollectDecoration(db, session.UserId, decoration, 1, cToken);
            result.Collected.Add(decoration.ToNameWithArticle());
            result.Rewards.Add(new($"decorations/{decoration.ToString().ToLower()}"));
            exciting = true;
        }

        if (progress.AdventureStep.UnlockableAvatar is string avatar)
        {
            if (await AvatarHelpers.UnlockAvatar(db, session.UserId, avatar, cToken))
            {
                result.Collected.Add("a new Profile Picture");
                result.Rewards.Add(new($"avatars/{avatar}"));
                exciting = true;
            }
        }

        Vassal? newRecruit = null;
        
        if (progress.AdventureStep.Recruit is VassalTemplate recruit)
        {
            var min = progress.Vassals!.Sum(v => v.Level) / progress.Vassals!.Count;
            var max = Math.Min(100, progress.Vassals.Max(v => v.Level));

            var level = min >= max ? max : rng.Next(min, max + 1);

            newRecruit = GenerateRecruit(rng, session.UserId, recruit, level, hasMilitarization);

            db.Add(newRecruit);

            result.Rewards.Add(new($"vassal/portraits/{newRecruit.Species.ToString().ToLower()}/{newRecruit.Portrait}"));

            exciting = true;
        }

        var additions = new List<string>();

        if(newRecruit != null)
            additions.Add($"{newRecruit.Name} joined us");
        
        if (result.Collected.Count > 0)
            additions.Add($"we collected {result.Collected.ToNiceString()}");

        var lines = new List<string>();
        
        if(result.Text != null)
            lines.Add(result.Text);

        if(additions.Count > 0)
        {
            if(lines.Count == 0)
                lines.Add($"{additions.ToNiceString().UppercaseFirst()}{(exciting ? "!" : ".")}");
            else // enclose in parentheses
                lines.Add($"({additions.ToNiceString().UppercaseFirst()}{(exciting ? "!" : ".")})");
        }

        user.LastMissionCompletedOn = DateTimeOffset.UtcNow;

        var relationshipDecoration = await RelationshipHelper.AdvanceRelationshipsAndMaybeGetLoot(db, rng, progress.Vassals!, durationInMinutes, cToken);

        if (relationshipDecoration != null)
        {
            if(!upgradedTown)
                upgradedTown = await TownHelpers.MakeDecorable(db, session.UserId, cToken);

            result.Rewards.Add(new($"decorations/{relationshipDecoration.Decoration.Type.ToString().ToLower()}"));

            lines.Add($"During some downtime, {relationshipDecoration.Vassal.Name} made {relationshipDecoration.Decoration.Type.ToNameWithArticle()}.");

            if (relationshipDecoration.Quintessence > 0)
            {
                lines.Add($"(Artistic Visions provided {relationshipDecoration.Quintessence}!)");
                result.Rewards.Add(new("resources/quintessence", relationshipDecoration.Quintessence));
            }
        }

        if (upgradedTown)
            lines.Add("(You can now place Decorations in your Town!)");

        var text = string.Join("\n\n", lines);
        
        PersonalLogHelper.Create(db, session.UserId, text, new[]
        {
            PersonalLogActivityType.CompleteMission,
            PersonalLogHelper.TagFromMissionType(progress.AdventureStep.Type),
            PersonalLogHelper.TagFromMissionOutcome(MissionOutcome.Good)
        });
        
        MissionMath.UpdateVassalsAfterMissionCompletion(progress.Vassals!, progress.AdventureStep.Type, MissionOutcome.Good, false);

        db.UserAdventureStepCompleted.Add(new()
        {
            UserId = session.UserId,
            AdventureStepId = progress.AdventureStepId
        });

        db.UserAdventureStepInProgress.Remove(progress);

        await db.SaveChangesAsync(cToken);

        if (progress.AdventureStep.Type == MissionType.RecruitTown)
        {
            await UserHelper.ComputeLevel(db, user, cToken);

            await db.SaveChangesAsync(cToken);
        }

        return new(new(text, result.Rewards));
    }

    private Vassal GenerateRecruit(Random rng, Guid userId, VassalTemplate template, int level, bool hasMilitarization)
    {
        var possibleSpecies = new[] { Species.Human, Species.Midine, Species.Ruqu };

        var vassal = VassalGenerator.Generate(rng, level, template.Species ?? rng.Next(possibleSpecies), hasMilitarization);

        vassal.UserId = userId;
        vassal.Favorite = true;

        vassal.Name = template.Name ?? vassal.Name;

        if (template.Species != null && template.Portrait != null)
            vassal.Portrait = template.Portrait;

        vassal.Element = template.Element ?? vassal.Element;
        vassal.Sign = template.Sign ?? vassal.Sign;
        vassal.Nature = template.Nature ?? vassal.Nature;

        return vassal;
    }

    public sealed record ResultDto(string Text, List<MissionReward> Rewards);
}