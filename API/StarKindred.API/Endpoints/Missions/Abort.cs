using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Missions;

[ApiController]
public sealed class Abort
{
    [HttpPost("/missions/{id:guid}/abort")]
    public async Task<ApiResponse> _(
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db,
        CancellationToken cToken,
        Guid id
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var mission = await db.Missions
            .Include(m => m.Vassals!)
                .ThenInclude(v => v.Weapon)
            .AsSingleQuery() // TODO: not profiled
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("That mission does not exist.");

        if (mission.CreatedOn.AddMinutes(MissionMath.DurationInMinutes(mission.Type, 0, mission.Vassals!)) <= DateTimeOffset.UtcNow)
            throw new UnprocessableEntity("That mission is ready to complete! (No need to abort it!)");

        foreach (var v in mission.Vassals!)
            v.MissionId = null;

        db.Missions.Remove(mission);

        await db.SaveChangesAsync(cToken);

        return new ApiResponse();
    }
}