using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Vassals;

[ApiController]
public sealed class WillpowerOptions
{
    [HttpGet("/vassals/{vassalId:guid}/willpowerOptions")]
    public async Task<ApiResponse<Response>> _(
        CancellationToken cToken,
        Guid vassalId,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var vassal = await db.Vassals
            .Include(v => v.Mission)
            .Include(v => v.TimedMission)
            .Include(v => v.StatusEffects)
            .AsSingleQuery()
            .FirstOrDefaultAsync(v => v.Id == vassalId && v.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("There is no such Vassal.");

        var options = VassalMath.WillpowerOptions(vassal);

        return new(new(options));
    }

    public sealed record Response(List<WillpowerOption> Options);
}