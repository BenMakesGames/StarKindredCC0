using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.Vassals;

[ApiController]
public sealed class Equip
{
    [HttpPost("/vassals/{vassalId:guid}/equip")]
    public async Task<ApiResponse> _(
        CancellationToken cToken,
        Guid vassalId,
        [FromBody] Request request,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var vassal = await db.Vassals
            .Include(v => v.Leader)
            .FirstOrDefaultAsync(v => v.Id == vassalId && v.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("There is no such Vassal.");

        if (vassal.IsOnAMission)
            throw new UnprocessableEntity("You cannot change the equipment of a Vassal while they're busy with a task.");

        var weapon = await db.Weapons
            .Include(w => w.Vassal!)
                .ThenInclude(v => v.Leader)
            .FirstOrDefaultAsync(w => w.Id == request.WeaponId && w.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("There is no such weapon.");
        
        if(weapon.Vassal is { IsOnAMission: true })
            throw new UnprocessableEntity($"That weapon is equipped to {weapon.Vassal.Name}, but they're currently busy. You cannot change the equipment of a Vassal while they're busy with a task.");

        if(weapon.Durability == 0)
            throw new UnprocessableEntity("That weapon must be repaired before it can be equipped.");
        
        vassal.WeaponId = weapon.Id;

        await db.SaveChangesAsync(cToken);

        return new ApiResponse();
    }

    public sealed record Request(Guid WeaponId);
}