using FluentValidation;
using StarKindred.API.Utility;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Extensions;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.Accounts;

[ApiController]
public sealed class Search
{
    [HttpGet("/accounts")]
    public async Task<ApiResponse<PaginatedResults<AccountSummaryDto>>> _(
        CancellationToken cToken,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db,
        [FromQuery] Request query
    )
    {
        await currentUser.GetSessionOrThrow(cToken);

        var results = await db.Users
            .OrderBy(u => u.CreatedOn)
            .Search(query.Search)
            .Select(u => new AccountSummaryDto(u.Id, u.Name, null, null))
            .AsPaginatedResultsAsync(query.Page, 20, cToken);

        return new(results);
    }

    public sealed record Request(int Page = 1, string? Search = null)
    {
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Page).PageNumber();
            }
        }
    }
    public sealed record AccountSummaryDto(Guid Id, string Name, string? Icon, string? Title);
}

public static class UserExtensions
{
    public static IQueryable<User> Search(this IQueryable<User> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return query;

        var nameLike = $"%{search.Trim()}%";

        return query.Where(u => EF.Functions.Like(u.Name, nameLike));
    }
}