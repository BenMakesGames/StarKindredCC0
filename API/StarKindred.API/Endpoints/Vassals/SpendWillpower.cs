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
public sealed class SpendWillpower
{
    [HttpPost("/vassals/{vassalId:guid}/spendWillpower")]
    public async Task<ApiResponse> _(
        CancellationToken cToken,
        Guid vassalId,
        Request request,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        var user = await db.Users.FirstAsync(u => u.Id == session.UserId, cToken);

        var vassal = await db.Vassals
            .Include(v => v.Mission)
            .Include(v => v.TimedMission)
            .Include(v => v.StatusEffects)
            .Include(v => v.Leader)
            .AsSingleQuery()
            .FirstOrDefaultAsync(v => v.Id == vassalId && v.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("There is no such Vassal.");

        if (vassal.IsOnAMission)
            throw new UnprocessableEntity("Cannot spend Willpower for a Vassal while they're busy with a task.");

        var options = VassalMath.WillpowerOptions(vassal);

        var selection = options.Find(o => o.Type == request.Selection && o.CanUse)
            ?? throw new UnprocessableEntity("Cannot spend Willpower that way on this Vassal right now.");

        if (selection.Cost > vassal.Willpower)
            throw new UnprocessableEntity($"{vassal.Name} does not have enough Willpower.");

        var tags = new List<PersonalLogActivityType>()
        {
            PersonalLogActivityType.Vassal,
            PersonalLogActivityType.SpentWillpower,
        };

        string effectDescription;

        switch (selection.Type)
        {
            case WillpowerSpendType.Focus:
                effectDescription = "grant them Focus";
                StatusEffectsHelper.AddStatusEffect(vassal, StatusEffectType.Focused, 1);
                break;

            case WillpowerSpendType.GoldBuff:
                effectDescription = "grant them Golden Touch";
                StatusEffectsHelper.AddStatusEffect(vassal, StatusEffectType.GoldenTouch, 1);
                break;

            case WillpowerSpendType.LevelBuff:
                effectDescription = "grant them Strength";
                StatusEffectsHelper.AddStatusEffect(vassal, StatusEffectType.Power, 2);
                break;

            case WillpowerSpendType.QuintBuff:
                effectDescription = "grant them Artistic Visions";
                StatusEffectsHelper.AddStatusEffect(vassal, StatusEffectType.ArtisticVision, 1);
                break;

            case WillpowerSpendType.LevelUp:
                effectDescription = $"level them up from level {vassal.Level} to {vassal.Level + 1}";
                tags.Add(PersonalLogActivityType.LeveledUpVassal);
                vassal.Level++;
                break;

            default: throw new Exception("That option hasn't been implemented :(");
        }

        vassal.Willpower -= selection.Cost;

        PersonalLogHelper.Create(db, session.UserId, $"You spent {selection.Cost} of **{vassal.Name}**'s Willpower to {effectDescription}.", tags);

        await db.SaveChangesAsync(cToken);

        if (selection.Type == WillpowerSpendType.LevelUp)
        {
            await UserHelper.ComputeLevel(db, user, cToken);

            await db.SaveChangesAsync(cToken);
        }

        return new ApiResponse();
    }

    public sealed record Request(WillpowerSpendType Selection);
}