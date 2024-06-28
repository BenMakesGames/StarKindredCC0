using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using StarKindred.API.Entities;
using StarKindred.API.Extensions;
using StarKindred.API.Services;
using StarKindred.API.Utility;
using StarKindred.Common.Services;

namespace StarKindred.API.Endpoints.Accounts.Subscriptions;

[ApiController]
public sealed class Get
{
    private const int PageSize = 12;

    [HttpGet("/accounts/subscriptions")]
    public async Task<ApiResponse<PaginatedResults<SubscriptionDto>>> _(
        [FromQuery] Request request,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var results = await db.UserSubscriptions
            .Where(s => s.UserId == session.UserId)
            .OrderByDescending(s => s.EndDate)
            .Select(s => new SubscriptionDto(
                s.SubscriptionService,
                s.StartDate,
                s.EndDate
            ))
            .AsPaginatedResultsAsync(request.Page, PageSize, cToken);

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

    public sealed record SubscriptionDto(string SubscriptionService, DateTimeOffset StartDate, DateTimeOffset EndDate);
}