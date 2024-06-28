using StarKindred.API.Entities;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;

namespace StarKindred.API.Utility;

public static class VassalMath
{
    public static int MaxLevel(Vassal vassal) => vassal.Sign == AstrologicalSign.Mountain ? 110 : 100;
    public static int MaxWillpower(Vassal vassal) => vassal.Sign == AstrologicalSign.LargeCupAndLittleCup ? 4 : 3;

    public static ResourceType NatureResourceType(Nature nature) => nature switch
    {
        Nature.Cavalier => ResourceType.Iron,
        Nature.Competitor => ResourceType.Gold,
        Nature.Defender => ResourceType.Stone,
        Nature.Explorer => ResourceType.Wood,
        Nature.Evangelist => ResourceType.Quintessence,
        Nature.Loner => ResourceType.Stone,
        Nature.Monger => ResourceType.Gold,
        Nature.Perfectionist => ResourceType.Marble,
        Nature.ThrillSeeker => ResourceType.Wine,
        Nature.Visionary => ResourceType.Quintessence,
        _ => throw new ArgumentException("Invalid nature")
    };

    public static List<ResourceQuantity> ResourcesToLevelUp(Vassal vassal, bool hasFreeTradeTech)
    {
        var resources = BaseRequirements(vassal.Species, vassal.Level);

        if (vassal.Level >= 20)
        {
            resources = ResourceHelper.Add(resources, vassal.Nature switch
            {
                Nature.Cavalier => LevelRequirements(vassal.Level, 20, ResourceType.Iron),
                Nature.Competitor => LevelRequirements(vassal.Level, 20, ResourceType.Gold),
                Nature.Defender => LevelRequirements(vassal.Level, 20, ResourceType.Stone),
                Nature.Explorer => LevelRequirements(vassal.Level, 20, ResourceType.Wood),
                Nature.Evangelist => LevelRequirements(vassal.Level, 20, ResourceType.Quintessence),
                Nature.Loner => LevelRequirements(vassal.Level, 20, ResourceType.Stone),
                Nature.Monger => LevelRequirements(vassal.Level, 20, ResourceType.Gold),
                Nature.Perfectionist => LevelRequirements(vassal.Level, 20, ResourceType.Marble),
                Nature.ThrillSeeker => LevelRequirements(vassal.Level, 20, ResourceType.Wine),
                Nature.Visionary => LevelRequirements(vassal.Level, 20, ResourceType.Quintessence),
                _ => throw new ArgumentException("Invalid nature")
            }).ToList();
        }

        if (vassal.Level >= 60)
        {
            resources = ResourceHelper.Add(resources, vassal.Sign switch
            {
                AstrologicalSign.Raven => LevelRequirements(vassal.Level, 60, ResourceType.Quintessence),
                AstrologicalSign.Kundrav => LevelRequirements(vassal.Level, 60, ResourceType.Meat),
                AstrologicalSign.EightPlants => LevelRequirements(vassal.Level, 60, ResourceType.Wheat),
                AstrologicalSign.River => LevelRequirements(vassal.Level, 60, ResourceType.Wheat),
                AstrologicalSign.DoubleTrident => LevelRequirements(vassal.Level, 60, ResourceType.Meat),
                AstrologicalSign.Tablet => LevelRequirements(vassal.Level, 60, ResourceType.Quintessence),
                AstrologicalSign.LargeCupAndLittleCup => LevelRequirements(vassal.Level, 60, ResourceType.Wine),
                AstrologicalSign.Cat => LevelRequirements(vassal.Level, 60, ResourceType.Meat),
                AstrologicalSign.Crown => LevelRequirements(vassal.Level, 60, ResourceType.Gold),
                AstrologicalSign.TheGoddess => LevelRequirements(vassal.Level, 60, ResourceType.Quintessence),
                AstrologicalSign.Mountain => LevelRequirements(vassal.Level, 60, ResourceType.Stone),
                AstrologicalSign.PapyrusBoat => LevelRequirements(vassal.Level, 60, ResourceType.Gold),
                AstrologicalSign.PanFlute => LevelRequirements(vassal.Level, 60, ResourceType.Wine),
                _ => throw new ArgumentException("Invalid astrological sign")
            }).ToList();
        }

        if(vassal.Sign == AstrologicalSign.Tablet)
            resources = resources.Select(r => r with { Quantity = r.Quantity * 9 / 10 }).ToList();

        if (hasFreeTradeTech)
        {
            resources = resources
                .Where(r => !(r.Type == ResourceType.Gold && r.Quantity == 20))
                .Select(r => r.Type == ResourceType.Gold ? r with { Quantity = r.Quantity - 20 } : r)
                .ToList();
        }

        return resources;
    }

    private static List<ResourceQuantity> BaseRequirements(Species species, int vassalLevel)
    {
        var resourceTypes = species switch
        {
            Species.Midine => (ResourceType.Meat, ResourceType.Wheat, ResourceType.Meat),
            Species.Human => (ResourceType.Wheat, ResourceType.Meat, ResourceType.Gold),
            Species.Ruqu => (ResourceType.Wheat, ResourceType.Quintessence, ResourceType.Wine),
            Species.Puturu => (ResourceType.Wood, ResourceType.Wheat, ResourceType.Quintessence),
            _ => throw new ArgumentException($"Level-up requirements for {species} not implemented :(")
        };
        
        var targetLevel = vassalLevel + 1;

        var resources = new List<ResourceQuantity>()
        {
            new(ResourceType.Gold, (vassalLevel / 10 + 1) * 20),
            new(resourceTypes.Item1, Math.Min(targetLevel, 20) * 50)
        };

        if(targetLevel > 40)
            resources = ResourceHelper.Add(resources, LevelRequirements(vassalLevel, 40, resourceTypes.Item2));

        if(targetLevel > 80)
            resources = ResourceHelper.Add(resources, LevelRequirements(vassalLevel, 80, resourceTypes.Item3));

        if(targetLevel > 100)
            resources = ResourceHelper.Add(resources, LevelRequirements(vassalLevel, 100, resourceTypes.Item1));

        return resources;
    }
    
    private static ResourceQuantity LevelRequirements(int vassalLevel, int levelThreshold, ResourceType resource)
        => new(resource, LevelRequirementQuantity(vassalLevel + 1, levelThreshold));

    private static int LevelRequirementQuantity(int targetLevel, int levelThreshold)
        => Math.Min(targetLevel - levelThreshold, 20) * 50;

    public static List<WillpowerOption> WillpowerOptions(Vassal vassal)
    {
        var options = new List<WillpowerOption>();

        if (vassal.Level < 10)
            options.Add(new(WillpowerSpendType.LevelUp, 1, true));
        else if (vassal.Level < 20)
            options.Add(new(WillpowerSpendType.LevelUp, 2, true));
        else if (vassal.Level < 30 && MaxWillpower(vassal) >= 3)
            options.Add(new(WillpowerSpendType.LevelUp, 3, true));
        else if (vassal.Level < 40 && MaxWillpower(vassal) >= 4)
            options.Add(new(WillpowerSpendType.LevelUp, 4, true));

        if(vassal.Level >= 10)
        {
            options.Add(new(
                WillpowerSpendType.Focus,
                2,
                !vassal.StatusEffects!.Any(s => s.Type == StatusEffectType.Focused)
            ));
        }

        if (vassal.Level >= 30)
        {
            var (power, statusEffect) = vassal.Nature switch
            {
                Nature.Cavalier or Nature.Competitor or Nature.ThrillSeeker or Nature.Defender => (WillpowerSpendType.LevelBuff, StatusEffectType.Power),
                Nature.Visionary or Nature.Evangelist or Nature.Loner => (WillpowerSpendType.QuintBuff, StatusEffectType.ArtisticVision),
                Nature.Explorer or Nature.Monger or Nature.Perfectionist => (WillpowerSpendType.GoldBuff, StatusEffectType.GoldenTouch),
                _ => throw new NotImplementedException()
            };

            options.Add(new(
                power,
                2,
                !vassal.StatusEffects!.Any(s => s.Type == statusEffect)
            ));
        }

        return options;
    }

    public static void IncreaseWillpower(Vassal v)
    {
        if (v.RetirementPoints < 10)
            v.RetirementPoints++;

        if(v.Willpower < MaxWillpower(v))
            v.Willpower++;
    }
}