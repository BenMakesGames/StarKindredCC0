using BenMakesGames.RandomHelpers;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace StarKindred.API.Utility;

public static class DecorationHelper
{
    public static readonly DecorationType[] CommonDecorations =
    {
        DecorationType.Watchtower,
        DecorationType.LogPile,
        DecorationType.RedFlag,
        DecorationType.BlueFlag,
        DecorationType.WhiteFlag,
        DecorationType.PurpleFlag,
        DecorationType.BlackFlag,
    };

    public static readonly DecorationType[] UncommonDecorations =
    {
        DecorationType.Pyramid,
        DecorationType.Windmill,
        DecorationType.Ship,
        DecorationType.FalseAveries,
        DecorationType.VanillaIceCream,
        DecorationType.ChocolateIceCream,
        DecorationType.WoodenBridge,
        DecorationType.PalaceTower,
    };

    public static readonly DecorationType[] RareDecorations =
    {
        DecorationType.Rainbow,
        DecorationType.Torii,
        DecorationType.Head,
        DecorationType.MarbleHead,
        DecorationType.VanillaIceCreamWithCherry,
        DecorationType.ChocolateIceCreamWithCherry,
        DecorationType.StoneBridge,
        DecorationType.SwordInStone,
    };

    public static DecorationType GetRandomDecoration(Random rng) => rng.Next(100) switch
    {
        < 60 => rng.Next(CommonDecorations),
        < 90 => rng.Next(UncommonDecorations),
        _ => rng.Next(RareDecorations),
    };

    public static async Task<Decoration> CollectDecoration(Db db, Guid userId, DecorationType type, int quantity, CancellationToken cToken)
    {
        var decoration = await db.Decorations.FirstOrDefaultAsync(t => t.UserId == userId && t.Type == type, cToken);

        if(decoration == null)
        {
            decoration = new Decoration()
            {
                UserId = userId,
                Type = type,
                Quantity = quantity
            };

            db.Decorations.Add(decoration);
        }
        else
        {
            decoration.Quantity += quantity;
        }

        return decoration;
    }
}
