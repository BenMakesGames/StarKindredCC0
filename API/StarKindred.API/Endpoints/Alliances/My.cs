using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Alliances;

[ApiController]
public sealed class My
{
    [HttpGet("/alliances/my")]
    public async Task<ApiResponse<AllianceDto?>> _(
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var data = await db.UserAlliances
            .Where(ua => ua.UserId == session.UserId)
            .Select(ua => new {
                Members = ua.Alliance!.Members!
                    .Select(m => new MemberDto(
                        m.UserId,
                        m.User!.Name,
                        m.User.Level,
                        m.AllianceRank == null ? null : m.AllianceRank.Title,
                        // alliance leaders have no title, and therefore no rank; to sort by rank, we hack alliance leaders int.MaxValue:
                        m.UserId == m.Alliance!.LeaderId ? int.MaxValue : (m.AllianceRank == null ? 0 : m.AllianceRank.Rank),
                        m.User.Avatar,
                        m.User.Color
                    ))
                    .ToList(),
                ua.Alliance.LeaderId,
                session.UserId,
                ua.Alliance.CreatedOn,
                ua.Alliance.Level,
                Giant = ua.Alliance!.Giant == null
                    ? null
                    : new GiantDto(ua.Alliance!.Giant.StartsOn, ua.Alliance.Giant.ExpiresOn, ua.Alliance.Giant.Element, ua.Alliance.Giant.Health, ua.Alliance.Giant.Damage),
                Logs = ua.Alliance.Logs!
                    .OrderByDescending(l => l.CreatedOn)
                    .Take(10)
                    .Select(l => new LogDto(l.CreatedOn, l.ActivityType, l.Message))
                    .ToList()
            })
            .AsSplitQuery() // TODO: not profiled
            .AsNoTracking()
            .FirstOrDefaultAsync(cToken);

        if (data == null)
            return new(null);

        var rank = await db.UserAlliances
            .Include(ua => ua.AllianceRank)
            .FirstAsync(ua => ua.UserId == session.UserId, cToken);
        
        return new(new AllianceDto(
            data.Members,
            data.LeaderId,
            session.UserId,
            data.CreatedOn,
            data.Level,
            data.Giant,
            AllianceRightsHelper.GetRights(data.LeaderId, rank),
            data.Logs
        ));
    }

    public sealed record AllianceDto(List<MemberDto> Members, Guid LeaderId, Guid MyId, DateTimeOffset CreatedOn, int Level, GiantDto? Giant, List<AllianceRight> Rights, List<LogDto> Logs);
    public sealed record MemberDto(Guid Id, string Name, int Level, string? Rank, int RankLevel, string Avatar, HSL Color);
    public sealed record GiantDto(DateTimeOffset StartsOn, DateTimeOffset ExpiresOn, Element Element, int Health, int Damage);
    public sealed record LogDto(DateTimeOffset CreatedOn, AllianceLogActivityType ActivityType, string Message);
}