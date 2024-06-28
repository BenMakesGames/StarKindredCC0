using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;

namespace StarKindred.API.Utility.Buildings;

public class BuildingPowerHelpers
{
    public static void BuildingPowerReadyOrThrow(Building building)
    {
        if (building.PowerLastActivatedOn.UtcDateTime.Date >= DateTimeOffset.UtcNow.Date)
            throw new UnprocessableEntity("That building's power is not ready to use.");
    }

    public static List<PowerDto> AvailablePowers(Building building, List<TechnologyType> technologies)
    {
        return building.Type switch
        {
            BuildingType.Palace => AvailablePalacePowers(technologies),
            BuildingType.Hunter => AvailableHunterPowers(technologies),
            BuildingType.Lumberyard => AvailableLumberyardPowers(technologies),
            BuildingType.Vineyard => AvailableVineyardPowers(technologies),
            BuildingType.Pasture => AvailablePasturePowers(technologies),
            BuildingType.MarbleQuarry => AvailableMarbleQuarryPowers(technologies),
            BuildingType.IronMine => AvailableIronMinePowers(technologies),
            BuildingType.GoldMine => AvailableGoldMinePowers(technologies),
            BuildingType.Temple => AvailableTemplePowers(technologies),
            BuildingType.TradeDepot => AvailableTradeDepotPowers(technologies),
            BuildingType.Fishery => AvailableFisheryPowers(technologies),
            _ => throw new Exception($"Unsupported building type: {building.Type}")
        };
    }

    private static List<PowerDto> AvailablePalacePowers(List<TechnologyType> technologies)
    {
        var powers = new List<PowerDto>()
        {
            new(Level20Power.Palace_Knight, "Knighting Ceremony", "ui/crown", new(ResourceType.Gold, 1000)),
        };

        if(technologies.Contains(TechnologyType.TourismI))
            powers.Add(new(Level20Power.Palace_AttractSettlers, "Attract Settlers", "ui/covered-wagon", new(ResourceType.Gold, 500)));

        return powers;
    }

    private static List<PowerDto> AvailableHunterPowers(List<TechnologyType> technologies)
    {
        return new()
        {
            new(Level20Power.Hunter_Hunt, "Monster-hunting", "ui/hunter", null),
        };
    }

    private static List<PowerDto> AvailableLumberyardPowers(List<TechnologyType> technologies)
    {
        var powers = new List<PowerDto>()
        {
            new(Level20Power.Lumberyard_MoreWood, "500 Wood", "resources/wood", null),
            new(Level20Power.Lumberyard_Axe, "Axe", "weapons/unknown/axe", null)
        };

        if(technologies.Contains(TechnologyType.FinishingIII))
            powers.Add(new(Level20Power.Lumberyard_FinishedGoods, "300 Gold", "resources/gold", null));
        else if(technologies.Contains(TechnologyType.FinishingII))
            powers.Add(new(Level20Power.Lumberyard_FinishedGoods, "250 Gold", "resources/gold", null));
        else if(technologies.Contains(TechnologyType.FinishingI))
            powers.Add(new(Level20Power.Lumberyard_FinishedGoods, "200 Gold", "resources/gold", null));

        return powers;
    }

    private static List<PowerDto> AvailableVineyardPowers(List<TechnologyType> technologies)
    {
        var powers = new List<PowerDto>()
        {
            new(Level20Power.Vineyard_Sell, "500 Gold", "resources/gold", new(ResourceType.Wine, 1000)),
            new(Level20Power.Vineyard_Party, "Entertain", "ui/party", new(ResourceType.Wine, 1000))
        };

        if(technologies.Contains(TechnologyType.SacrificeIII))
            powers.Add(new(Level20Power.Pasture_Vineyard_Sacrifice, "100 Quint", "resources/quintessence", null));
        else if(technologies.Contains(TechnologyType.SacrificeII))
            powers.Add(new(Level20Power.Pasture_Vineyard_Sacrifice, "75 Quint", "resources/quintessence", null));
        else if(technologies.Contains(TechnologyType.SacrificeI))
            powers.Add(new(Level20Power.Pasture_Vineyard_Sacrifice, "50 Quint", "resources/quintessence", null));

        return powers;
    }

    private static List<PowerDto> AvailablePasturePowers(List<TechnologyType> technologies)
    {
        var powers = new List<PowerDto>()
        {
            new(Level20Power.Pasture_Homestead, "Homestead", "decorations/threeanimals", null)
        };

        if(technologies.Contains(TechnologyType.SacrificeIII))
            powers.Add(new(Level20Power.Pasture_Vineyard_Sacrifice, "100 Quint", "resources/quintessence", null));
        else if(technologies.Contains(TechnologyType.SacrificeII))
            powers.Add(new(Level20Power.Pasture_Vineyard_Sacrifice, "75 Quint", "resources/quintessence", null));
        else if(technologies.Contains(TechnologyType.SacrificeI))
            powers.Add(new(Level20Power.Pasture_Vineyard_Sacrifice, "50 Quint", "resources/quintessence", null));

        return powers;
    }

    private static List<PowerDto> AvailableMarbleQuarryPowers(List<TechnologyType> technologies)
    {
        return new()
        {
            new(Level20Power.MarbleQuarry_Veins, "Metallic Vein", "vein", null)
        };
    }

    private static List<PowerDto> AvailableIronMinePowers(List<TechnologyType> technologies)
    {
        return new()
        {
            new(Level20Power.IronMine_Repair, "Repair Tools", "ui/repair", null)
        };
    }

    private static List<PowerDto> AvailableGoldMinePowers(List<TechnologyType> technologies)
    {
        return new()
        {
            new(Level20Power.GoldMine_Gems, "Gems!", "treasures/emerald", null)
        };
    }

    private static List<PowerDto> AvailableTemplePowers(List<TechnologyType> technologies)
    {
        var powers = new List<PowerDto>()
        {
            new(Level20Power.Temple_Miracle, "Miracle", "ui/miracle", null)
        };

        if (technologies.Contains(TechnologyType.Ritual))
            powers.Add(new(Level20Power.Temple_Wand, "Wand", "weapons/unknown/wand", null));

        return powers;
    }

    private static List<PowerDto> AvailableTradeDepotPowers(List<TechnologyType> technologies)
    {
        var relevantTechnologies = technologies.Count(t =>
            t is TechnologyType.MapMakingI or TechnologyType.MapMakingII or
                TechnologyType.MapMakingIII or TechnologyType.MapMakingIV
        );
        
        var cost = 1000 - relevantTechnologies * 50;
        
        return new()
        {
            new(Level20Power.TradeDepot_Wheat, "2000 Wheat", "resources/wheat", new(ResourceType.Gold, cost)),
            new(Level20Power.TradeDepot_Wood, "1500 Wood", "resources/wood", new(ResourceType.Gold, cost)),
            new(Level20Power.TradeDepot_Meat, "1000 Meat", "resources/meat", new(ResourceType.Gold, cost)),
            new(Level20Power.TradeDepot_Stone, "1000 Stone", "resources/stone", new(ResourceType.Gold, cost)),
            new(Level20Power.TradeDepot_Iron, "750 Iron", "resources/iron", new(ResourceType.Gold, cost)),
            new(Level20Power.TradeDepot_Wine, "750 Wine", "resources/wine", new(ResourceType.Gold, cost)),
            new(Level20Power.TradeDepot_Marble, "500 Marble", "resources/marble", new(ResourceType.Gold, cost)),
        };
    }

    private static List<PowerDto> AvailableFisheryPowers(List<TechnologyType> technologies)
    {
        var available = new List<PowerDto>()
        {
            new(Level20Power.Fishery_BoatRide, "Boat Ride", "boatride", null),
        };

        if(technologies.Contains(TechnologyType.CupronickleIII))
            available.Add(new(Level20Power.Fishery_Market, "300 Gold", "resources/gold", null));
        else if(technologies.Contains(TechnologyType.CupronickleII))
            available.Add(new(Level20Power.Fishery_Market, "250 Gold", "resources/gold", null));
        else if(technologies.Contains(TechnologyType.CupronickleI))
            available.Add(new(Level20Power.Fishery_Market, "200 Gold", "resources/gold", null));

        return available;
    }

    public static async Task<(Building Building, List<TechnologyType> Technologies)> PrepareBuildingPower(Db db, Guid userId, Guid buildingId, Level20Power power, CancellationToken cToken)
    {
        var building = await db.Buildings
            .FirstOrDefaultAsync(b => b.Id == buildingId && b.UserId == userId, cToken)
            ?? throw new NotFoundException("That building does not exist.");

        BuildingPowerReadyOrThrow(building);

        var techs = await db.UserTechnologies
            .Where(t => t.UserId == userId)
            .Select(t => t.Technology)
            .ToListAsync(cToken);
        
        var availablePowers = AvailablePowers(building, techs);

        if(!availablePowers.Any(p => p.Power == power))
            throw new NotFoundException("That power does not exist.");

        var choice = availablePowers.First(p => p.Power == power);

        if(choice.Cost is { } cost)
        {
            var resources = await db.Resources
                .Where(r => r.UserId == userId && r.Type == cost.Type)
                .ToListAsync(cToken);

            ResourceHelper.PayOrThrow(resources, new() { cost });
        }

        return (building, techs);
    }
}

public sealed record PowerDto(Level20Power Power, string Title, string Image, ResourceQuantity? Cost);