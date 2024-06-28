using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility.Buildings;

namespace StarKindred.API.Endpoints.Buildings;

[ApiController]
public sealed class Harvest
{
    [HttpPost("/buildings/{buildingId:guid}/harvest")]
    public async Task<ApiResponse<Response>> _(
        Guid buildingId,
        [FromServices] Db db, [FromServices] ICurrentUser currentUser, CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var building = await db.Buildings
            .FirstOrDefaultAsync(r => r.Id == buildingId && r.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("That building does not exist.");

        var technologies = await db.UserTechnologies
            .Where(t => t.UserId == session.UserId)
            .Select(t => t.Technology)
            .ToListAsync(cToken);

        var yield = BuildingHarvestMath.GetYield(building.Type, building.Level, building.LastHarvestedOn, technologies);
        
        if(!yield.Resources.Any(r => r.Quantity > 0))
            throw new UnprocessableEntity("That building has no resources to harvest.");

        building.LastHarvestedOn = building.LastHarvestedOn.AddMinutes(yield.MinutesConsumed);

        var newQuantities = new List<ResourceDto>();
        
        foreach(var y in yield.Resources)
        {
            var resource = await db.Resources
                .FirstOrDefaultAsync(r => r.Type == y.Type && r.UserId == session.UserId, cToken);

            if(resource == null)
            {
                resource = new()
                {
                    UserId = session.UserId,
                    Type = y.Type
                };

                db.Resources.Add(resource);
            }

            resource.Quantity += y.Quantity;
            
            newQuantities.Add(new(resource.Type, resource.Quantity));
        }

        await db.SaveChangesAsync(cToken);

        return new(new(newQuantities));
    }

    public sealed record Response(List<ResourceDto> Resources);
    public sealed record ResourceDto(ResourceType Type, int NewQuantity);
}