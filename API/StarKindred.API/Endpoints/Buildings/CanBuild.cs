using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility.Buildings;

namespace StarKindred.API.Endpoints.Buildings;

[ApiController]
public sealed class CanBuild
{
    [HttpGet("/buildings/canBuild")]
    public async Task<ApiResponse<Response>> _(
        [FromQuery] Request request,
        [FromServices] Db db, [FromServices] ICurrentUser currentUser, CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var buildingAlreadyExists = await db.Buildings
            .AnyAsync(r => r.Position == request.Position && r.UserId == session.UserId, cToken);

        if(buildingAlreadyExists)
            throw new UnprocessableEntity("A building has already been constructed there.");

        var buildingTypes = BuildingCosts.BuildingsAvailableAtPosition(request.Position);
        var options = buildingTypes.Select(BuildCost).ToList();

        return new(new(options));
    }

    private static Option BuildCost(BuildingType type)
    {
        return new(type, BuildingCosts.BuildCost[type]);
    }
    
    public sealed record Request(int Position)
    {
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Position).InclusiveBetween(1, 10).WithMessage("You must select a building position.");
            }
        }

    }

    public sealed record Response(List<Option> Options);
    public sealed record Option(BuildingType Building, List<ResourceQuantity> Cost);
}