using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.Vassals;

[ApiController]
public sealed class Favorite
{
    [HttpPost("/vassals/{vassalId:guid}/favorite")]
    public async Task<ApiResponse> _(
        Guid vassalId,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var vassal = await db.Vassals.FirstOrDefaultAsync(v => v.Id == vassalId && v.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("Vassal does not exist.");

        if (vassal.Favorite)
            return new();

        vassal.Favorite = true;

        await db.SaveChangesAsync(cToken);

        return new();
    }
}