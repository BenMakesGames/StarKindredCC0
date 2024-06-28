using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;
using StarKindred.API.Utility.Buildings;

namespace StarKindred.API.Endpoints.Buildings;

[ApiController]
public sealed class Build
{
    [HttpPost("/buildings/build")]
    public async Task<ApiResponse> _(
        Request request,
        [FromServices] Db db, [FromServices] ICurrentUser currentUser, CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var town = await db.Towns.FirstAsync(t => t.UserId == session.UserId, cToken);

        if (town.Level < TownHelpers.LevelRequiredForPosition[request.Position])
            throw new UnprocessableEntity("Cannot build there, yet.");
        
        var buildingAlreadyExists = await db.Buildings
            .AnyAsync(r => r.Position == request.Position && r.UserId == session.UserId, cToken);

        if(buildingAlreadyExists)
            throw new UnprocessableEntity("A building has already been constructed there.");

        var buildingTypesAvailable = BuildingCosts.BuildingsAvailableAtPosition(request.Position);

        if (!buildingTypesAvailable.Contains(request.Building))
            throw new UnprocessableEntity($"Cannot build a {request.Building} here.");

        var buildingCost = BuildingCosts.BuildCost[request.Building];

        var resources = await db.Resources
            .Where(r => r.UserId == session.UserId)
            .ToListAsync(cToken);

        ResourceHelper.PayOrThrow(resources, buildingCost);

        db.Buildings.Add(new Building()
        {
            UserId = session.UserId,
            Position = request.Position,
            Type = request.Building,
        });

        PersonalLogHelper.Create(db, session.UserId, $"You built a {request.Building.GetDescription()}.", new[]
        {
            PersonalLogActivityType.Building,
            PersonalLogActivityType.BuildBuilding
        });

        await db.SaveChangesAsync(cToken);

        return new ApiResponse();
    }

    public sealed record Request(int Position, BuildingType Building)
    {
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Position).InclusiveBetween(1, 10).WithMessage("You must select a building position.");
            }
        }
    }
}