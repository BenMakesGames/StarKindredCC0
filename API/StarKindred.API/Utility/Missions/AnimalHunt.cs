using BenMakesGames.RandomHelpers;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using Humanizer;
using StarKindred.API.Endpoints.Missions;
using StarKindred.API.Entities;

namespace StarKindred.API.Utility.Missions;

public static class AnimalHunt
{
    public static async Task<Complete.ResponseDto> Do(
        Db db, Random rng, int level, int maxVassals, List<Vassal> vassals, CancellationToken cToken
    )
    {
        var percentChanceOfSuccess = MissionMath.PercentChanceOfSuccess(level, maxVassals, vassals, true);

        var outcome = MissionOutcome.Bad;

        var great = rng.Next(20) == 1;
        var good = rng.Next(100) <= percentChanceOfSuccess || vassals.Any(v => v.Sign == AstrologicalSign.DoubleTrident);

        if (great || good)
        {
            if (vassals.Any(v => StatusEffectsHelper.RemoveStatusEffect(v, StatusEffectType.Focused)) || great)
                outcome = MissionOutcome.Great;
            else
                outcome = MissionOutcome.Good;
        }

        // each vassal collects meat equal to their % of the mission level (max of 125%)
        var multiplier = vassals.Sum(v => Math.Min(1.25, (v.Level + 1) / (level + 1f)));
        var halfLevel = level / 2;
        var baseMeat = 20 - level / 10 + (int)(halfLevel * Math.Sqrt(halfLevel) * multiplier);
        var meat = baseMeat;
        var wheat = 0;
        var wood = 0;
        
        foreach (var v in vassals)
        {
            if (v.Sign == AstrologicalSign.Cat)
                meat += baseMeat / 5;
            else if (v.Sign == AstrologicalSign.EightPlants)
                wheat += baseMeat / 5;
        }

        meat = rng.Next(meat * 3 / 4, meat * 5 / 4 + 1);
        wheat = wheat == 0 ? 0 : rng.Next(wheat * 3 / 4, wheat * 5 / 4 + 1);

        if (outcome == MissionOutcome.Great)
        {
            meat = meat * 3 / 2;
            wheat = wheat * 3 / 2;
        }
        else if (outcome == MissionOutcome.Bad)
        {
            meat = (int)Math.Sqrt(meat);
            wheat = (int) Math.Sqrt(wheat);
        }

        var bonusWoodForMeat = vassals
            .Where(v => v.Weapon != null)
            .Select(v => v.Weapon)
            .Sum(WeaponHelper.BonusWoodForMeat);

        wood += (int)Math.Ceiling(meat * bonusWoodForMeat);
        
        var collected = new List<string>();

        collected.Add($"{meat} Meat");

        if(wheat > 0)
            collected.Add($"{wheat} Wheat");

        if(wood > 0)
            collected.Add($"{wood} Wood");
        
        var outcomeText = outcome == MissionOutcome.Great
            ? $"{vassals.Humanize(v => v.Name)} had a great hunt! {collected.Humanize()} were collected."
            : $"{vassals.Humanize(v => v.Name)} hunted for a while, bringing home {collected.Humanize()}."
        ;
        
        if(outcome == MissionOutcome.Bad)
        {
            var woundedVassal = rng.Next(vassals);
            
            if(StatusEffectsHelper.AddStatusEffect(woundedVassal, StatusEffectType.BrokenBone, 3))
                outcomeText += $"\n\nDuring the hunt, {woundedVassal.Name} received several heavy attacks from the creature, and got a Broken Bone! :(";
            else
                outcomeText += $"\n\nDuring the hunt, {woundedVassal.Name} received several heavy attacks from the creature. Anyone else would have received a Broken Bone, but {woundedVassal.Name} escaped with only minor cuts and bruises.";
        }

        if(percentChanceOfSuccess <= 50)
        {
            // reward Thrill-seekers
            foreach (var v in vassals.Where(v => v.Nature == Nature.ThrillSeeker))
                VassalMath.IncreaseWillpower(v);
        }

        var resourceQuantities = new List<ResourceQuantity>()
        {
            new(ResourceType.Meat, meat),
            new(ResourceType.Wheat, wheat),
            new(ResourceType.Wood, wood),
        };

        await ResourceHelper.CollectResources(
            db,
            vassals[0].UserId,
            resourceQuantities,
            cToken
        );
        
        return new Complete.ResponseDto(
            outcome,
            outcomeText,
            MissionReward.CreateFromResources(resourceQuantities)
        );
    }
}