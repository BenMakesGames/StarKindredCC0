using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Weapons;

[ApiController]
public sealed class LevelUp
{
    [HttpPost("/weapons/{weaponId:guid}/levelUp")]
    public async Task<ApiResponse<WeaponDto>> _(
        CancellationToken cToken,
        Guid weaponId,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var weapon = await db.Weapons.FirstOrDefaultAsync(w => w.Id == weaponId && w.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("Weapon does not exist.");
        
        var resourcesToLevel = WeaponHelper.ResourcesToLevelUp(weapon)
            ?? throw new UnprocessableEntity("That weapon cannot be leveled up.");

        var resources = await db.Resources
            .Where(r => r.UserId == session.UserId)
            .ToListAsync(cToken);

        ResourceHelper.PayOrThrow(resources, resourcesToLevel);

        weapon.Level++;

        await db.SaveChangesAsync(cToken);
        
        return new(new(
            weapon.Level,
            weapon.PrimaryBonus,
            weapon.Level >= 3 ? weapon.SecondaryBonus : null,
            WeaponHelper.ResourcesToLevelUp(weapon)
        ));
    }

    public sealed record WeaponDto(
        int Level,
        WeaponBonus PrimaryEffect,
        WeaponBonus? SecondaryEffect,
        List<ResourceQuantity>? ResourcesToLevelUp
    );
}