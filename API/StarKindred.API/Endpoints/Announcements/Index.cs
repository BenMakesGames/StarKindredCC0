using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Extensions;
using StarKindred.API.Services;
using StarKindred.API.Utility;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;

namespace StarKindred.API.Endpoints.Announcements;

[ApiController]
public sealed class Index
{
    public const int PageSize = 10;
    
    [HttpGet("/announcements")]
    public async Task<ApiResponse<PaginatedResults<AnnouncementDto>>> _(
        [FromQuery] Request request,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var results = await db.Announcements
            .WithType(request.Type)
            .OrderByDescending(a => a.CreatedOn)
            .Select(a => new AnnouncementDto(
                a.CreatedOn,
                a.Type,
                a.Body
            ))
            //.Cacheable(CacheExpirationMode.Absolute, TimeSpan.FromHours(1)) // TODO: when we have a Redis server, uncomment this line
            .AsPaginatedResultsAsync(request.Page, PageSize, cToken)
        ;

        if(await currentUser.GetSessionOrThrow(cToken) is { } session)
        {
            var lastViewed = await db.AnnouncementViews
                .FirstOrDefaultAsync(v => v.UserId == session.UserId, cToken);

            if (lastViewed == null)
            {
                db.AnnouncementViews.Add(new()
                {
                    UserId = session.UserId,
                    ViewedOn = DateTimeOffset.UtcNow
                });
            }
            else
            {
                lastViewed.ViewedOn = DateTimeOffset.UtcNow;
            }

            await db.SaveChangesAsync(cToken);
        }

        return new(results);
    }

    public sealed record Request(AnnouncementType? Type, int Page = 1)
    {
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Page).PageNumber();
            }
        }
    }

    public sealed record AnnouncementDto(DateTimeOffset CreatedOn, AnnouncementType Type, string Body);
}

public static class IQueryableExtensions
{
    public static IQueryable<Announcement> WithType(this IQueryable<Announcement> query, AnnouncementType? type) =>
        type == null
            ? query
            : query.Where(v => v.Type == type);
}