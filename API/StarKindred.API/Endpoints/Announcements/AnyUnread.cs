using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Services;
using StarKindred.Common.Services;

namespace StarKindred.API.Endpoints.Announcements;

[ApiController]
public class AnyUnread
{
    [HttpGet("announcements/anyUnread")]
    public async Task<ApiResponse<Response>> _(
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        var viewed = await db.AnnouncementViews.FirstOrDefaultAsync(v => v.UserId == session.UserId, cToken);

        if (viewed == null)
            return new(new(true));

        var latestAnnouncement = await db.Announcements
            .OrderByDescending(a => a.CreatedOn)
            .Select(a => new { CreatedOn = a.CreatedOn })
            .FirstOrDefaultAsync(cToken);

        var anyNew = latestAnnouncement != null && latestAnnouncement.CreatedOn > viewed.ViewedOn;

        return new(new(anyNew));
    }

    public record Response(bool UnreadAnnouncements);
}