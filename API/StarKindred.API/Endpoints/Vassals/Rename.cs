using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Vassals;

[ApiController]
public sealed class Rename
{
    [HttpPost("/vassals/{vassalId:guid}/rename")]
    public async Task<ApiResponse> _(
        CancellationToken cToken,
        Guid vassalId,
        [FromBody] Request request,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var vassal = await db.Vassals
            .FirstOrDefaultAsync(v => v.Id == vassalId && v.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("There is no such Vassal.");

        // you CAN rename Vassals who are busy

        var newName = request.Name.Trim();
        
        if(vassal.Name == newName)
            throw new UnprocessableEntity($"{vassal.Name} is already called {vassal.Name}!");

        var treasures = db.Treasures
            .Where(t => t.UserId == session.UserId && t.Type == TreasureType.RenamingScroll)
            .ToList();

        TreasureHelper.UseOrThrow(treasures, TreasureType.RenamingScroll);

        PersonalLogHelper.Create(db, session.UserId, $"You renamed **{vassal.Name}** to **{newName}**.", new[]
        {
            PersonalLogActivityType.Vassal,
            PersonalLogActivityType.RenamedVassal
        });

        vassal.Name = newName;

        await db.SaveChangesAsync(cToken);

        return new ApiResponse();
    }

    public sealed record Request(string Name)
    {
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                Transform(x => x.Name, n => n.Trim())
                    .NotEmpty().WithMessage("Must provide a name for your Vassal.")
                    .MaximumLength(30).WithMessage("Vassal names must be 30 characters or less.")
                ;
            }
        }
    }
}