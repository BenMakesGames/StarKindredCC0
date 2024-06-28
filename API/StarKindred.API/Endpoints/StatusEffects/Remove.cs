using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.StatusEffects;

[ApiController]
public sealed class Remove
{
    [HttpPost("/statusEffects/{id:guid}/remove")]
    public async Task<ApiResponse> _(
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db,
        CancellationToken cToken,
        Guid id
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var statusEffect = await db.StatusEffects
            .Include(se => se.Vassal!)
                .ThenInclude(v => v.Leader)
            .FirstOrDefaultAsync(se => se.Id == id && se.Vassal!.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("That status effect does not exist.");

        if (statusEffect.Vassal!.IsOnAMission)
            throw new UnprocessableEntity("You cannot use items on Vassals that are on a mission.");

        var ichor = await db.Treasures
            .Where(t => t.UserId == session.UserId && t.Type == TreasureType.Ichor)
            .ToListAsync(cToken);

        TreasureHelper.UseOrThrow(ichor, TreasureType.Ichor);
        
        db.StatusEffects.Remove(statusEffect);
        
        await db.SaveChangesAsync(cToken);
        
        return new ApiResponse();
    }
}