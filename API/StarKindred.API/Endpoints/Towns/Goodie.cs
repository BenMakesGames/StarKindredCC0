using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Towns;

[ApiController]
public sealed class Goodie
{
    [HttpPost("/towns/my/goodies/{location:int}")]
    public async Task<ApiResponse> _(
        int location,
        CancellationToken cToken,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        
        var goodie = await db.Goodies.FirstOrDefaultAsync(g => g.UserId == session.UserId && g.Location == location, cToken)
            ?? throw new NotFoundException("There is no such goodie to collect.");

        var gains = new List<ResourceQuantity>()
        {
            new(goodie.Type, goodie.Quantity)
        };

        db.Goodies.Remove(goodie);
        
        await ResourceHelper.CollectResources(db, session.UserId, gains, cToken);

        await db.SaveChangesAsync(cToken);

        return new ApiResponse();
    }
}