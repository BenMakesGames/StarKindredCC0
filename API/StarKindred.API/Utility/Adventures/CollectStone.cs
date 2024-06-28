using StarKindred.API.Entities;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;

namespace StarKindred.API.Utility.Adventures;

public static class CollectStone
{
    public static async Task<AdventureResult> Do(Db db, List<Vassal> vassals, string? narrative, CancellationToken cToken)
    {
        var userId = vassals[0].UserId;

        var totalLevel = vassals.Sum(v => v.Level);

        var baseStone = totalLevel * 15;
        var stone = baseStone;

        var resourceQuantity = new List<ResourceQuantity>() { new(ResourceType.Stone, stone) };

        await ResourceHelper.CollectResources(db, userId, resourceQuantity, cToken);

        return new(narrative, new() { $"{stone} Stone" }, MissionReward.CreateFromResources(resourceQuantity));
    }
}