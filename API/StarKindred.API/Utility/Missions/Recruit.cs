using BenMakesGames.RandomHelpers;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Endpoints.Missions;
using StarKindred.API.Entities;
using StarKindred.API.Utility.Adventures;
using StarKindred.API.Utility.Technologies;

namespace StarKindred.API.Utility.Missions;

public static class Recruit
{
    public static readonly Species[] AvailableSpecies =
    {
        Species.Human,
        Species.Midine,
        Species.Ruqu
    };

    public static async Task<Complete.ResponseDto> Do(Db db, Random rng, User user, Species species, List<Vassal> vassals, CancellationToken cToken)
    {
        var totalVassalCount = await db.Vassals.CountAsync(v => v.UserId == user.Id, cToken);
        var hasMilitarization = await TechTree.HasTechnology(db, user.Id, TechnologyType.Militarization, cToken);
        
        var outcome = totalVassalCount == 1 ? MissionOutcome.Great : rng.NextMissionOutcome(vassals);

        var vassalNames = vassals.Count == 1
            ? $"{vassals[0].Name}"
            : "your Vassals";
        
        var numRecruits = outcome == MissionOutcome.Great ? 2 : 1;

        var recruits = new List<Vassal>();

        var initialRelationshipMinutes = MissionMath.DurationInMinutes(MissionType.RecruitTown, 0, vassals);

        for (int i = 0; i < numRecruits; i++)
        {
            var recruitLevel = ComputeRecruitLevel(rng, vassals);

            var recruit = VassalGenerator.Generate(rng, recruitLevel, species, hasMilitarization);

            recruit.UserId = user.Id;

            recruits.Add(recruit);

            db.Vassals.Add(recruit);

            db.Relationships.AddRange(vassals.Select(recruiter => new Relationship()
            {
                Vassals = new List<Vassal>() { recruiter, recruit },
                Minutes = initialRelationshipMinutes
            }));
        }

        var recruitNames = recruits.Select(r => r.Name).ToList();

        var (gold, wine) = await CollectGoldAndOrWine(db, vassals, cToken);

        RewardEvangelists(vassals);

        var rewards = MissionReward.CreateFromResources(new() { new(ResourceType.Gold, gold), new(ResourceType.Wine, wine) });

        foreach(var r in recruits)
            rewards.Add(new MissionReward($"vassal/portraits/{r.Species.ToString().ToLower()}/{r.Portrait}"));

        if (outcome == MissionOutcome.Great)
        {
            var message = $"The people of the town were very impressed by {vassalNames}! {recruitNames.Humanize()} have both agreed to join you!";

            if(gold > 0)
            {
                message += $" Delighted to see your Vassals, the people of the village also offered gifts worth {gold} Gold";

                if(wine > 0) message += $", and {wine} Wine";

                message += "!";
            }

            return new Complete.ResponseDto(
                MissionOutcome.Great,
                message,
                rewards
            );
        }

        if (outcome == MissionOutcome.Bad)
        {
            // TODO: get pickpocketed, or something
        }

        var message2 = $"{vassalNames.Transform(To.SentenceCase)} visited the village, and recruited {recruitNames.Humanize()}.";

        if(gold > 0)
        {
            message2 += $" Delighted to see your Vassals, the people of the village also offered gifts worth {gold} Gold";

            if(wine > 0) message2 += $", and {wine} Wine";

            message2 += "!";
        }

        return new Complete.ResponseDto(
            MissionOutcome.Good,
            message2,
            rewards
        );
    }

    public static async Task<AdventureResult> DoAdventure(Db db, Random rng, User user, List<Vassal> vassals, string? narrative, int durationInMinutes, bool hasMilitarization, CancellationToken cToken)
    {
        var outcome = rng.NextMissionOutcome(vassals);

        var vassalNames = vassals.Count == 1
            ? $"{vassals[0].Name}"
            : "your Vassals";

        var numRecruits = outcome == MissionOutcome.Great ? 2 : 1;

        var recruitNames = new List<string>();
        var recruits = new List<Vassal>();

        for (int i = 0; i < numRecruits; i++)
        {
            var recruitLevel = ComputeRecruitLevel(rng, vassals);

            var recruit = VassalGenerator.Generate(rng, recruitLevel, rng.Next(AvailableSpecies), hasMilitarization);

            recruit.UserId = user.Id;

            recruits.Add(recruit);
            recruitNames.Add(recruit.Name);

            db.Vassals.Add(recruit);

            db.Relationships.AddRange(vassals.Select(recruiter => new Relationship()
            {
                Vassals = new List<Vassal>() { recruiter, recruit },
                Minutes = durationInMinutes
            }));
        }

        var (gold, wine) = await CollectGoldAndOrWine(db, vassals, cToken);

        RewardEvangelists(vassals);

        var collected = new List<string>();

        if(gold > 0) collected.Add($"{gold} Gold");
        if(wine > 0) collected.Add($"{wine} Wine");

        var details = $"\n\n{vassalNames.Transform(To.SentenceCase)} recruited {recruitNames.Humanize()}.";

        var rewards = MissionReward.CreateFromResources(new()
        {
            new(ResourceType.Gold, gold),
            new(ResourceType.Wine, wine)
        });

        foreach(var r in recruits)
            rewards.Add(new MissionReward($"vassal/portraits/{r.Species.ToString().ToLower()}/{r.Portrait}"));

        return new(narrative + details, collected, rewards);
    }

    public static int ComputeRecruitLevel(Random rng, IReadOnlyCollection<Vassal> vassals)
    {
        var maxLevel = vassals.Max(v => v.Level);

        var vassalLevelsThatCount = vassals.Select(v => v.Level).Where(l => l > maxLevel / 2).ToList();

        var minLevel = vassalLevelsThatCount.Sum() / (vassalLevelsThatCount.Count + 1);

        var recruitLevel = rng.Next(minLevel, maxLevel + 1);

        foreach (var vassal in vassals)
        {
            if (vassal.Sign == AstrologicalSign.PanFlute)
                recruitLevel += (int) Math.Ceiling(vassal.Level / 8f);

            recruitLevel += WeaponHelper.NewRecruitLevelBonus(vassal.Weapon!);
        }

        return Math.Clamp(recruitLevel, 0, 100);
    }

    private static async Task<(int gold, int wine)> CollectGoldAndOrWine(Db db, List<Vassal> vassals, CancellationToken cToken)
    {
        var crowns = vassals.Count(v => v.Sign == AstrologicalSign.Crown);
        var gold = 0;
        var wine = 0;

        if (crowns > 0)
        {
            var crownGold = (10 + vassals.Where(v => v.Sign == AstrologicalSign.Crown).Sum(v => v.Level)) * 5;

            var shovelBonus = vassals
                .Where(v => v.Weapon != null)
                .Select(v => v.Weapon)
                .Sum(WeaponHelper.BonusGoldForGold);

            gold += crownGold;
            gold += (int) (crownGold * shovelBonus);
            gold += crownGold * vassals.Count(v => v.StatusEffects!.Any(se => se.Type == StatusEffectType.GoldenTouch));

            foreach (var v in vassals)
            {
                StatusEffectsHelper.RemoveStatusEffect(v, StatusEffectType.GoldenTouch);

                if(v.Nature == Nature.Monger)
                    VassalMath.IncreaseWillpower(v);
            }
        }

        var bonusWineForGold = vassals
            .Where(v => v.Weapon != null)
            .Select(v => v.Weapon)
            .Sum(WeaponHelper.BonusWineForGold);

        wine += (int) Math.Ceiling(gold * bonusWineForGold);

        await ResourceHelper.CollectResources(
            db,
            vassals[0].UserId,
            new List<ResourceQuantity>()
            {
                new(ResourceType.Gold, gold),
                new(ResourceType.Wine, wine)
            },
            cToken
        );

        return (gold, wine);
    }

    private static void RewardEvangelists(IList<Vassal> vassals)
    {
        foreach(var v in vassals.Where(v => v.Nature == Nature.Evangelist))
            VassalMath.IncreaseWillpower(v);
    }
}