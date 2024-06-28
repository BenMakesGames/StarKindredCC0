using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;

namespace StarKindred.API.Utility.Buildings.Powers;

public static class TradeDepotPowers
{
    public static async Task<(string, ResourceQuantity?, bool)> DoTrade(
        Db db,
        ICurrentUser.CurrentSessionDto session,
        Level20Power trade,
        CancellationToken cToken
    )
    {
        var gains = trade switch
        {
            Level20Power.TradeDepot_Wheat => new ResourceQuantity(ResourceType.Wheat, 2000),
            Level20Power.TradeDepot_Wood => new ResourceQuantity(ResourceType.Wood, 1500),
            Level20Power.TradeDepot_Meat => new ResourceQuantity(ResourceType.Meat, 1000),
            Level20Power.TradeDepot_Stone => new ResourceQuantity(ResourceType.Stone, 1000),
            Level20Power.TradeDepot_Iron => new ResourceQuantity(ResourceType.Iron, 750),
            Level20Power.TradeDepot_Wine => new ResourceQuantity(ResourceType.Wine, 750),
            Level20Power.TradeDepot_Marble => new ResourceQuantity(ResourceType.Marble, 500),
            _ => throw new UnprocessableEntity("An exchange must be selected.")
        };

        await ResourceHelper.CollectResources(db, session.UserId, new() { gains }, cToken);

        return ($"Received {gains.Quantity} {gains.Type}.", gains, false);
    }
}