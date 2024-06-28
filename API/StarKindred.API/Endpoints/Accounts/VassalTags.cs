using EFCoreSecondLevelCacheInterceptor;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.Accounts;

[ApiController]
public sealed class VassalTags
{
    [HttpGet("/accounts/tags")]
    public async Task<ApiResponse<ResponseDto>> _(
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var tags = await db.UserVassalTags
            .Where(t => t.UserId == session.UserId)
            .Select(t => new TagDto(t.Title, t.Color, t.Vassals!.Count))
            .Cacheable(CacheExpirationMode.Sliding, TimeSpan.FromDays(1))
            .ToListAsync(cToken);

        return new(new(tags));
    }

    public sealed record ResponseDto(List<TagDto> Tags);
    public sealed record TagDto(string Title, string Color, int VassalCount);
}