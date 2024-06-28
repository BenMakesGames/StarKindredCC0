using BenMakesGames.RandomHelpers;
using StarKindred.API.Entities;
using StarKindred.API.Services;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;

namespace StarKindred.API.Utility.Buildings.Powers;

public static class GoldMinePowers
{
    public static async Task<(string, ResourceQuantity?, bool)> DoGemPower(
        Db db,
        ICurrentUser.CurrentSessionDto session,
        Random rng,
        List<TechnologyType> technologies,
        CancellationToken cToken
    )
    {
        var gold = technologies.Count(t => t is TechnologyType.HushingI or TechnologyType.HushingII) * 50;

        // get a random gem???
        var gem = rng.Next(new[]
        {
            //TreasureType.Mercury,
            TreasureType.Emerald,
            //TreasureType.LapisLazuli,
        });

        var received = $"1 {gem}";

        if (gold > 0)
        {
            await ResourceHelper.CollectResources(db, session.UserId, new() { new(ResourceType.Gold, gold) }, cToken);
            received += $", and {gold} Gold";
        }

        await TreasureHelper.CollectTreasure(db, session.UserId, gem, 1, cToken);

        return ($"Miners found a deposit of rare {gem}! (Received {received}.)", null, false);
    }
}