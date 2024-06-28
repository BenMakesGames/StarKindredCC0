using StarKindred.API.Entities;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;

namespace StarKindred.API.Utility.Adventures;

public static class Gather
{
    public static async Task<AdventureResult> Do(Db db, List<Vassal> vassals, string? narrative, CancellationToken cToken)
    {
        var userId = vassals[0].UserId;

        var totalLevel = vassals.Sum(v => v.Level);
        var totalEightPlants = vassals.Count(v => v.Sign == AstrologicalSign.EightPlants);

        var baseWheat = totalLevel * 20;
        var wheat = baseWheat;

        wheat += baseWheat * totalEightPlants * 20 / 100;

        await ResourceHelper.CollectResources(db, userId, new() { new(ResourceType.Wheat, wheat) }, cToken);

        return new(
            narrative,
            new() { $"{wheat} Wheat" },
            MissionReward.CreateFromResources(new() { new(ResourceType.Wheat, wheat) })
        );
    }
}