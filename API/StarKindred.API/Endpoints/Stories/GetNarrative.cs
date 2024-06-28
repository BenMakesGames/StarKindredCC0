using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.Stories;

[ApiController]
public sealed class GetNarrative
{
    [HttpGet("/stories/{storyId:guid}/narrative")]
    public async Task<ApiResponse<ResponseDto>> _(
        Guid storyId,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var completedStory = await db.UserAdventureStepCompleted
            .Include(ua => ua.AdventureStep)
            .AsSingleQuery() // TODO: not profiled
            .FirstOrDefaultAsync(ua =>
                ua.AdventureStepId == storyId &&
                ua.AdventureStep!.Type == MissionType.Story &&
                ua.UserId == session.UserId,
                cToken
            )
            ?? throw new NotFoundException("You have not completed that story.");

        return new(new(completedStory.AdventureStep!.Narrative!));
    }

    public sealed record ResponseDto(string Narrative);
}