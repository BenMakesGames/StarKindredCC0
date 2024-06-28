using BenMakesGames.RandomHelpers;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using Humanizer;
using StarKindred.API.Endpoints.TimedMissions;
using StarKindred.API.Entities;

namespace StarKindred.API.Utility.TimedMissions;

public static class WanderingMonster
{
    public static async Task<Complete.ResponseDto> Do(
        Db db, Random rng, int level, Element element, TreasureType? treasure, WeaponBonus? weapon, List<Vassal> vassals, CancellationToken cToken
    )
    {
        var maxVassals = MissionMath.MaxVassals(MissionType.WanderingMonster, level);
        var percentChanceOfSuccess = MissionMath.PercentChanceOfSuccess(level, element, maxVassals, vassals);

        var outcome = MissionOutcome.Bad;

        var great = rng.Next(20) == 1;
        var good = rng.Next(100) <= percentChanceOfSuccess || vassals.Any(v => v.Sign == AstrologicalSign.DoubleTrident);

        if (great || good)
        {
            if (vassals.Any(v => StatusEffectsHelper.RemoveStatusEffect(v, StatusEffectType.Focused)) || great)
                outcome = MissionOutcome.Great;
            else
                outcome = MissionOutcome.Good;
        }

        // each vassal collects meat equal to their % of the mission level (max of 125%)
        var multiplier = vassals.Sum(v => Math.Min(1.25, (v.Level + 1) / (level + 1f)));
        var baseMeat = (int)(10 * level * multiplier);
        var gold = 0;
        var meat = baseMeat;
        var wheat = 0;
        var wood = 0;
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
            if (v.Sign == AstrologicalSign.Cat)
                meat += baseMeat / 5;
            else if (v.Sign == AstrologicalSign.EightPlants)
                wheat += baseMeat / 5;
            else if (v.Sign == AstrologicalSign.Raven)
                gold += baseMeat / 5;
        }

        var shovelBonus = vassalWeapons.Sum(WeaponHelper.BonusGoldForGold);

        gold += (int)(gold * shovelBonus)
            + gold * vassals.Count(v => v.StatusEffects!.Any(se => se.Type == StatusEffectType.GoldenTouch));

        vassals.ForEach(v => StatusEffectsHelper.RemoveStatusEffect(v, StatusEffectType.GoldenTouch));

        meat = meat == 0 ? 0 : rng.Next(meat * 3 / 4, meat * 5 / 4 + 1);
        wheat = wheat == 0 ? 0 : rng.Next(wheat * 3 / 4, wheat * 5 / 4 + 1);
        gold = gold == 0 ? 0 : rng.Next(gold * 3 / 4, gold * 5 / 4 + 1);

        if (outcome == MissionOutcome.Great)
        {
            meat = meat * 3 / 2;
            wheat = wheat * 3 / 2;
            gold = gold * 3 / 2;
        }
        else if (outcome == MissionOutcome.Bad)
        {
            meat = (int)Math.Sqrt(meat);
            wheat = (int)Math.Sqrt(wheat);
            gold = (int)Math.Sqrt(gold);
        }

        var bonusWoodForMeat = vassalWeapons.Sum(WeaponHelper.BonusWoodForMeat);

        wood += (int)Math.Ceiling(meat * bonusWoodForMeat);

        var bonusWineForGold = vassalWeapons.Sum(WeaponHelper.BonusWineForGold);

        wine += (int)Math.Ceiling(gold * bonusWineForGold);

        var collected = new List<string>();

        if(meat > 0) collected.Add($"{meat} Meat");
        if(wheat > 0) collected.Add($"{wheat} Wheat");
        if(wood > 0) collected.Add($"{wood} Wood");
        if(gold > 0) collected.Add($"{gold} Gold");
        if(wine > 0) collected.Add($"{wine} Wine");

        var resourceQuantities = new List<ResourceQuantity>()
        {
            new(ResourceType.Meat, meat),
            new(ResourceType.Wheat, wheat),
            new(ResourceType.Wood, wood),
            new(ResourceType.Gold, gold),
            new(ResourceType.Wine, wine),
        };

        var rewards = MissionReward.CreateFromResources(resourceQuantities);

        var outcomeText = outcome == MissionOutcome.Great
            ? $"{vassals.Humanize(v => v.Name)} had a great hunt! {collected.Humanize()} were collected."
            : $"{vassals.Humanize(v => v.Name)} hunted for a while, bringing home {collected.Humanize()}."
        ;

        var completed = true;
        
        if(outcome == MissionOutcome.Bad)
        {
            var randomVassal = rng.Next(vassals);

            var reward = (treasure, weapon) switch
            {
                (TreasureType t, null) => t.ToName(),
                (null, WeaponBonus) => WeaponHelper.Names[weapon.Value],
                (_, _) => "???"
            };
            
            if (StatusEffectsHelper.AddStatusEffect(randomVassal, StatusEffectType.BrokenBone, 3))
                outcomeText += $" The hunt went badly, however: the creature became enranged, and {randomVassal.Name} received several heavy attacks, and a Broken Bone; the hunt had to be called off, and the {reward} was not collected :(";
            else
                outcomeText += $" The hunt went badly, however: the creature became enranged, and {randomVassal.Name} received several heavy attacks; the hunt had to be called off, and the {reward} was not collected :( Fortunately, where most Vassals would have received a Broken Bone, {randomVassal.Name} escaped with minor cuts and bruises.";

            completed = false;
        }
        else if(treasure != null)
        {
            await TreasureHelper.CollectTreasure(db, vassals[0].UserId, treasure.Value, 1, cToken);
            
            outcomeText += $" Also, the monster dropped {treasure.Value.ToNameWithArticle()}!";

            rewards.Add(new($"treasures/{treasure.Value.ToString().ToLower()}"));
        }
        else if(weapon != null)
        {
            var item = WeaponHelper.CollectWeapon(db, rng, vassals[0].UserId, weapon.Value);

            outcomeText += $" Also, the monster's {item.Name} was claimed!";

            rewards.Add(new($"weapons/{item.Image}"));
        }

        if(outcome != MissionOutcome.Bad)
        {
            // reward Defenders
            foreach(var v in vassals.Where(v => v.Nature == Nature.Defender))
                VassalMath.IncreaseWillpower(v);
        }

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