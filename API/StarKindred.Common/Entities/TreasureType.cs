namespace StarKindred.Common.Entities;

// the following are equal:
// 1000 Wheat
//  750 Wood
//  500 Meat
//  500 Stone
//  375 Wine
//  375 Iron
//  250 Gold
//  250 Marble
//  150-200 Quint

public enum TreasureType
{
    MagicHammer, // upgrade a building to level 10
    Ichor,
    RenamingScroll,
    BoxOfOres, // choose: 100 stone, 75 iron, 50 marble, or 50 gold
    BasicChest, // choose: 200 wheat, 150 wood, 100 meat, or 50 gold
    BigBasicChest, // choose: 400 wheat, 300 wood, 200 meat, or 100 gold
    WeaponChest, // cloud; choose a weapon type
    GoldChest, // choose: 300 gold, 300 marble, or a magic hammer
    RubyChest, // choose: ichor, 300 iron, or 200 quint
    TwilightChest, // choose: renaming scroll, or 200 gold, or 150 quint
    TreasureMap,
    WrappedSword,
    Soma, // 1000 wheat, 400 wine, or 200 quint
    CupOfLife, // choose: ichor, 500 meat, or 200 quint
    CrystallizedQuint, // 1000 quint, or 1500 gold
    Emerald, // wand, wine, gold, or quint
    RallyingStandard,
    FishBag, // 250 meat, or 100 quint
}

public static class TreasureTypeExtensions
{
    public static string ToName(this TreasureType type, int quantity)
        => quantity == 1 ? ToName(type) : ToNamePlural(type);

    public static string ToName(this TreasureType type) => type switch {
        TreasureType.BasicChest => "Basic Chest",
        TreasureType.BigBasicChest => "Big Basic Chest",
        TreasureType.GoldChest => "Gold Chest",
        TreasureType.MagicHammer => "Magic Hammer",
        TreasureType.RenamingScroll => "Renaming Scroll",
        TreasureType.RubyChest => "Ruby Chest",
        TreasureType.TreasureMap => "Treasure Map",
        TreasureType.TwilightChest => "Twilight Chest",
        TreasureType.WeaponChest => "Weapon Chest",
        TreasureType.WrappedSword => "Wrapped Sword",
        TreasureType.BoxOfOres => "Box of Ores",
        TreasureType.CupOfLife => "Cup of Life",
        TreasureType.CrystallizedQuint => "Crystallized Quint",
        TreasureType.RallyingStandard => "Rallying Standard",
        TreasureType.FishBag => "Fish Bag",
        _ => type.ToString()
    };
    
    public static string ToNamePlural(this TreasureType type) => type switch
    {
        TreasureType.BoxOfOres => "Boxes of Ores",
        TreasureType.CupOfLife => "Cups of Life",
        TreasureType.CrystallizedQuint => "Crystallized Quint",
        _ => $"{type.ToName()}s",
    };
    
    public static string ToArticle(this TreasureType type) => type switch
    {
        TreasureType.Ichor => "an",
        TreasureType.Emerald => "an",
        _ => "a",
    };
    
    public static string ToNameWithArticle(this TreasureType type) => $"{type.ToArticle()} {type.ToName()}";
}