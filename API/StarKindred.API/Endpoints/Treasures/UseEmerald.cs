using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Treasures;

[ApiController]
public sealed class UseEmerald
{
    [HttpPost("treasures/use/emerald")]
    public async Task<ApiResponse> _(
        [FromBody] RequestDto request,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Random rng,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        
        var rubyChests = await db.Treasures
            .Where(t => t.Type == TreasureType.Emerald && t.UserId == session.UserId && t.Quantity > 0)
            .ToListAsync(cToken)
            ?? throw new NotFoundException("You don't have any Emeralds.");

        TreasureHelper.UseOrThrow(rubyChests, TreasureType.Emerald);

        if (request.Choice == Choice.Wand)
        {
            WeaponHelper.CollectWeapon(db, rng, session.UserId, WeaponBonus.GoldGetsWine);
        }
        else
        {
            var gains = request.Choice switch
            {
                Choice.Wine => new ResourceQuantity(ResourceType.Wine, 300),
                Choice.Gold => new ResourceQuantity(ResourceType.Gold, 200),
                Choice.Quintessence => new ResourceQuantity(ResourceType.Quintessence, 150),
                _ => throw new UnprocessableEntity("Invalid choice."),
            };
        
            await ResourceHelper.CollectResources(db, session.UserId, new List<ResourceQuantity>() { gains }, cToken);
        }

        await db.SaveChangesAsync(cToken);

        return new();
    }

    public sealed record RequestDto(Choice Choice);
    public enum Choice { Wine, Gold, Quintessence, Wand };
}