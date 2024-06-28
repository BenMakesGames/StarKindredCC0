using StarKindred.Common.Entities;
using StarKindred.API.Entities;

namespace StarKindred.API.Utility.Buildings;

public static class BuildingHarvestMath
{
    public const int MaxMinutes = 60 * 48;
    
    public static int SecondsTowardNextYield(BuildingType type, int level, DateTimeOffset lastHarvestedOn)
    {
        var minutesUntilYield = MinutesForYield(type, level);

        var yieldOn = lastHarvestedOn.AddMinutes(minutesUntilYield);
        
        while(yieldOn < DateTimeOffset.UtcNow)
        {
            yieldOn = yieldOn.AddMinutes(minutesUntilYield);
        }
        
        return minutesUntilYield * 60 - (int)(yieldOn - DateTimeOffset.UtcNow).TotalSeconds;
    }

    public static YieldSpeed GetYieldSpeed(BuildingType type) => type switch
    {
        BuildingType.Palace => YieldSpeed.Slow,
        BuildingType.Woodsman or BuildingType.Hunter or BuildingType.Lumberyard => YieldSpeed.Fast,
        BuildingType.Farm or BuildingType.Vineyard or BuildingType.Pasture => YieldSpeed.Fast,
        BuildingType.StonePit or BuildingType.MarbleQuarry or BuildingType.IronMine or BuildingType.GoldMine => YieldSpeed.Slow,
        BuildingType.Obelisk or BuildingType.Temple => YieldSpeed.Fast,
        BuildingType.Harbor or BuildingType.TradeDepot or BuildingType.Fishery => YieldSpeed.Slow,
        _ => throw new ArgumentException("Invalid building type", nameof(type))
    };
    
    public static Yield GetYield(BuildingType type, int level, DateTimeOffset lastHarvestedOn, List<TechnologyType> technologies)
    {
        var minutes = (int)(DateTimeOffset.UtcNow - lastHarvestedOn).TotalMinutes;
        var yieldSpeed = GetYieldSpeed(type);

        return type switch
        {
            BuildingType.Palace => GetPalaceYield(level, minutes, yieldSpeed),

            BuildingType.Woodsman => GetLumberjackYield(level, minutes, yieldSpeed),
            BuildingType.Hunter => GetHunterYield(level, minutes, yieldSpeed),
            BuildingType.Lumberyard => GetLumberyardYield(level, minutes, yieldSpeed),

            BuildingType.Farm => GetFarmYield(level, minutes, yieldSpeed),
            BuildingType.Vineyard => GetVineyardYield(level, minutes, yieldSpeed),
            BuildingType.Pasture => GetPastureYield(level, minutes, yieldSpeed),

            BuildingType.StonePit => GetStonePitYield(level, minutes, yieldSpeed),
            BuildingType.MarbleQuarry => GetMarbleQuarryYield(level, minutes, yieldSpeed),
            BuildingType.IronMine => GetIronMineYield(level, minutes, yieldSpeed),
            BuildingType.GoldMine => GetGoldMineYield(level, minutes, yieldSpeed),

            BuildingType.Obelisk => GetObeliskYield(level, minutes, yieldSpeed),
            BuildingType.Temple => GetTempleYield(level, minutes, yieldSpeed, technologies),

            BuildingType.Harbor => GetHarborYield(level, minutes, yieldSpeed),
            BuildingType.TradeDepot => GetTradeDepotYield(level, minutes, yieldSpeed),
            BuildingType.Fishery => GetFisheryYield(level, minutes, yieldSpeed),

            _ => throw new ArgumentException("Invalid building type", nameof(type))
        };
    }

    public static int MinutesForYield(BuildingType type, int level) =>
        MinutesForYield(GetYieldSpeed(type), level);
    
    public static int MinutesForYield(YieldSpeed speed, int level) => speed switch
    {
        YieldSpeed.Slow => 60 - level,
        YieldSpeed.Fast => 60 - level * 3 / 2,
        _ => throw new Exception($"Unhandled yield speed: {speed}")
    };

    private static Yield GetPalaceYield(int level, int minutes, YieldSpeed yieldSpeed)
    {
        var minutesForYield = MinutesForYield(yieldSpeed, level);

        int quantity = minutes / minutesForYield;
        int clampedQuantity = Math.Min(minutes, MaxMinutes) / minutesForYield;

        return new Yield(quantity * minutesForYield, new List<ResourceQuantity>()
        {
            new(ResourceType.Wood, clampedQuantity * 5),
            new(ResourceType.Wheat, clampedQuantity * 5),
            new(ResourceType.Gold, clampedQuantity * 5),
        });
    }

    private static Yield GetLumberjackYield(int level, int minutes, YieldSpeed yieldSpeed)
    {
        var minutesForYield = MinutesForYield(yieldSpeed, level);

        int quantity = minutes / minutesForYield;
        int clampedQuantity = Math.Min(minutes, MaxMinutes) / minutesForYield;

        return new Yield(quantity * minutesForYield, new List<ResourceQuantity>()
        {
            new(ResourceType.Wood, clampedQuantity * 10)
        });
    }
    
    private static Yield GetHunterYield(int level, int minutes, YieldSpeed yieldSpeed)
    {
        var minutesForYield = MinutesForYield(yieldSpeed, level);

        int quantity = minutes / minutesForYield;
        int clampedQuantity = Math.Min(minutes, MaxMinutes) / minutesForYield;

        return new Yield(quantity * minutesForYield, new List<ResourceQuantity>()
        {
            new(ResourceType.Wood, clampedQuantity * 10),
            new(ResourceType.Meat, clampedQuantity * 5)
        });
    } // +meat
    
    private static Yield GetLumberyardYield(int level, int minutes, YieldSpeed yieldSpeed)
    {
        var minutesForYield = MinutesForYield(yieldSpeed, level);

        int quantity = minutes / minutesForYield;
        int clampedQuantity = Math.Min(minutes, MaxMinutes) / minutesForYield;

        return new Yield(quantity * minutesForYield, new List<ResourceQuantity>()
        {
            new(ResourceType.Wood, clampedQuantity * 15),
        });
    } // +wood
    
    private static Yield GetFarmYield(int level, int minutes, YieldSpeed yieldSpeed)
    {
        var minutesForYield = MinutesForYield(yieldSpeed, level);

        int quantity = minutes / minutesForYield;
        int clampedQuantity = Math.Min(minutes, MaxMinutes) / minutesForYield;

        return new Yield(quantity * minutesForYield, new List<ResourceQuantity>()
        {
            new(ResourceType.Wheat, clampedQuantity * 10)
        });
    } // wheat
    
    private static Yield GetVineyardYield(int level, int minutes, YieldSpeed yieldSpeed)
    {
        var minutesForYield = MinutesForYield(yieldSpeed, level);

        int quantity = minutes / minutesForYield;
        int clampedQuantity = Math.Min(minutes, MaxMinutes) / minutesForYield;

        return new Yield(quantity * minutesForYield, new List<ResourceQuantity>()
        {
            new(ResourceType.Wheat, clampedQuantity * 10),
            new(ResourceType.Wine, clampedQuantity * 5)
        });
    } // +wine
    
    private static Yield GetPastureYield(int level, int minutes, YieldSpeed yieldSpeed)
    {
        var minutesForYield = MinutesForYield(yieldSpeed, level);

        int quantity = minutes / minutesForYield;
        int clampedQuantity = Math.Min(minutes, MaxMinutes) / minutesForYield;

        return new Yield(quantity * minutesForYield, new List<ResourceQuantity>()
        {
            new(ResourceType.Wheat, clampedQuantity * 10),
            new(ResourceType.Meat, clampedQuantity * 5)
        });
    } // +meat
    
    private static Yield GetStonePitYield(int level, int minutes, YieldSpeed yieldSpeed)
    {
        var minutesForYield = MinutesForYield(yieldSpeed, level);

        int quantity = minutes / minutesForYield;
        int clampedQuantity = Math.Min(minutes, MaxMinutes) / minutesForYield;

        return new Yield(quantity * minutesForYield, new List<ResourceQuantity>()
        {
            new(ResourceType.Stone, clampedQuantity * 10),
        });
    } // stone
    
    private static Yield GetMarbleQuarryYield(int level, int minutes, YieldSpeed yieldSpeed)
    {
        var minutesForYield = MinutesForYield(yieldSpeed, level);

        int quantity = minutes / minutesForYield;
        int clampedQuantity = Math.Min(minutes, MaxMinutes) / minutesForYield;

        return new Yield(quantity * minutesForYield, new List<ResourceQuantity>()
        {
            new(ResourceType.Stone, clampedQuantity * 10),
            new(ResourceType.Marble, clampedQuantity * 5),
        });
    } // +marble

    private static Yield GetIronMineYield(int level, int minutes, YieldSpeed yieldSpeed)
    {
        var minutesForYield = MinutesForYield(yieldSpeed, level);

        int quantity = minutes / minutesForYield;
        int clampedQuantity = Math.Min(minutes, MaxMinutes) / minutesForYield;

        return new Yield(quantity * minutesForYield, new List<ResourceQuantity>()
        {
            new(ResourceType.Stone, clampedQuantity * 10),
            new(ResourceType.Iron, clampedQuantity * 5),
        });
    } // +iron
    
    private static Yield GetGoldMineYield(int level, int minutes, YieldSpeed yieldSpeed)
    {
        var minutesForYield = MinutesForYield(yieldSpeed, level);

        int quantity = minutes / minutesForYield;
        int clampedQuantity = Math.Min(minutes, MaxMinutes) / minutesForYield;

        return new Yield(quantity * minutesForYield, new List<ResourceQuantity>()
        {
            new(ResourceType.Stone, clampedQuantity * 10),
            new(ResourceType.Gold, clampedQuantity * 5),
        });
    } // +gold
    
    private static Yield GetObeliskYield(int level, int minutes, YieldSpeed yieldSpeed)
    {
        var minutesForYield = MinutesForYield(yieldSpeed, level);

        int quantity = minutes / minutesForYield;
        int clampedQuantity = Math.Min(minutes, MaxMinutes) / minutesForYield;

        return new Yield(quantity * minutesForYield, new List<ResourceQuantity>()
        {
            new(ResourceType.Quintessence, clampedQuantity * 5),
        });
    } // quint
    
    private static Yield GetTempleYield(int level, int minutes, YieldSpeed yieldSpeed, List<TechnologyType> technologies)
    {
        var minutesForYield = MinutesForYield(yieldSpeed, level);

        int quantity = minutes / minutesForYield;
        int clampedQuantity = Math.Min(minutes, MaxMinutes) / minutesForYield;

        var resources = new List<ResourceQuantity>()
        {
            new(ResourceType.Quintessence, clampedQuantity * 10),
        };

        var tithesCount = technologies.Count(t =>
            t is TechnologyType.TithesI or TechnologyType.TithesII or TechnologyType.TithesIII or
                TechnologyType.TithesIV
        );

        if (tithesCount > 0)
            resources.Add(new(ResourceType.Wine, clampedQuantity * 10 / 4 * tithesCount));

        return new Yield(quantity * minutesForYield, resources);
    } // +quint
    
    private static Yield GetHarborYield(int level, int minutes, YieldSpeed yieldSpeed)
    {
        var minutesForYield = MinutesForYield(yieldSpeed, level);

        int quantity = minutes / minutesForYield;
        int clampedQuantity = Math.Min(minutes, MaxMinutes) / minutesForYield;

        return new Yield(quantity * minutesForYield, new List<ResourceQuantity>()
        {
            new(ResourceType.Meat, clampedQuantity * 10),
        });
    } // meat
    
    private static Yield GetTradeDepotYield(int level, int minutes, YieldSpeed yieldSpeed)
    {
        var minutesForYield = MinutesForYield(yieldSpeed, level);

        int quantity = minutes / minutesForYield;
        int clampedQuantity = Math.Min(minutes, MaxMinutes) / minutesForYield;

        return new Yield(quantity * minutesForYield, new List<ResourceQuantity>()
        {
            new(ResourceType.Meat, clampedQuantity * 10),
            new(ResourceType.Gold, clampedQuantity * 5),
        });
    } // +gold
    
    private static Yield GetFisheryYield(int level, int minutes, YieldSpeed yieldSpeed)
    {
        var minutesForYield = MinutesForYield(yieldSpeed, level);

        int quantity = minutes / minutesForYield;
        int clampedQuantity = Math.Min(minutes, MaxMinutes) / minutesForYield;

        return new Yield(quantity * minutesForYield, new List<ResourceQuantity>()
        {
            new(ResourceType.Meat, clampedQuantity * 15),
        });
    } // +meat

    public sealed record Yield(int MinutesConsumed, List<ResourceQuantity> Resources);
    
    public enum YieldSpeed
    {
        Fast,
        Slow,
    }
    
    public static List<BuildingType> GetAvailableSpecializations(BuildingType type) => type switch
    {
        BuildingType.Farm => new List<BuildingType>() { BuildingType.Pasture, BuildingType.Vineyard },
        BuildingType.Harbor => new List<BuildingType>() { BuildingType.Fishery, BuildingType.TradeDepot },
        BuildingType.Woodsman => new List<BuildingType>() { BuildingType.Hunter, BuildingType.Lumberyard },
        BuildingType.StonePit => new List<BuildingType>() { BuildingType.GoldMine, BuildingType.IronMine, BuildingType.MarbleQuarry },
        BuildingType.Obelisk => new List<BuildingType>() { BuildingType.Temple },
        _ => new List<BuildingType>(),
    };
}