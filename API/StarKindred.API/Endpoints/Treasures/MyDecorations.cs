using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.Treasures;

[ApiController]
public sealed class MyDecorations
{
    [HttpGet("/treasures/my/decorations")]
    public async Task<ApiResponse<Response>> _(
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var decorations = await db.Decorations
            .Where(d => d.UserId == session.UserId)
            .Select(d => new DecorationDto(d.Type, d.Quantity))
            .ToListAsync(cToken);
        
        return new(new(decorations));
    }

    public sealed record Response(List<DecorationDto> Decorations);
    public sealed record DecorationDto(DecorationType Type, int Quantity);
}