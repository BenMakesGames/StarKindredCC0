using BenMakesGames.RandomHelpers;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using Humanizer;
using StarKindred.API.Endpoints.TimedMissions;
using StarKindred.API.Entities;

namespace StarKindred.API.Utility.TimedMissions;

public static class TreasureHunt
{
    public static async Task<Complete.ResponseDto> Do(
        Db db, Random rng, int level, TreasureType? treasure, WeaponBonus? weapon, List<Vassal> vassals, CancellationToken cToken
    )
    {
        var maxVassals = MissionMath.MaxVassals(MissionType.TreasureHunt, level);

        var percentChanceOfSuccess = MissionMath.PercentChanceOfSuccess(level, maxVassals, vassals, false);

        var outcome = MissionOutcome.Bad;

        var great = rng.Next(20) == 1;
        var good = rng.Next(100) <= percentChanceOfSuccess || vassals.Any(v => v.Sign == AstrologicalSign.DoubleTrident);

        if (great || good)
        {
            if (
                vassals.Any(v => v.Sign == AstrologicalSign.TheGoddess) ||
                vassals.Any(v => StatusEffectsHelper.RemoveStatusEffect(v, StatusEffectType.Focused)) ||
                great
            )
                outcome = MissionOutcome.Great;
            else
                outcome = MissionOutcome.Good;
        }

        // each vassal collects gold equal to their % of the mission level (max of 125%)
        var multiplier = vassals.Sum(v => Math.Min(1.25, (v.Level + 1) / (level + 1f)));
        var baseGold = (int)(10 * level * multiplier);
        var gold = baseGold;
        var wheat = 0;
        var wine = 0;

        var vassalWeapons = vassals
            .Where(v => v.Weapon != null)
            .Select(v => v.Weapon)
            .ToList();

        if (weapon != null)
        {
            var wheatBonus = vassalWeapons.Sum(WeaponHelper.BonusWheatForWeapons);

            wheat += (int)(level * wheatBonus);
        }

        foreach (var v in vassals)
        {
            if (v.Sign == AstrologicalSign.Raven)
                gold += baseGold / 5;
            else if (v.Sign == AstrologicalSign.EightPlants)
                wheat += baseGold / 5;
        }

        var shovelBonus = vassalWeapons.Sum(WeaponHelper.BonusGoldForGold);

        gold += (int)(baseGold * shovelBonus);

        if (outcome != MissionOutcome.Bad)
        {
            gold += baseGold * vassals.Count(v => v.StatusEffects!.Any(se => se.Type == StatusEffectType.GoldenTouch));

            vassals.ForEach(v => StatusEffectsHelper.RemoveStatusEffect(v, StatusEffectType.GoldenTouch));
        }

        gold = rng.Next(gold * 3 / 4, gold * 5 / 4 + 1);
        wheat = wheat == 0 ? 0 : rng.Next(wheat * 3 / 4, wheat * 5 / 4 + 1);

        if (outcome == MissionOutcome.Great)
        {
            gold *= 2;
            wheat *= 2;
        }
        else if (outcome == MissionOutcome.Bad)
        {
            gold = 0;
            wheat /= 2;
        }

        var bonusWineForGold = vassalWeapons.Sum(WeaponHelper.BonusWineForGold);

        wine += (int)Math.Ceiling(gold * bonusWineForGold);

        var collected = new List<string>();

        if(gold > 0) collected.Add($"{gold} Gold");
        if(wheat > 0) collected.Add($"{wheat} Wheat");
        if(wine > 0) collected.Add($"{wine} Wine");

        var resourceQuantities = new List<ResourceQuantity>()
        {
            new(ResourceType.Gold, gold),
            new(ResourceType.Wheat, wheat),
            new(ResourceType.Wine, wine),
        };

        var rewards = MissionReward.CreateFromResources(resourceQuantities);

        var outcomeText = outcome == MissionOutcome.Great
            ? $"{vassals.Humanize(v => v.Name)} found an incredible amount of treasure! {collected.Humanize()} were collected."
            : $"{vassals.Humanize(v => v.Name)} found the treasure, bringing home {collected.Humanize()}."
        ;

        var completed = true;

        if(outcome == MissionOutcome.Bad)
        {
            var randomVassal = rng.Next(vassals);

            StatusEffectsHelper.AddStatusEffect(randomVassal, StatusEffectType.BrokenBone, 3);

            var reward = (treasure, weapon) switch
            {
                (TreasureType t, null) => t.ToName(),
                (null, WeaponBonus) => WeaponHelper.Names[weapon.Value],
                (_, _) => "???"
            };

            if (StatusEffectsHelper.AddStatusEffect(randomVassal, StatusEffectType.BrokenBone, 3))
                outcomeText += $" It went badly, however: {randomVassal.Name} sprung a trap, and was hit by a heavy boulder, receiving a Broken Bone. The expedition had to be called off, and the {reward} was not collected :(";
            else
                outcomeText += $" It went badly, however: {randomVassal.Name} sprung a trap, and was hit by a heavy boulder; the expedition had to be called off, and the {reward} was not collected :( Fortunately, where most Vassals would have received a Broken Bone, {randomVassal.Name} escaped with minor cuts and bruises.";

            completed = false;
        }
        else if(treasure != null)
        {
            await TreasureHelper.CollectTreasure(db, vassals[0].UserId, treasure.Value, 1, cToken);

            outcomeText += $" Also, {treasure.Value.ToNameWithArticle()} was found!";

            rewards.Add(new($"treasures/{treasure.Value.ToString().ToLower()}"));
        }
        else if(weapon != null)
        {
            var item = WeaponHelper.CollectWeapon(db, rng, vassals[0].UserId, weapon.Value);

            outcomeText += $" Also, the {item.Name} was found!";

            rewards.Add(new($"weapons/{item.Image}"));
        }

        // Explorers gain 1 Willpower
        foreach (var v in vassals.Where(v => v.Nature == Nature.Explorer))
            VassalMath.IncreaseWillpower(v);

        if(percentChanceOfSuccess <= 50)
        {
            // reward Thrill-seekers
            foreach (var v in vassals.Where(v => v.Nature == Nature.ThrillSeeker))
                VassalMath.IncreaseWillpower(v);
        }

        await ResourceHelper.CollectResources(
            db,
            vassals[0].UserId,
            resourceQuantities,
            cToken
        );

        return new Complete.ResponseDto(
            outcome,
            completed,
            outcomeText,
            rewards
        );
    }
}