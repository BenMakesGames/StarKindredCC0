using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility.Buildings;
using StarKindred.API.Utility.Technologies;

namespace StarKindred.API.Endpoints.Buildings;

[ApiController]
public sealed class GetAvailableRebuilds
{
    [HttpGet("/buildings/{buildingId:guid}/rebuild")]
    public async Task<ApiResponse<Response>> _(
        Guid buildingId,
        CancellationToken cToken,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var building = await db.Buildings
            .FirstOrDefaultAsync(b => b.Id == buildingId && b.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("That building does not exist.");

        var hasArchitectureIII = await TechTree.HasTechnology(db, session.UserId, TechnologyType.ArchitectureIII, cToken);

        var availableTypes = BuildingCosts.GetAvailableRebuildTypes(building.Type, building.Position, hasArchitectureIII);
        var newLevel = BuildingCosts.MaxLevel(building.Type, false) - 10;

        return new(new(availableTypes, newLevel));
    }

    public sealed record Response(List<BuildingType> Types, int NewLevel);
}