using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;

namespace StarKindred.API.Endpoints.Alliances;

[ApiController]
public sealed class Details
{
    [HttpGet("/alliances/{id:guid}")]
    public async Task<ApiResponse<AllianceDto>> _(
        Guid id,
        [FromServices] Db db,
        CancellationToken cToken
    )
    {
        var result = await db.Alliances
            .Where(a => a.Id == id)
            .Select(a => new AllianceDto(
                a.Members!
                    .Select(m => new MemberDto(
                        m.UserId,
                        m.User!.Name,
                        m.User.Level,
                        // alliance leaders have no title, and therefore no rank; to sort by rank, we hack alliance leaders int.MaxValue:
                        m.UserId == m.Alliance!.LeaderId ? int.MaxValue : (m.AllianceRank == null ? 0 : m.AllianceRank.Rank),
                        m.User.Avatar,
                        m.User.Color
                    ))
                    .ToList(),
                a.LeaderId,
                a.CreatedOn,
                a.Level,
                a.AllianceRecruitStatus == null || !a.AllianceRecruitStatus.OpenInvitationActive
                    ? null
                    : new OpenInvitation(a.AllianceRecruitStatus.OpenInvitationMinLevel, a.AllianceRecruitStatus.OpenInvitationMaxLevel)

            ))
            .AsNoTracking()
            .FirstOrDefaultAsync(cToken)
            ?? throw new NotFoundException("Alliance not found.");

        return new(result);
    }

    public sealed record AllianceDto(List<MemberDto> Members, Guid LeaderId, DateTimeOffset CreatedOn, int Level, OpenInvitation? OpenInvitation);
    public sealed record MemberDto(Guid Id, string Name, int Level, int RankLevel, string Avatar, HSL Color);
    public sealed record OpenInvitation(int MinLevel, int MaxLevel);
}