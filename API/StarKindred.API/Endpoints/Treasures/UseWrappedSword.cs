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
public sealed class UseWrappedSword
{
    [HttpPost("treasures/use/wrappedSword")]
    public async Task<ApiResponse> _(
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Random rng,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        
        var wrappedSwords = await db.Treasures
            .Where(t => t.Type == TreasureType.WrappedSword && t.UserId == session.UserId && t.Quantity > 0)
            .ToListAsync(cToken)
            ?? throw new NotFoundException("You don't have any Wrapped Swords.");

        TreasureHelper.UseOrThrow(wrappedSwords, TreasureType.WrappedSword);

        var weapon = WeaponHelper.CollectWeapon(db, rng, session.UserId, WeaponBonus.HuntingLevels);

        weapon.Level = 3;

        await db.SaveChangesAsync(cToken);

        return new()
        {
            Messages = new() { ApiMessage.Info($"You received a Level 3 {weapon.Name}.") },
        };
    }
}