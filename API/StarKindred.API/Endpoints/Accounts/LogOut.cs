using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using StarKindred.API.Entities;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.Accounts;

[ApiController]
public sealed class LogOut
{
    [HttpPost("/accounts/logOut")]
    public async Task<ApiResponse> _(
        [FromServices] Db db, [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        await currentUser.ClearSessionOrThrow(cToken);
        
        await db.SaveChangesAsync(cToken);

        return new();
    }
}
