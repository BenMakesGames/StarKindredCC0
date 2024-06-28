using FluentValidation;
using StarKindred.API.Utility;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Extensions;

namespace StarKindred.API.Endpoints.Alliances;

[ApiController]
public sealed class Search
{
    public const int PageSize = 20;
    
    [HttpGet("/alliances")]
    public async Task<ApiResponse<PaginatedResults<AllianceSummaryDto>>> _(
        [FromQuery] Request request,
        [FromServices] Db db,
        CancellationToken cToken
    )
    {
        var results = await db.Alliances
            .OrderByDescending(a => a.LastActiveOn)
            .ThenBy(a => a.CreatedOn)
            .Select(a => new AllianceSummaryDto(
                a.Id,
                new Leader(a.Leader!.Name, a.Leader.Avatar, a.Leader.Color),
                a.CreatedOn,
                a.LastActiveOn,
                a.Level,
                a.Members!.Count,
                a.AllianceRecruitStatus == null || !a.AllianceRecruitStatus.OpenInvitationActive
                    ? null
                    : new OpenInvitation(a.AllianceRecruitStatus.OpenInvitationMinLevel, a.AllianceRecruitStatus.OpenInvitationMaxLevel)
            ))
            .AsNoTracking()
            .AsPaginatedResultsAsync(request.Page, PageSize, cToken);

        return new ApiResponse<PaginatedResults<AllianceSummaryDto>>(results);
    }

    public sealed record Request(int Page = 1)
    {
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Page).PageNumber();
            }
        }
    }
    public sealed record AllianceSummaryDto(Guid Id, Leader Leader, DateTimeOffset CreatedOn, DateTimeOffset? LastActiveOn, int Level, int MemberCount, OpenInvitation? OpenInvitation);
    public sealed record Leader(string Name, string Avatar, HSL Color);
    public sealed record OpenInvitation(int MinLevel, int MaxLevel);
}