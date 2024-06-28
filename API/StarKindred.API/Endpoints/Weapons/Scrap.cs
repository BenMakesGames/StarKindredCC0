using BenMakesGames.RandomHelpers;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;
using StarKindred.Common.Entities;

namespace StarKindred.API.Endpoints.Weapons;

[ApiController]
public sealed class Scrap
{
    [HttpPost("/weapons/{weaponId:guid}/scrap")]
    public async Task<ApiResponse> _(
        CancellationToken cToken,
        Guid weaponId,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db,
        [FromServices] Random rng
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var hasScrappingI = await db.UserTechnologies
            .AnyAsync(ut => ut.UserId == session.UserId && ut.Technology == TechnologyType.ScrappingI, cToken);

        if (!hasScrappingI)
            throw new UnprocessableEntity("You do not have the required technology.");

        var weapon = await db.Weapons
            .Include(w => w.Vassal!)
                .ThenInclude(v => v.Leader)
            .FirstOrDefaultAsync(w => w.Id == weaponId && w.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("Weapon does not exist.");

        if(weapon.Vassal is { IsOnAMission: true })
            throw new UnprocessableEntity("Weapon is currently equipped by a Vassal that's busy with a task.");

        var resources = rng.Next(new ResourceQuantity[]
        {
            new(ResourceType.Quintessence, 10),
            new(ResourceType.Iron, 25),
            new(ResourceType.Wood, 50),
        });

        db.Weapons.Remove(weapon);

        await ResourceHelper.CollectResources(db, session.UserId, new() { resources }, cToken);

        await db.SaveChangesAsync(cToken);

        return new ApiResponse()
        {
            Messages = new()
            {
                ApiMessage.Success($"Scrapping the {weapon.Name} yielded {resources.Quantity} {resources.Type}.")
            }
        };
    }
}