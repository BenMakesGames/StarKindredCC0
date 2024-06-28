using FluentValidation;
using StarKindred.API.Utility;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using StarKindred.API.Entities;
using StarKindred.API.Extensions;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.Accounts;

[ApiController]
public sealed class Logs
{
    [HttpGet("accounts/logs")]
    public async Task<ApiResponse<PaginatedResults<LogDto>>> _(
        [FromQuery] Request request,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        
        var results = await db.PersonalLogs
            .Where(l => l.UserId == session.UserId)
            .OrderByDescending(l => l.CreatedOn)
            .Select(l => new LogDto(
                l.Message,
                l.PersonalLogTags!.Select(t => t.Tag).ToArray(),
                l.CreatedOn
            ))
            .AsPaginatedResultsAsync(request.Page, 20, cToken);

        return new ApiResponse<PaginatedResults<LogDto>>(results);
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

    public sealed record LogDto(string Message, PersonalLogActivityType[] Tags, DateTimeOffset Date);
}