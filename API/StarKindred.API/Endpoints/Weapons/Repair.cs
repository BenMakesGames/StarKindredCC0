using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Weapons;

[ApiController]
public sealed class Repair
{
    [HttpPost("/weapons/{weaponId:guid}/repair")]
    public async Task<ApiResponse> _(
        CancellationToken cToken,
        [FromBody] Request request,
        Guid weaponId,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var weapon = await db.Weapons
            .Include(w => w.Vassal!)
                .ThenInclude(v => v.Leader)
            .FirstOrDefaultAsync(w => w.Id == weaponId && w.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("Weapon does not exist.");
        
        if(weapon.Durability >= weapon.MaxDurability)
            throw new UnprocessableEntity("Weapon is already fully repaired.");
        
        if(weapon.Vassal is { IsOnAMission: true })
            throw new UnprocessableEntity("Weapon is currently equipped by a Vassal that's busy with a task.");

        var weaponToSacrifice = await db.Weapons
            .Include(w => w.Vassal)
            .FirstOrDefaultAsync(w => w.Id == request.MaterialsId && w.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("Weapon to use for materials does not exist.");
        
        if(weaponToSacrifice.Vassal != null)
            throw new UnprocessableEntity("Weapon to use for materials is currently equipped by a Vassal.");
        
        if(weaponToSacrifice.PrimaryBonus != weapon.PrimaryBonus)
            throw new UnprocessableEntity("Weapon to use for materials must be the same type as the weapon being repaired.");
        
        weapon.Durability = Math.Min(weapon.MaxDurability, weapon.Durability + WeaponHelper.RepairValue(weaponToSacrifice));

        db.Weapons.Remove(weaponToSacrifice);

        await db.SaveChangesAsync(cToken);

        return new ApiResponse();
    }

    public sealed record Request(Guid MaterialsId);
}