using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;

namespace StarKindred.API.Utility.Buildings.Powers;

public static class VineyardPowers
{
    public static async Task<(string, ResourceQuantity?, bool)> DoEntertain(
        Db db,
        ICurrentUser.CurrentSessionDto session,
        CancellationToken cToken
    )
    {
        // thrill-seekers, pan-flutes, and ruqu gain 1 willpower
        var vassals = await db.Vassals
            .Where(v =>
                v.UserId == session.UserId && (
                    v.Nature == Nature.ThrillSeeker ||
                    v.Sign == AstrologicalSign.PanFlute ||
                    v.Species == Species.Ruqu
                )
            )
            .ToListAsync(cToken);

        if(vassals.Count == 0)
            throw new UnprocessableEntity("You have no Thrill-seekers, Pan Flutes, or Ruqu for the winemakers to entertain.");
        
        foreach (var v in vassals)
            VassalMath.IncreaseWillpower(v);

        var message = vassals.Count <= 5
            ? $"{vassals.Select(v => v.Name).ToList().ToNiceString()} enjoyed the entertainment of your city's winemakers. " + (vassals.Count == 1 ? "(They gained 1 Willpower.)" : "(They each gained 1 Willpower).")
            : $"{vassals.Count} of your Vassals (Thrill-seekers, Pan Flutes, and Ruqu) enjoyed the entertainment of your city's winemakers. (They each gained 1 Willpower.)"
        ;

        return (message, null, false);
    }

    public static async Task<(string, ResourceQuantity?, bool)> DoSell(
        Db db,
        ICurrentUser.CurrentSessionDto session,
        CancellationToken cToken
    )
    {
        var resourcesGained = new ResourceQuantity(ResourceType.Gold, 500);

        await ResourceHelper.CollectResources(db, session.UserId, new() { resourcesGained }, cToken);

        return ("Your winemakers sold the wine they produced, earning 500 Gold.", resourcesGained, false);
    }
}