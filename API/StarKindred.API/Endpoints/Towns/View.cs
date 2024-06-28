using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;

namespace StarKindred.API.Endpoints.Towns;

[ApiController]
public sealed class View
{
    [HttpGet("/towns/view/{id:guid}")]
    public async Task<ApiResponse<Response>> Get(
        Guid id,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        currentUser.GetSessionIdOrThrow();
        
        var town = await db.Towns
            .Include(t => t.Decorations)
            .FirstOrDefaultAsync(t => t.UserId == id, cToken)
            ?? throw new NotFoundException("There is no such town!");
        
        var buildingEntities = await db.Buildings
            .Where(b => b.UserId == id)
            .ToListAsync(cToken);

        var buildings = buildingEntities
            .Select(b => new BuildingDto(
                b.Position,
                b.Type,
                b.Level
            ))
            .ToList()
        ;

        return new(new Response(
            buildings,
            town.Decorations!.Select(d => new DecorationDto(d.Type, d.X, d.Y, d.Scale, d.FlipX)).ToList()
        ));
    }

    public sealed record Response(List<BuildingDto> Buildings, List<DecorationDto> Decorations);
    public sealed record BuildingDto(int Position, BuildingType Type, int Level);
    public sealed record DecorationDto(DecorationType Type, float X, float Y, int Scale, bool FlipX);
}