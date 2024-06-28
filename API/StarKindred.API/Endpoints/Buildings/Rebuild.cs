using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;
using StarKindred.API.Utility.Buildings;

namespace StarKindred.API.Endpoints.Buildings;

[ApiController]
public sealed class Rebuild
{
    [HttpPost("/buildings/{buildingId:guid}/rebuild")]
    public async Task<ApiResponse> _(
        Guid buildingId,
        CancellationToken cToken,
        [FromBody] Request request,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var building = await db.Buildings
            .FirstOrDefaultAsync(b => b.Id == buildingId && b.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("That building does not exist.");

        var newLevel = BuildingCosts.MaxLevel(building.Type, false) - 10;

        var hasArchitectureIII = await db.UserTechnologies.AnyAsync(t => t.UserId == session.UserId && t.Technology == TechnologyType.ArchitectureIII, cToken);

        var availableTypes = BuildingCosts.GetAvailableRebuildTypes(building.Type, building.Position, hasArchitectureIII);

        if(!availableTypes.Contains(request.Type))
            throw new UnprocessableEntity("That type is not available to rebuild into.");

        var magicHammers = await db.Treasures
            .Where(t => t.UserId == session.UserId && t.Type == TreasureType.MagicHammer)
            .ToListAsync(cToken);

        TreasureHelper.UseOrThrow(magicHammers, TreasureType.MagicHammer);

        PersonalLogHelper.Create(db, session.UserId, $"You rebuilt a {building.Type.GetDescription()} into a {request.Type.GetDescription()}.", new[]
        {
            PersonalLogActivityType.Building,
            PersonalLogActivityType.RebuildBuilding
        });

        building.Type = request.Type;
        building.Level = newLevel;

        await db.SaveChangesAsync(cToken);

        return new ApiResponse();
    }

    public sealed record Request(BuildingType Type);
}