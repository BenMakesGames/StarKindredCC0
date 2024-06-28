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
public sealed class UseRallyingStandard
{
    [HttpPost("treasures/use/rallyingStandard")]
    public async Task<ApiResponse> _(
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        var user = await db.Users.FirstAsync(u => u.Id == session.UserId, cToken);

        var rallyingStandards = await db.Treasures
            .Where(t => t.Type == TreasureType.RallyingStandard && t.UserId == session.UserId && t.Quantity > 0)
            .ToListAsync(cToken)
            ?? throw new NotFoundException("You don't have any Rallying Standards.");

        var now = DateTimeOffset.UtcNow;

        if(user.LastUsedRallyingStandard >= now.Date)
            throw new UnprocessableEntity("You can only use a Rallying Standard once per day.");

        if(user.LastAttackedGiant.Date < now.Date)
            throw new UnprocessableEntity("You haven't attacked the Giant yet, today.");

        TreasureHelper.UseOrThrow(rallyingStandards, TreasureType.RallyingStandard);

        user.LastAttackedGiant = now.AddDays(-1).Date;
        user.LastUsedRallyingStandard = now.Date;

        await db.SaveChangesAsync(cToken);

        return new();
    }
}