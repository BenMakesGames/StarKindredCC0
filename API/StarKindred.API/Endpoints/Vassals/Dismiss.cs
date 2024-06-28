using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Vassals;

[ApiController]
public sealed class Dismiss
{
    [HttpPost("/vassals/{vassalId:guid}/dismiss")]
    public async Task<ApiResponse> _(
        Guid vassalId,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        var user = await db.Users.FirstAsync(u => u.Id == session.UserId, cToken);

        var vassal = await db.Vassals
            .Include(v => v.Leader)
            .FirstOrDefaultAsync(v => v.Id == vassalId && v.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("That Vassal does not exist.");

        if (vassal.Favorite)
            throw new UnprocessableEntity("You cannot dismiss a Favorite Vassal.");

        if(vassal.IsOnAMission)
            throw new UnprocessableEntity("You cannot dismiss a Vassal while they're busy with a task.");

        if(vassal.Leader != null)
            throw new UnprocessableEntity("You cannot dismiss a Vassal while they hold a leadership position.");

        if(vassal.RetirementPoints >= 10)
            throw new UnprocessableEntity("This Vassal is ready to be Retired! Do that, instead!");

        if ((await db.Vassals.CountAsync(v => v.UserId == session.UserId, cToken)) == 1)
            throw new UnprocessableEntity("You can't Dismiss your _only_ Vassal! Recruit some more Vassals, first.");

        PersonalLogHelper.Create(db, session.UserId, $"You dismissed **{vassal.Name}**.", new[]
        {
            PersonalLogActivityType.Vassal,
            PersonalLogActivityType.DismissedVassal
        });

        db.Vassals.Remove(vassal);

        await db.SaveChangesAsync(cToken);

        await UserHelper.ComputeLevel(db, user, cToken);

        await db.SaveChangesAsync(cToken);

        return new();
    }
}