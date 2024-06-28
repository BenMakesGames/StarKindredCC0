using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using StarKindred.API.Entities;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Accounts;

[ApiController]
public sealed class GetAvailableAvatars
{
    [HttpGet("/accounts/availableAvatars")]
    public async Task<ApiResponse<Response>> _(
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var available = await AvatarHelpers.GetAvailable(db, session.UserId, cToken);

        return new(new(available));
    }

    public sealed record Response(IList<string> Avatars);
}