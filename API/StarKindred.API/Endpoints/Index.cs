using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Services;
using StarKindred.Common.Services;

namespace StarKindred.API.Endpoints;

[ApiController]
public sealed class Index
{
    [HttpGet("/")]
    public async Task<ApiResponse> _(
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db,
        [FromServices] IAddressHelper addressHelper,
        CancellationToken cToken
    )
    {
        try
        {
            var session = await currentUser.GetSessionOrThrow(cToken);
            var user = await db.Users.FirstAsync(u => u.Id == session.UserId, cToken);

            return new()
            {
                Messages = new() { ApiMessage.Info($"Hi, {user.Name}! :D\n\nTo play the game, visit {addressHelper.ClientUrl}") }
            };
        }
        catch(Exception)
        {
            return new()
            {
                Messages = new() { ApiMessage.Info($"Hi! :D\n\nTo play the game, visit {addressHelper.ClientUrl}") }
            };
        }
    }
}