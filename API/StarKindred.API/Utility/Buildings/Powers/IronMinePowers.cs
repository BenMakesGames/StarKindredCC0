using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;

namespace StarKindred.API.Utility.Buildings.Powers;

public static class IronMinePowers
{
    public static async Task<(string, ResourceQuantity?, bool)> DoRepairPower(
        Db db,
        ICurrentUser.CurrentSessionDto session,
        CancellationToken cToken
    )
    {
        // repair 1 Durability for all weapons
        var affectedWeapons = await db.Weapons
            .Where(w => w.UserId == session.UserId && w.Durability < w.MaxDurability)
            .ToListAsync(cToken);

        if (affectedWeapons.Count == 0)
            throw new UnprocessableEntity("You have no equipment to repair.");

        foreach (var w in affectedWeapons)
            w.Durability++;

        return ($"{affectedWeapons.Count} weapons & tools had 1 Durability repaired.", null, false);
    }

    public static (string, ResourceQuantity?, bool) DoGetSword(
        Db db, Random rng, ICurrentUser.CurrentSessionDto session, List<TechnologyType> technologies
    )
    {
        var weapon = WeaponHelper.CollectWeapon(db, rng, session.UserId, WeaponBonus.HuntingLevels);

        weapon.Level += technologies.Count(t => t is TechnologyType.MilitiaII or TechnologyType.FoundryII);

        return ($"The foundry-workers have produced a Level-{weapon.Level} {weapon.Name}.", null, false);
    }
}