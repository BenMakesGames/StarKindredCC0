using StarKindred.API.Entities;
using StarKindred.API.Services;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;

namespace StarKindred.API.Utility.Buildings.Powers;

public static class TemplePowers
{
    public static async Task<(string, ResourceQuantity?, bool)> DoMiracle(
        Db db,
        ICurrentUser.CurrentSessionDto session,
        Random rng,
        CancellationToken cToken
    )
    {
        // it's a miracle!
        var (resources, miracle) = rng.Next(4) switch
        {
            0 => (new ResourceQuantity(ResourceType.Wheat, 1000), "Loaves"),
            1 => (new ResourceQuantity(ResourceType.Meat, 500), "Fishes"),
            2 => (new ResourceQuantity(ResourceType.Wine, 400), "Water to Wine"),
            3 => (new ResourceQuantity(ResourceType.Quintessence, 200), "Mana"),
            _ => throw new Exception("This should never happen."),
        };

        await ResourceHelper.CollectResources(db, session.UserId, new() { resources }, cToken);

        return ($"It's a Miracle of {miracle}! {resources.Quantity} {resources.Type} was received!", resources, false);
    }

    public static (string, ResourceQuantity?, bool) DoGetWand(
        Db db, Random rng, ICurrentUser.CurrentSessionDto session, List<TechnologyType> technologies
    )
    {
        var weapon = WeaponHelper.CollectWeapon(db, rng, session.UserId, WeaponBonus.GoldGetsWine);

        weapon.Level += technologies.Count(t => t is TechnologyType.MilitiaII or TechnologyType.FoundryII);

        return ($"The monks create a ritual Level-{weapon.Level} {weapon.Name}.", null, false);
    }
}