using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Accounts;

[ApiController]
public sealed class ChangeAppearance
{
    [HttpPost("/accounts/changeAppearance")]
    public async Task<ApiResponse> _(
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        [FromBody] Request request,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        var user = await db.Users.FirstAsync(u => u.Id == session.UserId, cToken);

        var available = await AvatarHelpers.GetAvailable(db, session.UserId, cToken);

        if (!available.Contains(request.Avatar))
            throw new UnprocessableEntity("Must select a Profile Picture.");

        user.Avatar = request.Avatar;

        PersonalLogHelper.Create(db, session.UserId, $"You changed your Profile Picture.", new[]
        {
            PersonalLogActivityType.AccountActivity,
            PersonalLogActivityType.UpdatedAvatarImage
        });

        await db.SaveChangesAsync(cToken);

        return new();
    }

    public sealed record Request(string Avatar);
}