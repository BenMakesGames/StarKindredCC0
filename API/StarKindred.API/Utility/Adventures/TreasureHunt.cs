using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using StarKindred.API.Entities;

namespace StarKindred.API.Utility.Adventures;

public static class TreasureHunt
{
    public static async Task<AdventureResult> Do(Db db, List<Vassal> vassals, string? narrative, CancellationToken cToken)
    {
        var userId = vassals[0].UserId;

        var totalLevel = vassals.Sum(v => v.Level);
        var totalEightPlants = vassals.Count(v => v.Sign == AstrologicalSign.EightPlants);
        var totalRavens = vassals.Count(v => v.Sign == AstrologicalSign.Raven);

        var baseGold = totalLevel * 20;
        var gold = baseGold;
        var wheat = 0;

        var resources = new List<ResourceQuantity>();
        var collected = new List<string>();

        wheat += baseGold * totalEightPlants * 20 / 100;
        gold += baseGold * totalRavens * 20 / 100;

        if (wheat > 0)
        {
            resources.Add(new(ResourceType.Wheat, wheat));
            collected.Add($"{wheat} Wheat");
        }

        if (gold > 0)
        {
            gold *= 1 + vassals.Count(v => v.StatusEffects!.Any(se => se.Type == StatusEffectType.GoldenTouch));
            vassals.ForEach(v => StatusEffectsHelper.RemoveStatusEffect(v, StatusEffectType.GoldenTouch));

            resources.Add(new(ResourceType.Gold, gold));
            collected.Add($"{gold} Gold");
        }

        foreach (var v in vassals)
        {
            if (v.Nature == Nature.Explorer)
            {
                VassalMath.IncreaseWillpower(v);
            }
            else if (v.Nature == Nature.Monger)
            {
                if(gold > 0)
                    VassalMath.IncreaseWillpower(v);
            }
        }

        await ResourceHelper.CollectResources(db, userId, resources, cToken);

        return new(narrative, collected, MissionReward.CreateFromResources(resources));
    }
}