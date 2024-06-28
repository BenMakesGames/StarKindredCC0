using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.TimedMissions;

[ApiController]
public sealed class Abort
{
    [HttpPost("/timedMissions/{id:guid}/abort")]
    public async Task<ApiResponse> _(
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db,
        CancellationToken cToken,
        Guid id
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var timedMission = await db.TimedMissions
            .Include(m => m.Vassals)
            .AsSplitQuery() // TODO: not profiled
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("That mission does not exist.");

        if(timedMission.StartedOn == null || timedMission.CompletesOn == null)
            throw new NotFoundException("That mission has not been started.");
        
        if (timedMission.CompletesOn < DateTimeOffset.UtcNow)
            throw new UnprocessableEntity("That mission is ready to complete! (No need to abort it!)");

        foreach (var v in timedMission.Vassals!)
            v.TimedMissionId = null;

        timedMission.StartedOn = null;
        timedMission.CompletesOn = null;
        
        await db.SaveChangesAsync(cToken);
        
        return new ApiResponse();
    }
}