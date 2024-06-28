using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Missions;

[ApiController]
public sealed class Start
{
    [HttpPost("/missions")]
    public async Task<ApiResponse> _(
        CancellationToken cToken,
        Request request,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var vassalCount = await db.Vassals.CountAsync(v => v.UserId == session.UserId, cToken);

        var highestVassalLevel = await db.Vassals
            .Where(v => v.UserId == session.UserId)
            .MaxAsync(v => v.Level, cToken);

        var availableMissionTypes = MissionMath.AvailableMissions(vassalCount, highestVassalLevel);

        if(!availableMissionTypes.Contains(request.Mission))
            throw new UnprocessableEntity("That mission is not yet available.");

        var minVassals = MissionMath.MinVassals(request.Mission, 0);
        var maxVassals = MissionMath.MaxVassals(request.Mission, 0);

        if(request.Vassals.Count < minVassals)
            throw new UnprocessableEntity($"A minimum of {maxVassals} are required for this mission.");

        if(request.Vassals.Count > maxVassals)
            throw new UnprocessableEntity($"No more than {maxVassals} may go on this mission.");

        var hasExistingMission = await db.Missions.AnyAsync(
            m => m.Type == request.Mission && m.UserId == session.UserId,
            cToken
        );

        if(hasExistingMission)
            throw new UnprocessableEntity("You already have Vassals on this mission.");
        
        var vassals = await db.Vassals
            .Include(v => v.StatusEffects)
            .Include(v => v.Leader)
            .AsSingleQuery() // TODO: not profiled
            .Where(v => request.Vassals.Contains(v.Id) && v.UserId == session.UserId)
            .ToListAsync(cToken);

        if(vassals.Count != request.Vassals.Count)
            throw new UnprocessableEntity("One or more of the selected Vassals could not be found...");
        
        if(vassals.Any(v => v.IsOnAMission || v.Leader != null))
            throw new UnprocessableEntity("One or more of the selected Vassals is busy with another task.");

        MissionMath.ValidateVassalStatusEffects(request.Mission, vassals);
        
        var mission = new Mission()
        {
            UserId = session.UserId,
            Type = request.Mission,
            Vassals = vassals
        };

        db.Add(mission);

        await db.SaveChangesAsync(cToken);
        
        return new ApiResponse();
    }

    public sealed record Request(MissionType Mission, List<Guid> Vassals)
    {
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Vassals.Count)
                    .GreaterThan(0)
                    .WithMessage("Must select at least one Vassal.")
                ;
            }
        }
    }
}