using Microsoft.AspNetCore.Mvc;
using StarKindred.API.Entities;
using StarKindred.API.Services;
using StarKindred.Common.Services;

namespace StarKindred.API.Endpoints.Towns;

[ApiController]
public class ClearAllDecorations
{
    [HttpPost("/towns/my/decorations/clearAll")]
    public async Task<ApiResponse> _(
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        db.TownDecorations.RemoveRange(db.TownDecorations.Where(d => d.Town!.UserId == session.UserId));

        await db.SaveChangesAsync(cToken);

        return new();
    }
}