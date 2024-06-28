using StarKindred.API.Entities;
using StarKindred.API.Services;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;

namespace StarKindred.API.Utility.Buildings.Powers;

public static class LumberyardPowers
{
    public static async Task<(string, ResourceQuantity?, bool)> DoMoreWood(Db db, ICurrentUser.CurrentSessionDto session, CancellationToken cToken)
    {
        var resources = new ResourceQuantity(ResourceType.Wood, 500);
        
        await ResourceHelper.CollectResources(db, session.UserId, new() { resources }, cToken);

        return ("Good lumberyarding! +500 Wood!", resources, false);
    }

    public static (string, ResourceQuantity?, bool) DoGetAxe(
        Db db, Random rng, ICurrentUser.CurrentSessionDto session, List<TechnologyType> technologies
    )
    {
        var weapon = WeaponHelper.CollectWeapon(db, rng, session.UserId, WeaponBonus.MeatGetsWood);

        weapon.Level += technologies.Count(t => t is TechnologyType.MilitiaII or TechnologyType.FoundryII);

        return ($"The lumberjacks offer you one of their axes: a Level-{weapon.Level} {weapon.Name}.", null, false);
    }

    public static async Task<(string, ResourceQuantity?, bool)> DoFinishedGoods(
        Db db,
        ICurrentUser.CurrentSessionDto session,
        List<TechnologyType> researchedTechnologies,
        CancellationToken cToken
    )
    {
        var applicableTechnologies = new[]
        {
            TechnologyType.FinishingII,
            TechnologyType.FinishingIII
        };

        var boostingTechs = researchedTechnologies.Count(t => applicableTechnologies.Contains(t));

        var gold = new ResourceQuantity(ResourceType.Gold, 200 + boostingTechs * 50);

        await ResourceHelper.CollectResources(db, session.UserId, new() { gold }, cToken);

        return ($"{gold.Quantity} Gold is earned selling finished goods.", gold, false);
    }
}