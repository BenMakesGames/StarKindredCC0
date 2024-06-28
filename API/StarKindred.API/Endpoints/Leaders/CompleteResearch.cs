using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;
using StarKindred.API.Utility.Technologies;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;

namespace StarKindred.API.Endpoints.Leaders;

[ApiController]
public sealed class CompleteResearch
{
    [HttpPost("leaders/{positionString}/completeResearch")]
    public async Task<ApiResponse> _(
        string positionString,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
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

        var currentlyResearching = await db.UserResearches.Where(t => t.UserId == session.UserId).ToListAsync(cToken);

        var research = currentlyResearching.FirstOrDefault(r => TechTree.TechInfo[r.Technology].Category == position)
            ?? throw new NotFoundException("You're not working on that project!");

        var techInfo = TechTree.TechInfo[research.Technology];

        if (DateTimeOffset.UtcNow < research.StartDate.AddMinutes(techInfo.ResearchTime(leader.Level)))
            throw new UnprocessableEntity("That project is still in-progress!");

        db.UserTechnologies.Add(new()
        {
            UserId = session.UserId,
            Technology = research.Technology,
        });

        db.UserResearches.Remove(research);

        PersonalLogHelper.Create(db, session.UserId, $"{leader.Name} completed the {techInfo.Title} project.", new[]
        {
            PersonalLogActivityType.Vassal,
            PersonalLogActivityType.Leader,
            PersonalLogActivityType.CompletedProject
        });

        await db.SaveChangesAsync(cToken);

        return new();
    }
}