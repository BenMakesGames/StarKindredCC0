using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Exceptions;

namespace StarKindred.API.Utility;

public static class TreasureHelper
{
    public static async Task FindAndUseQuantityOrThrow(
        Db db, Guid userId, TreasureType treasureType, int quantity, CancellationToken cToken
    )
    {
        var treasures = await db.Treasures
            .Where(t => t.Type == treasureType && t.UserId == userId && t.Quantity >= quantity)
            .FirstOrDefaultAsync(cToken)
            ?? throw new NotFoundException($"You don't have {quantity} {treasureType.ToName(quantity)}.");

        treasures.Quantity -= quantity;
    }

    public static void UseOrThrow(List<Treasure> treasures, TreasureType type)
    {
        var treasure = treasures.FirstOrDefault(r => r.Type == type);
        
        if(treasure == null || treasure.Quantity < 1)
            throw new UnprocessableEntity($"You don't have {type.ToNameWithArticle()} :(");

        treasure.Quantity--;
    }

    public static async Task CollectTreasure(Db db, Guid userId, TreasureType type, int quantity, CancellationToken cToken)
    {
        var treasure = await db.Treasures
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Type == type, cToken)
        ;

        if(treasure == null)
        {
            treasure = new Treasure()
            {
                UserId = userId,
                Type = type,
                Quantity = quantity
            };
            
            db.Treasures.Add(treasure);
        }
        else
        {
            treasure.Quantity += quantity;
        }
    }
}

public sealed record TreasureNames(string Name, string AName, string PluralName);