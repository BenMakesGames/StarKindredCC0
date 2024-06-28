using BenMakesGames.RandomHelpers;
using StarKindred.API.Entities;
using StarKindred.API.Services;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;

namespace StarKindred.API.Utility.Buildings.Powers;

public static class MarbleQuarryPowers
{
    public static async Task<(string, ResourceQuantity?, bool)> DoMetallicVein(
        Db db,
        ICurrentUser.CurrentSessionDto session,
        Random rng,
        List<TechnologyType> researchedTechnologies,
        CancellationToken cToken
    )
    {
        var techBonus = researchedTechnologies.Count(t => t is TechnologyType.HushingI or TechnologyType.HushingII) * 50;

        // randomly received either ~500 gold or ~500 iron
        var resources = new ResourceQuantity(
            rng.NextBool() ? ResourceType.Gold : ResourceType.Iron,
            rng.Next(400, rng.Next(501, 701)) + techBonus
        );

        await ResourceHelper.CollectResources(db, session.UserId, new() { resources }, cToken);

        return ($"Workers stumbled upon a vein of {resources.Type}! {resources.Quantity} {resources.Type} was collected.", resources, false);
    }
}