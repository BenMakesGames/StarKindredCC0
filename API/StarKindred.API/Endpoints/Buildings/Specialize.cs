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
public sealed class Specialize
{
    [HttpPost("/buildings/{buildingId:guid}/specialize")]
    public async Task<ApiResponse> _(
        Guid buildingId,
        [FromBody] Request request,
        [FromServices] Db db, [FromServices] ICurrentUser currentUser, CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var building = await db.Buildings
            .FirstOrDefaultAsync(r => r.Id == buildingId && r.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("That building does not exist.");

        var hasArchitectureIII = await db.UserTechnologies.AnyAsync(t => t.UserId == session.UserId && t.Technology == TechnologyType.ArchitectureIII, cToken);

        var options = hasArchitectureIII
            ? BuildingCosts.BuildingsAvailableAtPosition(building.Position).SelectMany(BuildingHarvestMath.GetAvailableSpecializations).Except(new[] { building.Type }).ToList()
            : BuildingHarvestMath.GetAvailableSpecializations(building.Type);
        
        if(options.Count == 0 || building.Level != 10)
            throw new UnprocessableEntity("This building cannot be specialized.");

        if(!options.Contains(request.Type))
            throw new UnprocessableEntity("Cannot specialize this building that way.");

        var yield = BuildingHarvestMath.GetYield(building.Type, building.Level, building.LastHarvestedOn, new());

        if(yield.Resources.Any(r => r.Quantity > 0))
            throw new UnprocessableEntity("All resources must be collected before the building can be specialized.");

        var magicHammers = await db.Treasures
            .Where(t => t.UserId == session.UserId && t.Type == TreasureType.MagicHammer)
            .ToListAsync(cToken);

        TreasureHelper.UseOrThrow(magicHammers, TreasureType.MagicHammer);

        PersonalLogHelper.Create(db, session.UserId, $"You specialized a {building.Type.GetDescription()} into a {request.Type.GetDescription()}.", new[]
        {
            PersonalLogActivityType.Building,
            PersonalLogActivityType.SpecializedBuilding
        });

        building.Type = request.Type;

        await db.SaveChangesAsync(cToken);

        return new ApiResponse();
    }

    public sealed record Request(BuildingType Type);
}