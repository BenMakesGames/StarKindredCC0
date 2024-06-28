using FluentValidation;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Treasures;

[ApiController]
public sealed class UseWeaponChest
{
    [HttpPost("treasures/use/weaponChest")]
    public async Task<ApiResponse> _(
        [FromBody] RequestDto request,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Random rng,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        
        if (request.Choice is Choice.Wood or Choice.Iron or Choice.Quint)
        {
            var hasScrappingII = await db.UserTechnologies
                .AnyAsync(ut => ut.UserId == session.UserId && ut.Technology == TechnologyType.ScrappingII, cToken);

            if (!hasScrappingII)
                throw new UnprocessableEntity("You don't have the required technology.");
        }

        await TreasureHelper.FindAndUseQuantityOrThrow(db, session.UserId, TreasureType.WeaponChest, request.Quantity, cToken);

        if (request.Choice is Choice.Wood or Choice.Iron or Choice.Quint)
        {
            if (request.Choice == Choice.Wood)
                await ResourceHelper.CollectResources(db, session.UserId, new() { new(ResourceType.Wood, 50 * request.Quantity) }, cToken);
            else if (request.Choice == Choice.Iron)
                await ResourceHelper.CollectResources(db, session.UserId, new() { new(ResourceType.Iron, 25 * request.Quantity) }, cToken);
            else if (request.Choice == Choice.Quint)
                await ResourceHelper.CollectResources(db, session.UserId, new() { new(ResourceType.Quintessence, 10 * request.Quantity) }, cToken);
            else
                throw new ArgumentOutOfRangeException();
        }
        else
        {
            for (int i = 0; i < request.Quantity; i++)
            {
                WeaponHelper.CollectWeapon(db, rng, session.UserId, request.Choice switch
                {
                    Choice.HuntingLevels => WeaponBonus.HuntingLevels,
                    Choice.FasterMissions => WeaponBonus.FasterMissions,
                    Choice.MoreGold => WeaponBonus.MoreGold,
                    Choice.MeatGetsWood => WeaponBonus.MeatGetsWood,
                    Choice.GoldGetsWine => WeaponBonus.GoldGetsWine,
                    Choice.WeaponsGetWheat => WeaponBonus.WeaponsGetWheat,
                    Choice.RecruitBonus => WeaponBonus.RecruitBonus,
                    _ => throw new ArgumentOutOfRangeException()
                });
            }
        }

        await db.SaveChangesAsync(cToken);

        return new();
    }

    public sealed record RequestDto(Choice Choice, int Quantity = 1)
    {
        public sealed class Validator : AbstractValidator<RequestDto>
        {
            public Validator()
            {
                RuleFor(x => x.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Cannot use fewer than 1 at a time.")

                    .LessThanOrEqualTo(20)
                    .When(x => x.Choice is not (Choice.Iron or Choice.Wood or Choice.Quint))
                    .WithMessage("When selecting a weapon, can only open up to 20 at a time. (Sorry, I realize that probs seems arbitrary and annoying.)")
                ;
            }
        }
    }


    public enum Choice
    {
        HuntingLevels, // more levels when animal or monster hunting
        FasterMissions, // faster missions
        MoreGold, // more gold when gold is earned
        MeatGetsWood, // wood when meat is earned
        GoldGetsWine, // wine when gold is earned
        WeaponsGetWheat, // wheat when weapons are collected
        RecruitBonus, // bonus levels to new recruits
        Wood,
        Iron,
        Quint
    }
}