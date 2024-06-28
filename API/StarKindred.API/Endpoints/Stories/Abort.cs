using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace StarKindred.API.Endpoints.Stories;

[ApiController]
public sealed class Abort
{
    [HttpPost("/stories/{id:guid}/abort")]
    public async Task<ApiResponse> _(
        Guid id,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var progress = await db.UserAdventureStepInProgress
            .Include(a => a.AdventureStep)
            .Include(a => a.Vassals)
            .AsSingleQuery() // TODO: not profiled
            .FirstOrDefaultAsync(a => a.UserId == session.UserId && a.Id == id, cToken)
            ?? throw new NotFoundException("Adventure not found")
        ;

        var now = DateTimeOffset.UtcNow;

        if(progress.CompletesOn < now)
            throw new UnprocessableEntity("Adventure has been completed; it cannot be aborted");

        foreach (var v in progress.Vassals!)
            v.UserAdventureStepInProgressId = null;

        db.UserAdventureStepInProgress.Remove(progress);

        await db.SaveChangesAsync(cToken);

        return new();
    }
}