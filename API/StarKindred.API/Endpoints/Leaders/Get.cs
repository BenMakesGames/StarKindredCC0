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
public sealed class Get
{
    [HttpGet("/leaders")]
    public async Task<ApiResponse<Response>> _(
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var townIsLevel3 = await db.Towns.AnyAsync(t => t.UserId == session.UserId && t.Level >= 3, cToken);

        if(!townIsLevel3)
            throw new AccessDeniedException("You must complete more missions for The Oracle.");

        var leaders = await db.TownLeaders
            .Where(l => l.UserId == session.UserId)
            .Select(l => new LeaderDto(
                l.Position,
                l.AssignedOn.AddDays(1),
                new VassalDto(
                    l.Vassal!.Id,
                    l.Vassal.Element,
                    l.Vassal.Nature,
                    l.Vassal.Sign,
                    l.Vassal.Species,
                    l.Vassal.Portrait,
                    l.Vassal.Level,
                    l.Vassal.Name
                ),
                null
            ))
            .ToListAsync(cToken)
        ;

        var research = await db.UserResearches
            .Where(r => r.UserId == session.UserId)
            .ToListAsync(cToken);

        leaders = leaders
            .Select(l =>
            {
                var project = research.FirstOrDefault(p => TechTree.TechInfo[p.Technology].Category == l.Position);

                if(project == null)
                    return l;

                var info = TechTree.TechInfo[project.Technology];
                var researchTime = info.ResearchTime(l.Vassal.Level);

                return l with {
                    Project = new ResearchDto(
                        project.Technology,
                        info.Title,
                        project.StartDate,
                        project.StartDate.AddMinutes(researchTime)
                    )
                };
            })
            .ToList();

        return new(new(leaders));
    }

    public sealed record Response(List<LeaderDto> Leaders);
    public sealed record LeaderDto(TownLeaderPosition Position, DateTimeOffset EstablishedOn, VassalDto Vassal, ResearchDto? Project);
    public sealed record VassalDto(
        Guid Id,
        Element Element,
        Nature Nature,
        AstrologicalSign Sign,
        Species Species,
        string Portrait,
        int Level,
        string Name
    );
    public sealed record ResearchDto(TechnologyType Technology, string Title, DateTimeOffset StartDate, DateTimeOffset EndDate);
}