using BenMakesGames.RandomHelpers;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using StarKindred.API.Entities;

namespace StarKindred.API.Utility;

public static class WeaponHelper
{
    public static readonly Dictionary<WeaponBonus, string> Names = new()
    {
        { WeaponBonus.HuntingLevels, "Sword" },
        { WeaponBonus.FasterMissions, "Horn" },
        { WeaponBonus.MoreGold, "Shovel" },
        { WeaponBonus.MeatGetsWood, "Axe" },
        { WeaponBonus.GoldGetsWine, "Wand" },
        { WeaponBonus.WeaponsGetWheat, "Scythe" },
        { WeaponBonus.RecruitBonus, "Lyre" },
    };

    // TODO: refactor: pull DB out of this method
    public static Weapon CollectWeapon(Db db, Random rng, Guid userId, WeaponBonus type)
    {
        var (image, rare) = GetImage(rng, type);
        var durability = rare ? 20 : rng.Next(10, rng.Next(15, 20 + 1) + 1);

        if (type is WeaponBonus.HuntingLevels or WeaponBonus.MeatGetsWood or WeaponBonus.WeaponsGetWheat) // swords, axes, & scythes
        {
            if (db.UserTechnologies.Any(ut => ut.UserId == userId && ut.Technology == TechnologyType.Blacksmithing))
                durability += 2;
        }
        else if (type is WeaponBonus.MoreGold or WeaponBonus.RecruitBonus or WeaponBonus.GoldGetsWine) // shovels, lyres, & wands
        {
            if (db.UserTechnologies.Any(ut => ut.UserId == userId && ut.Technology == TechnologyType.WoodWorking))
                durability += 2;
        }
        else if (type is WeaponBonus.FasterMissions) // horns
        {
            if (db.UserTechnologies.Any(ut => ut.UserId == userId && ut.Technology == TechnologyType.TrackingI))
                durability += 2;
        }

        var weapon = new Weapon()
        {
            UserId = userId,
            PrimaryBonus = type,
            SecondaryBonus = rng.Next(Enum.GetValues<WeaponBonus>().Where(w => w != type).ToList()),
            Name = GenerateName(rng, type, rare),
            Image = image,
            MaxDurability = durability,
            Durability = durability,
        };

        db.Weapons.Add(weapon);

        return weapon;
    }

    private static (string Image, bool Rare) GetImage(Random rng, WeaponBonus bonus)
    {
        if(rng.Next(60) == 0)
        {
            return (bonus switch
            {
                WeaponBonus.HuntingLevels => "silly/" + rng.Next(new[] { "cat" }),
                WeaponBonus.RecruitBonus => "silly/" + rng.Next(new[] { "melodica", "attractor" }),
                WeaponBonus.MoreGold => "silly/" + rng.Next(new[] { "attractor" }),
                WeaponBonus.MeatGetsWood => "silly/" + rng.Next(new[] { "cat" }),
                WeaponBonus.WeaponsGetWheat => "silly/" + rng.Next(new[] { "attractor" }),
                WeaponBonus.GoldGetsWine => "silly/" + rng.Next(new[] { "cat", "attractor" }),
                WeaponBonus.FasterMissions => "silly/" + rng.Next(new[] { "melodica" }),
                _ => ""
            }, true);
        }

        return (bonus switch
        {
            // TODO: This is a notable place where some graphics have been removed, as they are not available to the
            // public domain. You should create your own, and/or change the lists here.
            WeaponBonus.HuntingLevels => "sword/" + rng.Next(new[] { "dragon-scale", "green-scimitar", "brute-force", "vicious", "jahingirs" }),
            WeaponBonus.RecruitBonus => "lyre/" + rng.Next(new[] { "cherry-and-gold", "walnut", "har-meggido" }),
            WeaponBonus.MoreGold => "shovel/" + rng.Next(new[] { "fish-head", "gizubi", "owl" }),
            WeaponBonus.MeatGetsWood => "axe/" + rng.Next(new[] { "lightning", "sharktooth", "dark" }),
            WeaponBonus.WeaponsGetWheat => "scythe/" + rng.Next(new[] { "temperance", "new-moon", "everice" }),
            WeaponBonus.GoldGetsWine => "wand/" + rng.Next(new[] { "root", "red-warping", "happy", "yggdrasil" }),
            WeaponBonus.FasterMissions => "horn/" + rng.Next(new[] { "black", "bronze", "goat" }),
            _ => ""
        }, false);
    }

    private static string GenerateName(Random rng, WeaponBonus bonus, bool rare)
    {
        var template = GetNameTemplate(rng, rare);

        return template.Replace("%weapon%", Names[bonus]);
    }

    private static string GetNameTemplate(Random rng, bool isRare)
    {
        if(isRare)
        {
            return rng.Next(new[]
            {
                "Bizarre %weapon%",
                "Curious %weapon%",
                "Strange %weapon%",
                "Surprising %weapon%",
                "Unusual %weapon%",
            });
        }

        return rng.Next(new[]
        {
            "Bird's %weapon%",
            "Black %weapon%",
            "Blue %weapon%",
            "Butterfly %weapon%",
            "Cherry %weapon%",
            "Dark %weapon%",
            "Flying %weapon%",
            "Jeweled %weapon%",
            "Light %weapon%",
            "Moon %weapon%",
            "Shining %weapon%",
            "Silver %weapon%",
            "Singing %weapon%",
            "Stinging %weapon%",
            "Sun %weapon%",
        });
    }

    public static List<ResourceQuantity>? ResourcesToLevelUp(Weapon weapon)
    {
        if(weapon.Level >= 5)
            return null;

        return new()
        {
            new(ResourceType.Gold, weapon.Level * 50),
            new(ResourceType.Iron, weapon.Level * (weapon.Level + 1) * 100),
            new(ResourceType.Quintessence, weapon.Level * 100),
        };
    }

    public static int RepairValue(Weapon weapon) =>
        //(int)Math.Ceiling((weapon.Durability + weapon.MaxDurability) / 2f * weapon.Level / 3f);
        (int)Math.Ceiling((weapon.Durability + weapon.MaxDurability) * weapon.Level / 6f);

    public static double FasterMissionsMultiplier(Weapon? weapon) =>
        BonusLevel(weapon, WeaponBonus.FasterMissions) switch
        {
            0 => 1,
            1 => 0.95,
            2 => 0.93,
            3 => 0.9,
            _ => throw new ArgumentException("Level must be between 0 and 3.")
        };

    public static int NewRecruitLevelBonus(Weapon? weapon) =>
        BonusLevel(weapon, WeaponBonus.RecruitBonus) switch
        {
            0 => 0,
            1 => 2,
            2 => 5,
            3 => 10,
            _ => throw new ArgumentException("Level must be between 0 and 3.")
        };

    public static double BonusWoodForMeat(Weapon? weapon) =>
        BonusLevel(weapon, WeaponBonus.MeatGetsWood) switch
        {
            0 => 0,
            1 => 0.1,
            2 => 0.25,
            3 => 0.5,
            _ => throw new ArgumentException("Level must be between 0 and 3.")
        };

    public static double BonusWineForGold(Weapon? weapon) =>
        BonusLevel(weapon, WeaponBonus.GoldGetsWine) switch
        {
            0 => 0,
            1 => 0.1,
            2 => 0.25,
            3 => 0.5,
            _ => throw new ArgumentException("Level must be between 0 and 3.")
        };

    public static double BonusGoldForGold(Weapon? weapon) =>
        BonusLevel(weapon, WeaponBonus.MoreGold) switch
        {
            0 => 0,
            1 => 0.05,
            2 => 0.1,
            3 => 0.2,
            _ => throw new ArgumentException("Level must be between 0 and 3.")
        };

    public static double BonusWheatForWeapons(Weapon? weapon) =>
        BonusLevel(weapon, WeaponBonus.WeaponsGetWheat) switch
        {
            0 => 0,
            1 => 1,
            2 => 1.2,
            3 => 1.5,
            _ => throw new ArgumentException("Level must be between 0 and 3.")
        };

    public static int BonusHuntingLevels(Weapon? weapon) =>
        BonusLevel(weapon, WeaponBonus.HuntingLevels) switch
        {
            0 => 0,
            1 => 2,
            2 => 5,
            3 => 10,
            _ => throw new ArgumentException("Level must be between 0 and 3.")
        };

    public static int BonusLevel(Weapon? weapon, WeaponBonus bonus)
    {
        if(weapon == null)
            return 0;

        if(weapon.PrimaryBonus == bonus)
            return PrimaryBonusLevel(weapon.Level);

        if(weapon.SecondaryBonus == bonus)
            return SecondaryBonusLevel(weapon.Level);

        return 0;
    }

    public static int PrimaryBonusLevel(int level) => (int)Math.Floor(level / 2f) + 1;
    public static int SecondaryBonusLevel(int level) => level switch {
        1 => 0,
        2 => 0,
        3 => 1,
        4 => 1,
        5 => 2,
        _ => throw new ArgumentException("Level must be between 1 and 5.")
    };

    public static void DegradeWeaponDurability(Vassal vassal, Weapon weapon)
    {
        weapon.Durability--;

        if (weapon.Durability == 0)
            vassal.WeaponId = null;
    }

    public static void DegradeWeaponDurability(Vassal vassal, Weapon weapon, MissionType mission, bool missionCouldHaveYieldedWeapon)
    {
        if (!IsDegradedBy(weapon, mission, missionCouldHaveYieldedWeapon))
            return;

        DegradeWeaponDurability(vassal, weapon);
    }

    public static bool IsDegradedBy(Weapon weapon, MissionType mission, bool missionCouldHaveYieldedWeapon)
    {
        var bonuses = GetActiveBonuses(weapon);

        if (bonuses.Contains(WeaponBonus.FasterMissions))
            return true;

        return mission switch
        {
            MissionType.Settlers or MissionType.RecruitTown => bonuses.Any(b => b is WeaponBonus.RecruitBonus or WeaponBonus.MoreGold or WeaponBonus.GoldGetsWine),
            MissionType.TreasureHunt => bonuses.Any(b => b is WeaponBonus.MoreGold or WeaponBonus.GoldGetsWine) || (missionCouldHaveYieldedWeapon && bonuses.Any(b => b is WeaponBonus.WeaponsGetWheat)),
            MissionType.WanderingMonster => bonuses.Any(b => b is WeaponBonus.HuntingLevels or WeaponBonus.MeatGetsWood) || (missionCouldHaveYieldedWeapon && bonuses.Contains(WeaponBonus.WeaponsGetWheat)),
            MissionType.HuntLevel0 or MissionType.HuntLevel10 or MissionType.HuntLevel20 or
                MissionType.HuntLevel50 or MissionType.HuntLevel80 or MissionType.HuntLevel120 or
                MissionType.HuntLevel200 or MissionType.HuntAutoScaling => bonuses.Any(b => b is WeaponBonus.HuntingLevels or WeaponBonus.MeatGetsWood),
            _ => false
        };
    }

    public static List<WeaponBonus> GetActiveBonuses(Weapon weapon)
    {
        var bonuses = new List<WeaponBonus>()
        {
            weapon.PrimaryBonus
        };

        if (weapon.Level >= 3)
            bonuses.Add(weapon.SecondaryBonus);

        return bonuses;
    }
}
