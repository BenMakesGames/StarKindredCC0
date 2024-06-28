using FluentValidation;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Treasures;

[ApiController]
public sealed class UseFishBag
{
    [HttpPost("treasures/use/fishBag")]
    public async Task<ApiResponse> _(
        [FromBody] RequestDto request,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        
        await TreasureHelper.FindAndUseQuantityOrThrow(db, session.UserId, TreasureType.FishBag, request.Quantity, cToken);

        var gains = request.Choice switch
        {
            Choice.Meat => new ResourceQuantity(ResourceType.Meat, 250 * request.Quantity),
            Choice.Quintessence => new ResourceQuantity(ResourceType.Quintessence, 100 * request.Quantity),
            _ => throw new UnprocessableEntity("Invalid choice."),
        };
        
        await ResourceHelper.CollectResources(db, session.UserId, new List<ResourceQuantity>() { gains }, cToken);

        await db.SaveChangesAsync(cToken);

        return new();
    }

    public sealed record RequestDto(Choice Choice, int Quantity = 1)
    {
        public sealed class Validator : AbstractValidator<RequestDto>
        {
            public Validator()
            {
                RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Cannot use fewer than 1 at a time.");
            }
        }
    }

    public enum Choice { Meat, Quintessence };
}