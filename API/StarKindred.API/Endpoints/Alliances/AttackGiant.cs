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

namespace StarKindred.API.Endpoints.Alliances;

[ApiController]
public sealed class AttackGiant
{
    [HttpPost("alliances/attackGiant")]
    public async Task<ApiResponse> _(
        [FromBody] RequestDto request,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        var user = await db.Users.FirstAsync(u => u.Id == session.UserId, cToken);

        var now = DateTimeOffset.UtcNow;

        if(user.LastAttackedGiant.Date == now.Date)
            throw new UnprocessableEntity("You have already attacked the Giant today.");

        var membership = await db.UserAlliances
            .Where(ua => ua.UserId == session.UserId)
            .Include(ua => ua.Alliance!)
                .ThenInclude(a => a.Giant)
            .AsSingleQuery()
            .FirstOrDefaultAsync(cToken)
            ?? throw new NotFoundException("You are not in an Alliance.");

        if (membership.Alliance!.Giant == null)
            throw new NotFoundException("Your Alliance is not tracking a Giant.");

        if (membership.Alliance.Giant.StartsOn > now)
            throw new UnprocessableEntity("The Giant has not yet arrived.");

        if(membership.Alliance.Giant.ExpiresOn < now)
            throw new UnprocessableEntity("The Giant has already left.");

        var vassals = await db.Vassals
            .Include(v => v.StatusEffects)
            .Include(v => v.Weapon)
            .Include(v => v.Leader)
            .AsSplitQuery() // TODO: not profiled
            .Where(v => v.UserId == session.UserId && request.Vassals.Contains(v.Id))
            .ToListAsync(cToken);
        
        if(vassals.Count != request.Vassals.Count)
            throw new UnprocessableEntity("One or more of the selected Vassals could not be found.");

        if(vassals.Any(v => v.IsOnAMission || v.Leader != null))
            throw new UnprocessableEntity("One or more of the selected Vassals is busy with another task.");

        if(vassals.Any(v => v.StatusEffects!.Any(se => se.Type == StatusEffectType.BrokenBone)))
            throw new UnprocessableEntity("Vassals with Broken Bones cannot attack the Giant.");

        foreach (var v in vassals)
        {
            if (v.Nature == Nature.Cavalier)
                VassalMath.IncreaseWillpower(v);

            if (v.Weapon != null)
                WeaponHelper.DegradeWeaponDurability(v, v.Weapon);

            StatusEffectsHelper.UpdateStatusEffects(v);
        }

        await RelationshipHelper.AdvanceRelationshipsWithNoChanceOfLoot(db, vassals, 12 * 60, cToken);

        var damage = MissionMath.AttackDamage(vassals, membership.Alliance.Giant.Element);

        membership.Alliance.Giant.Damage += damage;
        user.LastAttackedGiant = now;

        var fightContribution = new GiantContribution()
        {
            UserId = session.UserId,
            Damage = damage,
        };
        
        db.GiantContributions.Add(fightContribution);
        
        db.AllianceLogs.Add(new()
        {
            ActivityType = AllianceLogActivityType.MemberAttackedGiant,
            AllianceId = membership.AllianceId,
            Message = $"{session.Name} attacked the Giant, dealing {damage} damage.",
        });

        await db.SaveChangesAsync(cToken);

        return new();
    }

    public sealed record RequestDto(List<Guid> Vassals)
    {
        public sealed class Validator : AbstractValidator<RequestDto>
        {
            public Validator()
            {
                RuleFor(x => x.Vassals).NotEmpty().WithMessage("You must select at least one Vassal.");
                RuleFor(x => x.Vassals.Count).LessThanOrEqualTo(3).WithMessage("You may only attack with up to three Vassals.");
            }
        }
    }
}