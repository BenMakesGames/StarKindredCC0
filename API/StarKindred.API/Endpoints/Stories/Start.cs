using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Stories;

[ApiController]
public sealed class Start
{
    [HttpPost("/stories/{stepId:guid}/start")]
    public async Task<ApiResponse> _(
        Guid stepId,
        RequestDto request,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var storyStep = await db.AdventureSteps.FirstOrDefaultAsync(s => s.Id == stepId, cToken)
            ?? throw new NotFoundException("That story step does not exist.");

        // if the selected story has a prereq, ensure the player has completed it
        if (storyStep.PreviousStep != null)
        {
            if(!await db.UserAdventureStepCompleted
                .AnyAsync(c => c.UserId == session.UserId && c.AdventureStep!.Step == storyStep.PreviousStep, cToken))
            {
                throw new UnprocessableEntity("You must complete the previous step before starting this step.");
            }
        }

        // ensure the player hasn't already started this story!
        if(await db.UserAdventureStepInProgress.AnyAsync(c => c.UserId == session.UserId, cToken))
        {
            throw new UnprocessableEntity("A story step is already in progress! (You can only work on one at a time.)");
        }

        if(request.VassalIds.Count < storyStep.MinVassals)
            throw new UnprocessableEntity($"At least {storyStep.MinVassals} Vassals are required for this story.");

        if(request.VassalIds.Count > storyStep.MaxVassals)
            throw new UnprocessableEntity($"At most {storyStep.MaxVassals} Vassals are allowed for this story.");

        var vassals = await db.Vassals
            .Include(v => v.StatusEffects)
            .Include(v => v.Weapon)
            .Include(v => v.Leader)
            .AsSplitQuery() // TODO: not profiled
            .Where(v => request.VassalIds.Contains(v.Id) && v.UserId == session.UserId)
            .ToListAsync(cToken);

        if(vassals.Count != request.VassalIds.Count)
            throw new UnprocessableEntity("One or more of the Vassals you selected does not exist.");

        if (storyStep.RequiredElement is Element requiredElement)
        {
            if(!vassals.Any(v => v.Element == requiredElement))
                throw new UnprocessableEntity($"At least one Vassal must be {requiredElement}-type.");
        }

        if(vassals.Any(v => v.IsOnAMission || v.Leader != null))
            throw new UnprocessableEntity("One or more of the selected Vassals is busy with another task.");

        MissionMath.ValidateVassalStatusEffects(storyStep.Type, vassals);

        var durationInMinutes = MissionMath.DurationInMinutes(storyStep.DurationInMinutes, vassals);

        db.UserAdventureStepInProgress.Add(new()
        {
            AdventureStep = storyStep,
            UserId = session.UserId,
            Vassals = vassals,
            CompletesOn = DateTimeOffset.UtcNow.AddMinutes(durationInMinutes)
        });

        await db.SaveChangesAsync(cToken);

        return new();
    }

    public sealed record RequestDto(List<Guid> VassalIds);
}