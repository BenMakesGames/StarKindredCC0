using FluentValidation;
using StarKindred.API.Utility;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using StarKindred.API.Entities;
using StarKindred.API.Extensions;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.Stories;

[ApiController]
public sealed class Search
{
    [HttpGet("/stories")]
    public async Task<ApiResponse<PaginatedResults<Response>>> _(
        [FromQuery] Request request,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var results = await db.Adventures
            .OrderBy(a => a.AdventureSteps!.Count(s => s.UserAdventureStepsCompleted!.Any(u => u.UserId == session.UserId)) == a.AdventureSteps!.Count)
                .ThenBy(a => a.ReleaseNumber)
            .Select(a => new Response(
                a.Id,
                a.ReleaseYear,
                a.ReleaseMonth,
                a.Title,
                a.Summary,
                a.AdventureSteps!.Count,
                a.AdventureSteps!.Count(s => s.UserAdventureStepsCompleted!.Any(u => u.UserId == session.UserId))
            ))
            .AsPaginatedResultsAsync(request.Page, 12, cToken)
        ;

        return new(results);
    }

    public sealed record Request(int Page = 1)
    {
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Page).PageNumber();
            }
        }
    }

    public sealed record Response(Guid Id, int Year, int Month, string Title, string Summary, int MissionsAvailable, int MissionsComplete);
}