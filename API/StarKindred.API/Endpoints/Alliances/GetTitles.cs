using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.Alliances;

[ApiController]
public sealed class GetTitles
{
    [HttpGet("alliances/titles")]
    public async Task<ApiResponse<ResponseDto>> _(
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var myAlliance = await db.Alliances
            .FirstOrDefaultAsync(a => a.Members!.Any(m => m.UserId == session.UserId), cToken)
            ?? throw new NotFoundException("You aren't in an Alliance!");

        var ranks = await db.AllianceRanks
            .Where(r => r.Alliance!.Members!.Any(m => m.UserId == session.UserId))
            .Select(r => new TitleDto(r.Id, r.Title, r.Rank, r.CanRecruit, r.CanKick, r.CanTrackGiants))
            .ToListAsync(cToken);

        return new(new(ranks, myAlliance.LeaderId == session.UserId));
    }

    public sealed record ResponseDto(List<TitleDto> Titles, bool CanEdit);
    public sealed record TitleDto(Guid Id, string Title, int Rank, bool CanRecruit, bool CanKick, bool CanTrackGiants);
}