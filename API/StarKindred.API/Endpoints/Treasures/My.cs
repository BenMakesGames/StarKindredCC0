using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Treasures;

[ApiController]
public sealed class My
{
    private static readonly TechnologyType[] RelevantTechnologies =
    {
        TechnologyType.ScrappingI,
        TechnologyType.ScrappingII
    };

    [HttpGet("/treasures/my")]
    public async Task<ApiResponse<Response>> _(
        [FromServices] ICurrentUser currentUser, [FromServices] Db db,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var treasures = await db.Treasures
            .Where(t => t.UserId == session.UserId && t.Quantity > 0)
            .Select(t => new TreasureDto(t.Type, t.Quantity))
            .ToListAsync(cToken);

        var relevantTechnologies = await db.UserTechnologies
            .Where(t => t.UserId == session.UserId && RelevantTechnologies.Contains(t.Technology))
            .Select(t => t.Technology)
            .ToListAsync(cToken);
        
        // 1: primary 1
        // 2: primary 2
        // 3: primary 2, secondary 1
        // 4: primary 3, secondary 1
        // 5: primary 3, secondary 2
        var weapons = await db.Weapons
            .Where(w => w.UserId == session.UserId)
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
                w.Vassal == null ? null : new VassalDto(w.Vassal.Id, w.Vassal.Name, w.Vassal.Level, w.Vassal.Element, w.Vassal.Species, w.Vassal.Portrait)
            ))
            .ToListAsync(cToken);

        return new(new(treasures, weapons, relevantTechnologies));
    }

    public sealed record Response(List<TreasureDto> Treasures, List<WeaponDto> Weapons, List<TechnologyType> Technologies);
    public sealed record TreasureDto(TreasureType Type, int Quantity);
    
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
        VassalDto? Vassal
    );

    public sealed record VassalDto(Guid Id, string Name, int Level, Element Element, Species Species, string Portrait);
}