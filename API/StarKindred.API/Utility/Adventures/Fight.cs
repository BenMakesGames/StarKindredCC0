using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using StarKindred.API.Entities;

namespace StarKindred.API.Utility.Adventures;

public static class Fight
{
    public static async Task<AdventureResult> Do(Db db, List<Vassal> vassals, string? narrative, CancellationToken cToken)
    {
        var userId = vassals[0].UserId;

        var totalLevel = vassals.Sum(v => v.Level);

        var totalEightPlants = vassals.Count(v => v.Sign == AstrologicalSign.EightPlants);
        var totalCats = vassals.Count(v => v.Sign == AstrologicalSign.Cat);
        var totalRavens = vassals.Count(v => v.Sign == AstrologicalSign.Raven);

        var baseValue = totalLevel * 20;
        var meat = 0;
        var gold = 0;
        var wheat = 0;

        meat += baseValue * totalCats * 20 / 100;
        wheat += baseValue * totalEightPlants * 20 / 100;
        gold += baseValue * totalRavens * 20 / 100;

        var resources = new List<ResourceQuantity>();
        var collected = new List<string>();

        if (wheat > 0)
        {
            resources.Add(new(ResourceType.Wheat, wheat));
            collected.Add($"{wheat} Wheat");
        }

        if (meat > 0)
        {
            resources.Add(new(ResourceType.Meat, meat));
            collected.Add($"{meat} Meat");
        }

        if (gold > 0)
        {
            gold *= 1 + vassals.Count(v => v.StatusEffects!.Any(se => se.Type == StatusEffectType.GoldenTouch));
            vassals.ForEach(v => StatusEffectsHelper.RemoveStatusEffect(v, StatusEffectType.GoldenTouch));

            resources.Add(new(ResourceType.Gold, gold));
            collected.Add($"{gold} Gold");
        }

        await ResourceHelper.CollectResources(db, userId, resources, cToken);

        foreach (var v in vassals)
        {
            if (v.Nature == Nature.Defender)
            {
                VassalMath.IncreaseWillpower(v);
            }
            else if (v.Nature == Nature.Monger)
            {
                if(gold > 0)
                    VassalMath.IncreaseWillpower(v);
            }
        }

        return new(narrative, collected, MissionReward.CreateFromResources(resources));
    }
}