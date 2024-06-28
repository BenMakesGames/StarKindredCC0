using EFCoreSecondLevelCacheInterceptor;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Missions;

[ApiController]
public sealed class Active
{
    [HttpGet("/missions")]
    public async Task<ApiResponse<Response>> _(
        CancellationToken cToken,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        var user = await db.Users.FirstAsync(u => u.Id == session.UserId, cToken);

        var missions = await db.Missions
            .Include(m => m.Vassals!)
                .ThenInclude(v => v.Weapon)
            .AsSingleQuery() // TODO: not profiled
            .Where(m => m.UserId == session.UserId)
            .ToListAsync(cToken);
        
        var data = missions
            .Select(m => new MissionDto(
                m.Id,
                m.Type,
                m.Vassals!.Select(v => new VassalDto(v.Id, v.Species, v.Portrait, v.Name, v.Level, v.Element)).ToList(),
                m.CreatedOn.ToUnixTimeMilliseconds(),
                m.CreatedOn.AddMinutes(MissionMath.DurationInMinutes(m.Type, 0, m.Vassals!)).ToUnixTimeMilliseconds()
            ))
            .ToList();

        var vassalCount = await db.Vassals.CountAsync(v => v.UserId == session.UserId, cToken);
        
        var highestVassalLevel = await db.Vassals
            .Where(v => v.UserId == session.UserId)
            .MaxAsync(v => v.Level, cToken);
        
        var available = MissionMath.AvailableMissions(vassalCount, highestVassalLevel)
            .Where(t => !missions.Any(m => m.Type == t))
            .Select(t => new AvailableDto(t, MissionMath.MinVassals(t, 0), MissionMath.MaxVassals(t, 0)))
            .ToList();

        var availableTimed = await db.TimedMissions
            .Where(m => m.UserId == session.UserId)
            .Select(m => new TimedMissionDto(
                m.Id,
                m.Type,
                m.Treasure,
                m.Weapon,
                m.Level,
                m.Element,
                m.Species,
                MissionMath.MinVassals(m.Type, m.Level),
                MissionMath.MaxVassals(m.Type, m.Level),
                m.Location,
                m.Vassals!.Select(v => new VassalDto(v.Id, v.Species, v.Portrait, v.Name, v.Level, v.Element)).ToList(),
                m.StartedOn == null ? null : m.StartedOn.Value.ToUnixTimeMilliseconds(),
                m.CompletesOn == null ? null : m.CompletesOn.Value.ToUnixTimeMilliseconds()
            ))
            .ToListAsync(cToken);

        var giant = await db.UserAlliances
            .Include(ua => ua.Alliance!)
                .ThenInclude(a => a.Giant)
            .AsSplitQuery() // TODO: not profiled
            .Where(ua => ua.UserId == session.UserId && ua.Alliance!.Giant != null)
            .Select(ua => new GiantDto(
                ua.Alliance!.Giant!.Element,
                ua.Alliance.Level,
                ua.Alliance.Giant.Health,
                ua.Alliance.Giant.Damage,
                ua.Alliance.Giant.StartsOn,
                ua.Alliance.Giant.ExpiresOn,
                user.LastAttackedGiant.Date < DateTimeOffset.UtcNow.Date
            ))
            .FirstOrDefaultAsync(cToken);

        var tags = await db.UserVassalTags
            .Where(t => t.UserId == session.UserId)
            .Select(t => new TagDto(t.Title, t.Color))
            .Cacheable(CacheExpirationMode.Sliding, TimeSpan.FromDays(1))
            .ToListAsync(cToken);
        
        return new(new(tags, data, availableTimed, available, giant));
    }

    public sealed record Response(List<TagDto> Tags, List<MissionDto> Missions, List<TimedMissionDto> TimedMissions, List<AvailableDto> Available, GiantDto? Giant);
    public sealed record AvailableDto(MissionType Type, int MinVassals, int MaxVassals);
    public sealed record MissionDto(Guid Id, MissionType Type, List<VassalDto> Vassals, long StartedOn, long CompletesOn);
    public sealed record TagDto(string Title, string Color);
    
    public sealed record TimedMissionDto(
        Guid Id,
        MissionType Type,
        TreasureType? Treasure,
        WeaponBonus? Weapon,
        int Level,
        Element? Element,
        Species? Species,
        int MinVassals, int MaxVassals,
        int Location,
        List<VassalDto> Vassals,
        long? StartedOn, long? CompletesOn
    );
    
    public sealed record VassalDto(Guid Id, Species Species, string Portrait, string Name, int Level, Element Element);
    public sealed record GiantDto(Element Element, int Level, int Health, int Damage, DateTimeOffset StartsOn, DateTimeOffset ExpiresOn, bool CanAttack);
}