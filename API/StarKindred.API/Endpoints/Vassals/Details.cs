using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;
using StarKindred.API.Utility.Technologies;

namespace StarKindred.API.Endpoints.Vassals;

[ApiController]
public sealed class Details
{
    private static readonly TreasureType[] UsefulTreasures =
    {
        TreasureType.Ichor,
        TreasureType.RenamingScroll
    };

    private static readonly TechnologyType[] RelevantTechnologies =
    {
        TechnologyType.ScrappingI,
    };
    
    [HttpGet("/vassals/{vassalId:guid}")]
    public async Task<ApiResponse<Response>> _(
        CancellationToken cToken,
        Guid vassalId,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var relevantTechnologies = await db.UserTechnologies
            .Where(t => t.UserId == session.UserId && RelevantTechnologies.Contains(t.Technology))
            .Select(t => t.Technology)
            .ToListAsync(cToken);

        var vassal = await db.Vassals
            .Include(v => v.Mission)
            .Include(v => v.TimedMission)
            .Include(v => v.UserAdventureStepInProgress!)
                .ThenInclude(s => s.AdventureStep)
            .Include(v => v.StatusEffects)
            .Include(v => v.Weapon)
            .Include(v => v.Tags)
            .Include(v => v.Relationships!.OrderByDescending(r => r.Minutes))
            .Include(v => v.Leader)
            .AsSplitQuery()
            .FirstOrDefaultAsync(v => v.Id == vassalId && v.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("There is no such Vassal.");

        var relationshipIds = vassal.Relationships!.Select(r => r.Id).ToList();

        var friends = await db.Relationships
            .Include(r => r.Vassals!/*.Where(v => v.Id != vassalId)*/) // TODO: why doesn't this work!?
            .AsSingleQuery() // TODO: not profiled
            .Where(r => relationshipIds.Contains(r.Id))
            .OrderByDescending(r => r.Minutes)
            .ToListAsync(cToken);

        var friendDtos = friends
            .SelectMany(r => r.Vassals!
                .Where(v => v.Id != vassalId)
                .Select(v => new RelationshipDto(
                    v.Id, v.Name, v.Favorite, v.Portrait, v.Species, v.Element, v.Level,
                    r.Minutes, r.Level, RelationshipHelper.LevelProgress(r.Level, r.Minutes)
                ))
            )
            .ToList();

        var treasures = await db.Treasures
            .Where(t => t.UserId == session.UserId && UsefulTreasures.Contains(t.Type))
            .Select(t => new TreasureDto(t.Type, t.Quantity))
            .ToListAsync(cToken);

        var hasFreeTrade = await TechTree.HasTechnology(db, session.UserId, TechnologyType.FreeTrade, cToken);

        return new ApiResponse<Response>(new(
            new(
                vassal.Name,
                vassal.Portrait,
                vassal.Level,
                vassal.Willpower,
                vassal.RetirementPoints,
                vassal.Favorite,
                vassal.Species,
                vassal.Element,
                vassal.Nature,
                vassal.Sign,
                vassal.CreatedOn,
                vassal.StatusEffects!
                    .Select(se => new StatusEffectDto(
                        se.Id,
                        se.Type,
                        se.Strength
                    ))
                    .ToList(),
                vassal.Tags!.Select(t => new TagDto(t.Title, t.Color)).ToList(),
                VassalMath.ResourcesToLevelUp(vassal, hasFreeTrade),
                CreateMissionDto(vassal),
                vassal.Leader?.Position,
                CreateWeaponDto(vassal),
                friendDtos
            ),
            treasures,
            relevantTechnologies
        ));
    }

    private MissionType? CreateMissionDto(Vassal vassal)
    {
        if (vassal.Mission != null) return vassal.Mission.Type;
        
        if(vassal.TimedMission != null) return vassal.TimedMission.Type;

        if(vassal.UserAdventureStepInProgress != null) return vassal.UserAdventureStepInProgress.AdventureStep!.Type;
        
        return null;
    }

    private WeaponDto? CreateWeaponDto(Vassal vassal)
    {
        if(vassal.Weapon == null)
            return null;
        
        return new WeaponDto(
            vassal.Weapon.Id,
            vassal.Weapon.Name,
            vassal.Weapon.Image,
            vassal.Weapon.Level,
            vassal.Weapon.PrimaryBonus,
            vassal.Weapon.Level >= 3 ? vassal.Weapon.SecondaryBonus : null,
            WeaponHelper.ResourcesToLevelUp(vassal.Weapon),
            vassal.Weapon.Durability,
            vassal.Weapon.MaxDurability
        );
    }

    public sealed record Response(VassalDetails Vassal, List<TreasureDto> Treasure, List<TechnologyType> Technologies);

    public sealed record TreasureDto(TreasureType Type, int Quantity);
    
    public sealed record VassalDetails(
        string Name,
        string Portrait,
        int Level,
        int Willpower,
        int RetirementProgress,
        bool Favorite,
        Species Species,
        Element Element,
        Nature Nature,
        AstrologicalSign Sign,
        DateTimeOffset RecruitDate,
        List<StatusEffectDto> StatusEffects,
        List<TagDto> Tags,
        List<ResourceQuantity> ResourcesToLevelUp,
        MissionType? Mission,
        TownLeaderPosition? Leader,
        WeaponDto? Weapon,
        List<RelationshipDto> Relationships
    );

    public sealed record RelationshipDto(
        Guid Id, string Name, bool Favorite, string Portrait, Species Species, Element Element, int Level,
        int RelationshipMinutes, int RelationshipLevel, float RelationshipProgress
    );
    public sealed record StatusEffectDto(Guid Id, StatusEffectType Type, int Strength);
    public sealed record WeaponDto(
        Guid Id,
        string Name,
        string Image,
        int Level,
        WeaponBonus PrimaryBonus,
        WeaponBonus? SecondaryBonus,
        List<ResourceQuantity>? ResourcesToLevelUp,
        int Durability,
        int MaxDurability
    );
    public sealed record TagDto(string Title, string Color);
}