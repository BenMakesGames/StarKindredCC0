using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using StarKindred.API.Entities;

namespace StarKindred.API.Utility.Adventures;

public static class Hunt
{
    public static async Task<AdventureResult> Do(Db db, List<Vassal> vassals, string? narrative, TreasureType? treasure, DecorationType? decoration, CancellationToken cToken)
    {
        var userId = vassals[0].UserId;

        var totalLevel = vassals.Sum(v => v.Level);

        var totalEightPlants = vassals.Count(v => v.Sign == AstrologicalSign.EightPlants);
        var totalCats = vassals.Count(v => v.Sign == AstrologicalSign.Cat);

        var baseMeat = totalLevel * 20;
        var meat = baseMeat;
        var wheat = 0;

        meat += baseMeat * totalCats * 20 / 100;
        wheat += baseMeat * totalEightPlants * 20 / 100;

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

        await ResourceHelper.CollectResources(db, userId, resources, cToken);

        return new AdventureResult(
            narrative,
            collected,
            MissionReward.CreateFromResources(resources)
        );
    }
}