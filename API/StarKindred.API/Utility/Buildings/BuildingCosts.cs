using StarKindred.Common.Entities;
using StarKindred.API.Entities;

namespace StarKindred.API.Utility.Buildings;

public static class BuildingCosts
{
    public static List<BuildingType> BuildingsAvailableAtPosition(int position) => position switch
    {
        2 => new() { BuildingType.Farm, BuildingType.Obelisk, BuildingType.Woodsman },
        1 or 3 or 5 => new() { BuildingType.Farm, BuildingType.Obelisk },
        4 or 6 => new() { BuildingType.Woodsman },
        7 or 8 => new() { BuildingType.StonePit },
        9 or 10 => new() { BuildingType.Harbor },
        _ => throw new ArgumentException("Position must be between 1 and 10.")
    };

    public static int MaxLevel(BuildingType t, bool hasExpansion) => t switch
    {
        BuildingType.Farm or BuildingType.Woodsman or BuildingType.Harbor or BuildingType.Obelisk or BuildingType.StonePit => 10,
        BuildingType.Palace when hasExpansion => 30,
        _ => 20
    };

    public static List<ResourceQuantity>? CostToUpgrade(BuildingType type, int level, bool hasArchitectureI, bool hasArchitectureII, bool hasExpansion)
    {
        if(level == MaxLevel(type, hasExpansion))
            return null;
        
        var cost = type switch
        {
            BuildingType.Palace => CostToUpgradePalace(level),
            BuildingType.Woodsman => CostToUpgradeWoodsman(level),
            BuildingType.Lumberyard => CostToUpgradeLumberyard(level),
            BuildingType.Hunter => CostToUpgradeHunter(level),
            BuildingType.Farm => CostToUpgradeFarm(level),
            BuildingType.Vineyard => CostToUpgradeVineyard(level),
            BuildingType.Pasture => CostToUpgradePasture(level),
            BuildingType.StonePit => CostToUpgradeStonePit(level),
            BuildingType.IronMine => CostToUpgradeIronMine(level),
            BuildingType.GoldMine => CostToUpgradeGoldMine(level),
            BuildingType.MarbleQuarry => CostToUpgradeMarbleQuarry(level),
            BuildingType.Obelisk => CostToUpgradeObelisk(level),
            BuildingType.Temple => CostToUpgradeTemple(level),
            BuildingType.Harbor => CostToUpgradeHarbor(level),
            BuildingType.Fishery => CostToUpgradeFishery(level),
            BuildingType.TradeDepot => CostToUpgradeTradeDepot(level),
            _ => throw new ArgumentException("Invalid building type", nameof(type))
        };

        return cost
            .Select(c => c.Type switch
            {
                ResourceType.Wood when hasArchitectureI => c with { Quantity = c.Quantity * 8 / 10 },
                ResourceType.Stone or ResourceType.Marble when hasArchitectureII => c with { Quantity = c.Quantity * 8 / 10 },
                _ => c
            })
            .ToList();
    }

    public static List<ResourceQuantity> CostToUpgradePalace(int level)
    {
        var resources = new List<ResourceQuantity>()
        {
            LevelRequirements(level, 0, ResourceType.Gold),
            LevelRequirements(level, 0, ResourceType.Stone),
            LevelRequirements(level, 0, ResourceType.Meat),
        };

        if(level >= 10)
        {
            resources.Add(LevelRequirements(level, 10, ResourceType.Marble));
            resources.Add(LevelRequirements(level, 10, ResourceType.Wine));
        }

        return resources;
    }

    public static List<ResourceQuantity> CostToUpgradeWoodsman(int level)
    {
        return new List<ResourceQuantity>()
        {
            new(ResourceType.Gold, 20),
            LevelRequirements(level, 0, ResourceType.Wood),
        };
    }

    public static List<ResourceQuantity> CostToUpgradeLumberyard(int level)
    {
        return new List<ResourceQuantity>()
        {
            new(ResourceType.Gold, 40),
            LevelRequirements(level, 0, ResourceType.Wood),
            LevelRequirements(level, 10, ResourceType.Iron),
        };
    }

    public static List<ResourceQuantity> CostToUpgradeHunter(int level)
    {
        return new List<ResourceQuantity>()
        {
            new(ResourceType.Gold, 40),
            LevelRequirements(level, 0, ResourceType.Wood),
            LevelRequirements(level, 10, ResourceType.Iron),
        };
    }

    public static List<ResourceQuantity> CostToUpgradeFarm(int level)
    {
        return new List<ResourceQuantity>()
        {
            new(ResourceType.Gold, 20),
            LevelRequirements(level, 0, ResourceType.Wood),
        };
    }

    public static List<ResourceQuantity> CostToUpgradePasture(int level)
    {
        return new List<ResourceQuantity>()
        {
            new(ResourceType.Gold, 40),
            LevelRequirements(level, 0, ResourceType.Wood),
            LevelRequirements(level, 10, ResourceType.Wheat),
        };
    }

    public static List<ResourceQuantity> CostToUpgradeVineyard(int level)
    {
        var baseCost = new List<ResourceQuantity>()
        {
            new(ResourceType.Gold, 40),
            LevelRequirements(level, 0, ResourceType.Wood),
        };

        return ResourceHelper.Add(baseCost, LevelRequirements(level, 10, ResourceType.Gold));
    }

    public static List<ResourceQuantity> CostToUpgradeStonePit(int level)
    {
        return new List<ResourceQuantity>()
        {
            new(ResourceType.Gold, 20),
            LevelRequirements(level, 0, ResourceType.Wood),
        };
    }

    public static List<ResourceQuantity> CostToUpgradeGoldMine(int level)
    {
        return new List<ResourceQuantity>()
        {
            new(ResourceType.Gold, 20),
            LevelRequirements(level, 0, ResourceType.Wood),
            LevelRequirements(level, 10, ResourceType.Iron),
        };
    }

    public static List<ResourceQuantity> CostToUpgradeMarbleQuarry(int level)
    {
        return new List<ResourceQuantity>()
        {
            new(ResourceType.Gold, 20),
            LevelRequirements(level, 0, ResourceType.Wood),
            LevelRequirements(level, 10, ResourceType.Iron),
        };
    }

    public static List<ResourceQuantity> CostToUpgradeIronMine(int level)
    {
        return new List<ResourceQuantity>()
        {
            new(ResourceType.Gold, 20),
            LevelRequirements(level, 0, ResourceType.Wood),
            LevelRequirements(level, 10, ResourceType.Iron),
        };
    }

    public static List<ResourceQuantity> CostToUpgradeObelisk(int level)
    {
        return new List<ResourceQuantity>()
        {
            new(ResourceType.Gold, 20),
            LevelRequirements(level, 0, ResourceType.Stone),
        };
    }

    public static List<ResourceQuantity> CostToUpgradeTemple(int level)
    {
        return new List<ResourceQuantity>()
        {
            new(ResourceType.Gold, 20),
            LevelRequirements(level, 0, ResourceType.Wood),
            LevelRequirements(level, 10, ResourceType.Marble),
        };
    }

    public static List<ResourceQuantity> CostToUpgradeHarbor(int level)
    {
        return new List<ResourceQuantity>()
        {
            new(ResourceType.Gold, 20),
            LevelRequirements(level, 0, ResourceType.Wood),
        };
    }

    public static List<ResourceQuantity> CostToUpgradeFishery(int level)
    {
        var baseCost = new List<ResourceQuantity>()
        {
            new(ResourceType.Gold, 20),
            LevelRequirements(level, 0, ResourceType.Wood),
        };

        return ResourceHelper.Add(baseCost, LevelRequirements(level, 10, ResourceType.Wood));
    }

    public static List<ResourceQuantity> CostToUpgradeTradeDepot(int level)
    {
        var baseCost = new List<ResourceQuantity>()
        {
            new(ResourceType.Gold, 20),
            LevelRequirements(level, 0, ResourceType.Wood),
        };

        return ResourceHelper.Add(baseCost, LevelRequirements(level, 10, ResourceType.Wood));
    }

    private static ResourceQuantity LevelRequirements(int buildingLevel, int levelThreshold, ResourceType resource)
    {
        var targetLevel = buildingLevel + 1;
        
        return new ResourceQuantity(resource, (targetLevel - levelThreshold) * 50);
    }

    public static readonly Dictionary<BuildingType, List<ResourceQuantity>> BuildCost = new()
    {
        {
            BuildingType.Farm,
            new() {
                new ResourceQuantity(ResourceType.Wood, 100),
                new ResourceQuantity(ResourceType.Gold, 20),
            }
        },
        {
            BuildingType.Obelisk,
            new() {
                new ResourceQuantity(ResourceType.Stone, 100),
                new ResourceQuantity(ResourceType.Gold, 20),
            }
        },
        {
            BuildingType.Woodsman,
            new() {
                new ResourceQuantity(ResourceType.Wood, 100),
                new ResourceQuantity(ResourceType.Gold, 20),
            }
        },
        {
            BuildingType.StonePit,
            new() {
                new ResourceQuantity(ResourceType.Wood, 100),
                new ResourceQuantity(ResourceType.Gold, 20),
            }
        },
        {
            BuildingType.Harbor,
            new() {
                new ResourceQuantity(ResourceType.Wood, 100),
                new ResourceQuantity(ResourceType.Stone, 50),
                new ResourceQuantity(ResourceType.Gold, 20),
            }
        }
    };

    private static readonly Dictionary<BuildingType, List<BuildingType>> AvailableRebuildTypes = new()
    {
        { BuildingType.Farm, new() { BuildingType.Woodsman, BuildingType.Obelisk } },
        { BuildingType.Woodsman, new() { BuildingType.Farm, BuildingType.Obelisk } },
        { BuildingType.Obelisk, new() { BuildingType.Farm, BuildingType.Lumberyard } },
        { BuildingType.Pasture, new() {BuildingType.Vineyard } },
        { BuildingType.Vineyard, new() {BuildingType.Pasture } },
        { BuildingType.Fishery, new() { BuildingType.TradeDepot } },
        { BuildingType.TradeDepot, new() { BuildingType.Fishery } },
        { BuildingType.Lumberyard, new() { BuildingType.Hunter } },
        { BuildingType.Hunter, new() { BuildingType.Lumberyard } },
        { BuildingType.GoldMine, new() { BuildingType.IronMine, BuildingType.MarbleQuarry } },
        { BuildingType.IronMine, new() { BuildingType.GoldMine, BuildingType.MarbleQuarry } },
        { BuildingType.MarbleQuarry, new() { BuildingType.GoldMine, BuildingType.IronMine } },
    };
    
    public static List<BuildingType> GetAvailableRebuildTypes(BuildingType type, int position, bool hasArchitectureIII)
    {
        if (hasArchitectureIII)
        {
            return BuildingsAvailableAtPosition(position)
                .SelectMany(BuildingHarvestMath.GetAvailableSpecializations)
                .Except(new[] { type })
                .ToList();
        }

        if (!AvailableRebuildTypes.ContainsKey(type))
            return new();
        
        // 1. get all the available types this building COULD rebuild into
        var availableTypes = AvailableRebuildTypes[type];

        // 2a. get the available types at this position
        var availableAtPosition = BuildingsAvailableAtPosition(position);

        // 2b. add the upgrades for all the types available at this position
        var availableUpgrades = availableAtPosition.SelectMany(BuildingHarvestMath.GetAvailableSpecializations).ToList();
        availableAtPosition.AddRange(availableUpgrades);
        
        // 3. remove from all the available types the types that are not available at this position
        return availableTypes.Intersect(availableAtPosition).ToList();
    }
}