namespace StarKindred.Common.Entities;

public enum DecorationType
{
    // common/uncommon/rare decorations (add to `DecorationHelper`)
    Head,
    Pyramid,
    Torii,
    Watchtower,
    Windmill,
    Ship,
    LogPile,
    RedFlag,
    BlueFlag,
    WhiteFlag,
    PurpleFlag,
    BlackFlag,
    Rainbow,
    FalseAveries,
    MarbleHead,
    VanillaIceCream,
    ChocolateIceCream,
    VanillaIceCreamWithCherry,
    ChocolateIceCreamWithCherry,
    WoodenBridge,
    StoneBridge,
    PalaceTower,
    SwordInStone,

    // story/adventure decorations
    ShalurianLighthouse,
    SmallMushrooms,
    LargeMushroom,
    SkeletalRemains,
    EnormousTibia,
    PurpleGrass,

    // pasture decorations
    OneAnimal,
    TwoAnimals,
    ThreeAnimals,
    FenceNorthSouth,
    FenceEastWest,
}

public static class DecorationTypeExtensions
{
    public static string ToName(this DecorationType type) => type switch
    {
        DecorationType.LogPile => "Log Pile",
        DecorationType.RedFlag => "Red Flag",
        DecorationType.BlueFlag => "Blue Flag",
        DecorationType.WhiteFlag => "White Flag",
        DecorationType.PurpleFlag => "Purple Flag",
        DecorationType.BlackFlag => "Black Flag",
        DecorationType.FalseAveries => "False Averies",
        DecorationType.MarbleHead => "Marble Head",
        DecorationType.WoodenBridge => "Wooden Bridge",
        DecorationType.StoneBridge => "Stone Bridge",
        DecorationType.ShalurianLighthouse => "Shalurian Lighthouse",
        DecorationType.VanillaIceCream => "Vanilla Ice Cream",
        DecorationType.ChocolateIceCream => "Chocolate Ice Cream",
        DecorationType.VanillaIceCreamWithCherry => "Vanilla Ice Cream w/ Cherry",
        DecorationType.ChocolateIceCreamWithCherry => "Chocolate Ice Cream w/ Cherry",

        DecorationType.OneAnimal => "One Animal",
        DecorationType.TwoAnimals => "Two Animals",
        DecorationType.ThreeAnimals => "Three Animals",
        DecorationType.FenceEastWest => "Wood Fence (EW)",
        DecorationType.FenceNorthSouth => "Wood Fence (NS)",

        DecorationType.SmallMushrooms => "Small Mushrooms",
        DecorationType.LargeMushroom => "Large Mushroom",
        DecorationType.SkeletalRemains => "Skeletal Remains",

        _ => type.ToString()
    };

    public static string ToArticle(this DecorationType type) => type switch
    {
        DecorationType.FalseAveries or DecorationType.SmallMushrooms or DecorationType.SkeletalRemains => "some",
        DecorationType.OneAnimal => "",
        DecorationType.TwoAnimals => "",
        DecorationType.ThreeAnimals => "",
        _ => "a"
    };

    public static string ToNameWithArticle(this DecorationType type) => $"{type.ToArticle()} {type.ToName()}".Trim();
}
