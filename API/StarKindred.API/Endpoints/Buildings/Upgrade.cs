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
public sealed class Upgrade
{
    [HttpPost("/buildings/{buildingId:guid}/upgrade")]
    public async Task<ApiResponse> _(
        Guid buildingId,
        [FromServices] Db db, [FromServices] ICurrentUser currentUser, CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var building = await db.Buildings
            .FirstOrDefaultAsync(r => r.Id == buildingId && r.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("That building does not exist.");

        var technologies = await db.UserTechnologies
            .Where(t =>
                t.UserId == session.UserId && (
                    t.Technology == TechnologyType.ArchitectureI ||
                    t.Technology == TechnologyType.ArchitectureII ||
                    t.Technology == TechnologyType.Expansion
                )
            )
            .Select(t => t.Technology)
            .ToListAsync(cToken);

        var hasExpansion = technologies.Contains(TechnologyType.Expansion);

        if(building.Level >= BuildingCosts.MaxLevel(building.Type, hasExpansion))
            throw new UnprocessableEntity("This building cannot be leveled up any further.");
        
        var yield = BuildingHarvestMath.GetYield(building.Type, building.Level, building.LastHarvestedOn, new());
        
        if(yield.Resources.Any(r => r.Quantity > 0))
            throw new UnprocessableEntity("All resources must be collected before the building can be upgraded.");

        var hasArchitectureI = technologies.Contains(TechnologyType.ArchitectureI);
        var hasArchitectureII = technologies.Contains(TechnologyType.ArchitectureII);

        var cost = BuildingCosts.CostToUpgrade(building.Type, building.Level, hasArchitectureI, hasArchitectureII, hasExpansion)
            ?? throw new Exception("Building cost does not exist??");

        var costTypes = cost.Select(c => c.Type).ToList();
        
        var resources = await db.Resources
            .Where(r => r.UserId == session.UserId && costTypes.Contains(r.Type))
            .ToListAsync(cToken);

        ResourceHelper.PayOrThrow(resources, cost);

        PersonalLogHelper.Create(db, session.UserId, $"You leveled-up a {building.Type.GetDescription()} from level {building.Level} to {building.Level + 1}.", new[]
        {
            PersonalLogActivityType.Building,
            PersonalLogActivityType.LeveledUpBuilding
        });

        building.Level++;
        
        await db.SaveChangesAsync(cToken);

        return new ApiResponse();
    }
}