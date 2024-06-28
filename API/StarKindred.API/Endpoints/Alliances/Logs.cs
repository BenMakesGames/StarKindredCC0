using FluentValidation;
using StarKindred.API.Utility;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Extensions;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.Alliances;

[ApiController]
public sealed class Logs
{
    [HttpGet("/alliances/my/logs")]
    public async Task<ApiResponse<PaginatedResults<LogDto>>> _(
        [FromQuery] Search search,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var membership = await db.UserAlliances.FirstOrDefaultAsync(ua => ua.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("You are not in an Alliance.");

        var results = await db.AllianceLogs
            .OrderByDescending(l => l.CreatedOn)
            .Where(l => l.AllianceId == membership.AllianceId)
            .Select(l => new LogDto(l.CreatedOn, l.ActivityType, l.Message))
            .AsPaginatedResultsAsync(search.Page, 20, cToken);

        return new(results);
    }

    public sealed record Search(int Page = 1)
    {
        public sealed class Validator : AbstractValidator<Search>
        {
            public Validator()
            {
                RuleFor(x => x.Page).PageNumber();
            }
        }
    }

    public sealed record LogDto(DateTimeOffset CreatedOn, AllianceLogActivityType ActivityType, string Message);
}