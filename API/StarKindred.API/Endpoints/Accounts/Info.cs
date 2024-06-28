using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Services;
using StarKindred.Common.Services;

namespace StarKindred.API.Endpoints.Accounts;

[ApiController]
public sealed class Info
{
    [HttpGet("/accounts/info")]
    public async Task<ApiResponse<ResponseDto>> _(
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        var user = await db.Users.FirstAsync(u => u.Id == session.UserId, cToken);

        return new(new(user.Id, user.Name, user.CreatedOn, user.Email));
    }

    public sealed record ResponseDto(Guid Id, string Name, DateTimeOffset SignUpDate, string EmailAddress);
}