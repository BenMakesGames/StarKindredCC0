namespace StarKindred.Common.Entities;

public enum BuildingType
{
    Palace,
    
    Woodsman, // wood
        Hunter, // +meat
        Lumberyard, // +wood
    
    Farm, // wheat
        Vineyard, // +wine
        Pasture, // +meat
    
    StonePit, // stone
        MarbleQuarry, // +marble
        IronMine, // +iron
        GoldMine, // +gold
    
    Obelisk, // quint
        Temple, // +quint
    
    Harbor, // meat
        TradeDepot, // +gold
        Fishery, // +meat
}

public static class BuildingTypeExtensions
{
    public static string GetDescription(this BuildingType type) => type switch
    {
        BuildingType.StonePit => "Stone Pit",
        BuildingType.MarbleQuarry => "Marble Quarry",
        BuildingType.GoldMine => "Gold Mine",
        BuildingType.IronMine => "Iron Mine",
        BuildingType.TradeDepot => "Trade Depot",
        _ => type.ToString(),
    };
}