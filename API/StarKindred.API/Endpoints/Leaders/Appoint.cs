using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;

namespace StarKindred.API.Endpoints.Leaders;

[ApiController]
public sealed class Appoint
{
    [HttpPost("/leaders")]
    public async Task<ApiResponse> _(
        Request request,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var townIsLevel3 = await db.Towns.AnyAsync(t => t.UserId == session.UserId && t.Level >= 3, cToken);

        if(!townIsLevel3)
            throw new AccessDeniedException("You must complete more missions for The Oracle.");

        var vassal = await db.Vassals
            .Include(v => v.Leader)
            .FirstOrDefaultAsync(v => v.UserId == session.UserId && v.Id == request.VassalId, cToken)
            ?? throw new NotFoundException("Vassal not found.");

        if (vassal.IsOnAMission)
            throw new UnprocessableEntity("Vassal is on a mission.");

        if(vassal.Leader != null)
            throw new UnprocessableEntity("Vassal already holds a leadership position.");

        var positionAlreadyFilled = await db.TownLeaders.AnyAsync(
            l => l.UserId == session.UserId && l.Position == request.Position,
            cToken
        );

        if(positionAlreadyFilled)
            throw new UnprocessableEntity("Another Vassal already occupies that leadership position.");

        db.TownLeaders.Add(new()
        {
            UserId = session.UserId,
            Vassal = vassal,
            Position = request.Position
        });

        await db.SaveChangesAsync(cToken);

        return new();
    }

    public sealed record Request(Guid VassalId, TownLeaderPosition Position);
}