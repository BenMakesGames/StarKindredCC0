using BenMakesGames.RandomHelpers;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using StarKindred.API.Endpoints.TimedMissions;
using StarKindred.API.Entities;
using StarKindred.API.Utility.Technologies;

namespace StarKindred.API.Utility.TimedMissions;

public static class Settlers
{
    public static async Task<Complete.ResponseDto> Do(Db db, Random rng, User user, Species species, int recruitLevel, List<Vassal> vassals, CancellationToken cToken)
    {
        var outcome = rng.NextMissionOutcome(vassals);
        var hasMilitarization = await TechTree.HasTechnology(db, user.Id, TechnologyType.Militarization, cToken);

        var vassalNames = vassals.Count == 1
            ? $"{vassals[0].Name}"
            : "your Vassals";

        var actualRecruitLevel = recruitLevel;
        
        foreach(var v in vassals)
            actualRecruitLevel += WeaponHelper.NewRecruitLevelBonus(v.Weapon!);

        var recruit = VassalGenerator.Generate(rng, actualRecruitLevel, species, hasMilitarization);

        recruit.UserId = vassals[0].UserId;

        db.Vassals.Add(recruit);

        var initialRelationshipMinutes = MissionMath.DurationInMinutes(MissionType.RecruitTown, 0, vassals);
        
        db.Relationships.AddRange(vassals.Select(recruiter => new Relationship()
        {
            Vassals = new List<Vassal>() { recruiter, recruit },
            Minutes = initialRelationshipMinutes
        }));

        var crowns = vassals.Count(v => v.Sign == AstrologicalSign.Crown);
        var gold = 0;
        var wine = 0;
        
        if(crowns > 0)
        {
            var crownGold = (10 + vassals.Where(v => v.Sign == AstrologicalSign.Crown).Sum(v => v.Level)) * 5;

            var shovelBonus = vassals
                .Where(v => v.Weapon != null)
                .Select(v => v.Weapon)
                .Sum(WeaponHelper.BonusGoldForGold);

            gold += crownGold;
            gold += (int)(crownGold * shovelBonus);
            gold += crownGold * vassals.Count(v => v.StatusEffects!.Any(se => se.Type == StatusEffectType.GoldenTouch));

            foreach (var v in vassals)
            {
                StatusEffectsHelper.RemoveStatusEffect(v, StatusEffectType.GoldenTouch);

                if(v.Nature == Nature.Monger)
                    VassalMath.IncreaseWillpower(v);
            }
        }

        // reward Evangelists
        foreach(var v in vassals.Where(v => v.Nature == Nature.Evangelist))
            VassalMath.IncreaseWillpower(v);
        
        var bonusWineForGold = vassals
            .Where(v => v.Weapon != null)
            .Select(v => v.Weapon)
            .Sum(WeaponHelper.BonusWineForGold);

        wine += (int)Math.Ceiling(gold * bonusWineForGold);
        
        if (outcome == MissionOutcome.Great)
        {
            var message = $"The settlers were very impressed by {vassalNames}! In addition to agreeing to settle in your village, {recruit.Name} joined you as Vassals!";

            if(gold > 0)
            {
                if(wine > 0)
                    message += $"\n\nThe settlers brought with them {gold} Gold, {wine} Wine, ";
                else
                    message += $"\n\nThe settlers brought with them {gold} Gold, ";
            }
            else
                message += "\n\nThe settlers brought with them ";
            
            var wheat = rng.Next(recruitLevel * 2, recruitLevel * 7 / 2);
            var meat = rng.Next(recruitLevel, recruitLevel * 5 / 2);
            var wood = rng.Next(0, recruitLevel * 3 / 2);
            
            message += $"{wood} Wood, {wheat} Wheat, and {meat} Meat.";

            var greatResourceQuantities = new List<ResourceQuantity>()
            {
                new(ResourceType.Wheat, wheat),
                new(ResourceType.Meat, meat),
                new(ResourceType.Wood, wood),
                new(ResourceType.Wine, wine),
                new(ResourceType.Gold, gold),
            };

            await ResourceHelper.CollectResources(
                db,
                user.Id,
                greatResourceQuantities,
                cToken
            );

            await db.SaveChangesAsync(cToken);

            var greatRewards = MissionReward.CreateFromResources(greatResourceQuantities);

            greatRewards.Add(new($"vassal/portraits/{recruit.Species.ToString().ToLower()}/{recruit.Portrait}"));

            return new Complete.ResponseDto(
                MissionOutcome.Great,
                true,
                message,
                greatRewards
            );
        }

        var message2 = $"The settlers are happy to talk with {vassalNames} for a while. They're grateful for the offer to join you, but are headed to a place which holds a special significance to their people. Still, a few decide to break from tradition, and join you. Among them, {recruit.Name}, who has the spark necessary to become a Vassal.";

        if (outcome == MissionOutcome.Bad)
        {
            message2 += "\n\nDuring the conversation, you were ambushed by a group of bandits! A few settlers were killed, and ";

            var woundedVassal = rng.Next(vassals);
            
            if(StatusEffectsHelper.AddStatusEffect(woundedVassal, StatusEffectType.BrokenBone, 3))
                message2 += $"{woundedVassal.Name} received several heavy attacks in the fight, and got a Broken Bone!";
            else
                message2 += $"{woundedVassal.Name} received several heavy attacks in the fight! Anyone else would have received a Broken Bone, but {woundedVassal.Name} escaped with only minor cuts and bruises.";

            if(gold > 0)
            {
                if(wine > 0)
                    message2 += $" The settlers offered {gold} Gold and {wine} Wine, either as help defeating the bandits, or to pay you to leave them alone, it isn't clear...";
                else
                    message2 += $" The settlers offered {gold} Gold, either as help defeating the bandits, or to pay you to leave them alone, it isn't clear...";
            }
        }
        else
        {
            if(gold > 0)
            {
                if(wine > 0)
                    message2 += $" The settlers offered gifts ({gold} Gold and {wine} Wine) before everyone parted ways.";
                else
                    message2 += $" The settlers offered gifts ({gold} Gold) before everyone parted ways.";
            }
        }

        var resourceQuantities = new List<ResourceQuantity>()
        {
            new(ResourceType.Wine, gold),
            new(ResourceType.Wine, wine),
        };

        await ResourceHelper.CollectResources(
            db,
            user.Id,
            resourceQuantities,
            cToken
        );

        await db.SaveChangesAsync(cToken);

        var rewards = MissionReward.CreateFromResources(resourceQuantities);

        rewards.Add(new($"vassal/portraits/{recruit.Species.ToString().ToLower()}/{recruit.Portrait}"));

        await UserHelper.ComputeLevel(db, user, cToken);

        await db.SaveChangesAsync(cToken);

        return new Complete.ResponseDto(
            outcome,
            true,
            message2,
            rewards
        );
    }
}