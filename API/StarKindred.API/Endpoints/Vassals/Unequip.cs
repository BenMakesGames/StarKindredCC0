using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.Vassals;

[ApiController]
public sealed class Unequip
{
    [HttpPost("/vassals/{vassalId:guid}/unequip")]
    public async Task<ApiResponse> _(
        CancellationToken cToken,
        Guid vassalId,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var vassal = await db.Vassals
            .Include(v => v.Leader)
            .FirstOrDefaultAsync(v => v.Id == vassalId && v.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("There is no such Vassal.");

        if (vassal.IsOnAMission)
            throw new UnprocessableEntity("You cannot change the equipment of a Vassal while they're busy with a task.");

        vassal.WeaponId = null;

        await db.SaveChangesAsync(cToken);

        return new ApiResponse();
    }
}