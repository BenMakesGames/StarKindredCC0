using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility.Technologies;

namespace StarKindred.API.Endpoints.Towns;

[ApiController]
public sealed class MyDecorations
{
    [HttpPost("/towns/my/decorations")]
    public async Task<ApiResponse> _(
        [FromBody] Request request,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var decorationTechsResearched = await db.UserTechnologies
            .CountAsync(t => t.UserId == session.UserId && TechTree.DecorationTechs.Contains(t.Technology), cToken);

        // 2, 6, 12, 20, 30
        var maxDecorations = 20 + decorationTechsResearched * (decorationTechsResearched + 1);

        if (request.Decorations.Count > maxDecorations)
            throw new UnprocessableEntity($"You may only have {maxDecorations} Decorations out at a time.");
        
        var decorationTypesAndQuantities = request.Decorations.GroupBy(d => d.Type).ToDictionary(g => g.Key, g => g.Count());
        var decorationTypes = decorationTypesAndQuantities.Keys.ToList();

        var town = await db.Towns.FirstAsync(t => t.UserId == session.UserId, cToken);
        
        var decorationsOwned = await db.Decorations
            .Where(d => d.UserId == session.UserId && decorationTypes.Contains(d.Type))
            .ToListAsync(cToken);

        if (
            decorationsOwned.Any(d => decorationTypesAndQuantities[d.Type] > d.Quantity) ||
            decorationTypes.Any(d => !decorationsOwned.Any(o => o.Type == d))
        )
        {
            throw new UnprocessableEntity("You do not have enough of the Decorations placed.");
        }

        db.TownDecorations.RemoveRange(db.TownDecorations.Where(d => d.TownId == town.Id));

        db.TownDecorations.AddRange(request.Decorations.Select(d => new TownDecoration()
        {
            TownId = town.Id,
            Type = d.Type,
            X = d.X,
            Y = d.Y,
            Scale = d.Scale,
            FlipX = d.FlipX
        }));

        await db.SaveChangesAsync(cToken);

        return new();
    }

    public sealed record Request(List<DecorationDto> Decorations)
    {
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleForEach(r => r.Decorations).ChildRules(d =>
                {
                    d.RuleFor(x => x.Scale).InclusiveBetween(50, 200).WithMessage("Scale must be between 50% and 200%.");
                });
            }
        }
    }

    public sealed record DecorationDto(DecorationType Type, float X, float Y, int Scale, bool FlipX);
}