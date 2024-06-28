using FluentValidation;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using StarKindred.API.Entities;
using StarKindred.API.Extensions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Weapons;

[ApiController]
public sealed class Search
{
    [HttpGet("/weapons")]
    public async Task<ApiResponse<PaginatedResults<WeaponDto>>> _(
        [FromQuery] RequestDto request,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        
        var weapons = await db.Weapons
            .Where(w => w.UserId == session.UserId)
            .WithPrimaryBonus(request.PrimaryBonus)
            .ExcludeWeaponIds(request.ExcludedWeapons)
            .OrderByDescending(w => w.Level)
            .ThenBy(w => w.Name)
            .Select(w => new WeaponDto(
                w.Id,
                w.Name,
                w.Level,
                w.Image,
                w.PrimaryBonus,
                w.Level >= 3 ? w.SecondaryBonus : null,
                WeaponHelper.ResourcesToLevelUp(w),
                w.Durability,
                w.MaxDurability,
                WeaponHelper.RepairValue(w),
                w.Vassal == null ? null : new VassalDto(w.Vassal.Id, w.Vassal.Name, w.Vassal.Level, w.Vassal.Element, w.Vassal.Species, w.Vassal.Portrait)
            ))
            .AsPaginatedResultsAsync(request.Page, 20, cToken);

        return new(weapons);
    }

    public sealed record RequestDto(int Page = 1, List<Guid>? ExcludedWeapons = null, WeaponBonus? PrimaryBonus = null)
    {
        public sealed class Validator : AbstractValidator<RequestDto>
        {
            public Validator()
            {
                RuleFor(x => x.Page).PageNumber();
            }
        }
    }
    
    public sealed record WeaponDto(
        Guid Id,
        string Name,
        int Level,
        string Image,
        WeaponBonus PrimaryEffect,
        WeaponBonus? SecondaryEffect,
        List<ResourceQuantity>? ResourcesToLevelUp,
        int Durability,
        int MaxDurability,
        int RepairValue,
        VassalDto? Vassal
    );
    public sealed record VassalDto(Guid Id, string Name, int Level, Element Element, Species Species, string Portrait);
}

public static class IQueryableExtensions
{
    public static IQueryable<Weapon> WithPrimaryBonus(this IQueryable<Weapon> query, WeaponBonus? bonus) =>
        bonus == null
            ? query
            : query.Where(v => v.PrimaryBonus == bonus);

    public static IQueryable<Weapon> ExcludeWeaponIds(this IQueryable<Weapon> query, List<Guid>? excludedWeaponIds) =>
        excludedWeaponIds == null
            ? query
            : query.Where(v => !excludedWeaponIds.Contains(v.Id));
}