using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Accounts;

[ApiController]
public sealed class ChangeRibbon
{
    [HttpPost("/accounts/changeRibbon")]
    public async Task<ApiResponse> _(
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        [FromBody] Request request,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        var user = await db.Users.FirstAsync(u => u.Id == session.UserId, cToken);

        user.Color = request.Color;

        PersonalLogHelper.Create(db, session.UserId, $"You changed your Avatar Color.", new[]
        {
            PersonalLogActivityType.AccountActivity,
            PersonalLogActivityType.UpdatedAvatarColor
        });

        await db.SaveChangesAsync(cToken);

        return new();
    }

    public sealed record Request(HSL Color)
    {
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Color.Hue).InclusiveBetween(0, 360).WithMessage("Hue must be between 0 and 360.");
                RuleFor(x => x.Color.Saturation).InclusiveBetween(0, 100).WithMessage("Saturation must be between 0 and 100.");
                RuleFor(x => x.Color.Luminosity).InclusiveBetween(0, 100).WithMessage("Luminosity must be between 0 and 100.");
            }
        }
    }
}