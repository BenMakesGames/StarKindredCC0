using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility.Technologies;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;

namespace StarKindred.API.Endpoints.Leaders;

[ApiController]
public sealed class GetAvailableResearch
{
    [HttpGet("/leaders/{positionString}/researchOptions")]
    public async Task<ApiResponse<ResponseDto>> _(
        string positionString,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db,
        CancellationToken cToken
    )
    {
        if (!Enum.TryParse(positionString, out TownLeaderPosition position))
            throw new NotFoundException("Unrecognized town leadership position.");

        var session = await currentUser.GetSessionOrThrow(cToken);

        var leader = await db.TownLeaders.Where(l => l.UserId == session.UserId && l.Position == position)
            .Select(l => l.Vassal)
            .FirstOrDefaultAsync(cToken)
            ?? throw new UnprocessableEntity("A leader has not been assigned to this position. No research can take place.");

        var researchedTechnologies = await db.UserTechnologies
            .Where(ut => ut.UserId == session.UserId)
            .Select(ut => ut.Technology)
            .ToListAsync(cToken);

        var availableTechs = TechTree.AvailableTechnologies(position, researchedTechnologies)
            .Select(t =>
            {
                var tech = TechTree.TechInfo[t];
                var time = tech.ResearchTime(leader.Level);

                return new TechInfoDto(t, tech.Title, tech.ResearchCost(leader.Nature), time);
            })
            .ToList();

        return new(new(availableTechs));
    }

    public sealed record ResponseDto(List<TechInfoDto> Technologies);
    public sealed record TechInfoDto(TechnologyType Value, string Title, List<ResourceQuantity> Cost, int TimeInMinutes);
}