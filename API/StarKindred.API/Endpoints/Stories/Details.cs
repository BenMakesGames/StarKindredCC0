using EFCoreSecondLevelCacheInterceptor;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace StarKindred.API.Endpoints.Stories;

[ApiController]
public sealed class Details
{
    [HttpGet("/stories/{adventureId:guid}")]
    public async Task<ApiResponse<AdventureDto>> _(
        Guid adventureId,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var adventure = await db.Adventures.FirstOrDefaultAsync(a => a.Id == adventureId, cToken)
            ?? throw new NotFoundException("Story does not exist.");
        
        var completed = await db.UserAdventureStepCompleted
            .Where(s => s.AdventureStep!.AdventureId == adventureId && s.UserId == session.UserId)
            .Select(s => new AdventureStepCompletedDto(s.AdventureStepId, s.AdventureStep!.Step, s.AdventureStep.X, s.AdventureStep.Y, s.AdventureStep!.Type))
            .ToListAsync(cToken)
        ;

        var completedSteps = completed.Select(s => s.Step).ToList();

        var availableSteps = await db.AdventureSteps
            .Where(a =>
                a.AdventureId == adventureId &&
                !completedSteps.Contains(a.Step) &&
                (
                    !a.PreviousStep.HasValue ||
                    completedSteps.Contains(a.PreviousStep.Value)
                )
            )
            .Select(a => new AvailableStepDto(a.Id, a.Step, a.X, a.Y, a.PinOverride, a.Type, a.Title, a.DurationInMinutes, a.MinVassals, a.MaxVassals, a.RequiredElement))
            .ToListAsync(cToken)
        ;

        var stepsInProgress = await db.UserAdventureStepInProgress
            .Where(s => s.AdventureStep!.AdventureId == adventureId && s.UserId == session.UserId)
            .Select(s => new AdventureStepInProgressDto(
                s.Id,
                s.AdventureStep!.Step,
                s.StartedOn.ToUnixTimeMilliseconds(),
                s.CompletesOn.ToUnixTimeMilliseconds(),
                s.Vassals!.Select(v => new VassalDto(v.Id, v.Level, v.Element, v.Species, v.Portrait)).ToList()
            ))
            .ToListAsync(cToken)
        ;

        var tags = await db.UserVassalTags
            .Where(t => t.UserId == session.UserId)
            .Select(t => new TagDto(t.Title, t.Color))
            .Cacheable(CacheExpirationMode.Sliding, TimeSpan.FromDays(1))
            .ToListAsync(cToken);

        return new(new(adventure.ReleaseYear, adventure.ReleaseMonth, adventure.IsDark, tags, completed, availableSteps, stepsInProgress));
    }

    public sealed record AdventureDto(int Year, int Month, bool IsDark, List<TagDto> Tags, List<AdventureStepCompletedDto> Completed, List<AvailableStepDto> AvailableSteps, List<AdventureStepInProgressDto> InProgress);
    public sealed record AdventureStepCompletedDto(Guid Id, int Step, float X, float Y, MissionType Type);
    public sealed record AvailableStepDto(Guid Id, int Step, float X, float Y, PinSide? PinOverride, MissionType Type, string Title, int DurationInMinutes, int MinVassals, int MaxVassals, Element? RequiredElement);
    public sealed record AdventureStepInProgressDto(Guid Id, int Step, long StartedOn, long CompletesOn, List<VassalDto> Vassals);
    public sealed record VassalDto(Guid Id, int Level, Element Element, Species Species, string Portrait);
    public sealed record TagDto(string Title, string Color);
}