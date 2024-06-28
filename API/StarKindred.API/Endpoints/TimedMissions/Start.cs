using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.TimedMissions;

[ApiController]
public sealed class Start
{
    [HttpPost("/timedMissions")]
    public async Task<ApiResponse> _(
        CancellationToken cToken,
        Request request,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        if(request.Vassals.Count < 1)
            throw new UnprocessableEntity("Must select at least one Vassal.");

        var timedMission = await db.TimedMissions.FirstOrDefaultAsync(m => m.Id == request.Id, cToken)
            ?? throw new NotFoundException("There is no such mission.");

        var minVassals = MissionMath.MinVassals(timedMission.Type, timedMission.Level);
        var maxVassals = MissionMath.MaxVassals(timedMission.Type, timedMission.Level);

        if(request.Vassals.Count < minVassals)
            throw new UnprocessableEntity($"A minimum of {maxVassals} are required for this mission.");

        if(request.Vassals.Count > maxVassals)
            throw new UnprocessableEntity($"No more than {maxVassals} may go on this mission.");

        var vassals = await db.Vassals
            .Include(v => v.StatusEffects)
            .Include(v => v.Weapon)
            .Include(v => v.Leader)
            .AsSplitQuery()
            .Where(v => request.Vassals.Contains(v.Id) && v.UserId == session.UserId)
            .ToListAsync(cToken);

        if(vassals.Count != request.Vassals.Count)
            throw new UnprocessableEntity("One or more of the selected Vassals could not be found...");

        if(vassals.Any(v => v.IsOnAMission || v.Leader != null))
            throw new UnprocessableEntity("One or more of the selected Vassals is busy with another task.");

        MissionMath.ValidateVassalStatusEffects(timedMission.Type, vassals);

        timedMission.Vassals = vassals;
        timedMission.StartedOn = DateTimeOffset.UtcNow;

        timedMission.CompletesOn = timedMission.StartedOn.Value.AddMinutes(MissionMath.DurationInMinutes(timedMission.Type, timedMission.Level, vassals));

        await db.SaveChangesAsync(cToken);
        
        return new ApiResponse();
    }

    public sealed record Request(Guid Id, List<Guid> Vassals);
}