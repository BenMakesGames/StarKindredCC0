using StarKindred.API.Entities;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;

namespace StarKindred.API.Utility.Adventures;

public static class MineGold
{
    public static async Task<AdventureResult> Do(Db db, List<Vassal> vassals, string? narrative, CancellationToken cToken)
    {
        var userId = vassals[0].UserId;

        var totalLevel = vassals.Sum(v => v.Level);

        var baseGold = totalLevel * 12;
        var gold = baseGold;

        // ravens:
        var totalRavens = vassals.Count(v => v.Sign == AstrologicalSign.Raven);

        gold += baseGold * totalRavens * 20 / 100;

        // shovels:
        var shovelBonus = vassals
            .Where(v => v.Weapon != null)
            .Select(v => v.Weapon)
            .Sum(WeaponHelper.BonusGoldForGold);

        gold += (int) (baseGold * shovelBonus);

        // golden touch:
        gold += baseGold * vassals.Count(v => v.StatusEffects!.Any(se => se.Type == StatusEffectType.GoldenTouch));

        foreach (var v in vassals)
        {
            StatusEffectsHelper.RemoveStatusEffect(v, StatusEffectType.GoldenTouch);

            if(v.Nature == Nature.Monger)
                VassalMath.IncreaseWillpower(v);
        }

        var resourceQuantities = new List<ResourceQuantity>() { new(ResourceType.Gold, gold) };

        await ResourceHelper.CollectResources(db, userId, resourceQuantities, cToken);

        return new(narrative, new() { $"{gold} Gold" }, MissionReward.CreateFromResources(resourceQuantities));
    }
}