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
public sealed class Research
{
    [HttpPost("/leaders/research")]
    public async Task<ApiResponse> _(
        Request request,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var info = TechTree.TechInfo[request.Research];

        var leader = await db.TownLeaders
            .Where(l => l.UserId == session.UserId && l.Position == info.Category)
            .Select(l => l.Vassal)
            .FirstOrDefaultAsync(cToken)
            ?? throw new UnprocessableEntity("No Vassal has been appointed to that position.");

        var currentlyResearching = await db.UserResearches.Where(t => t.UserId == session.UserId).ToListAsync(cToken);

        if (currentlyResearching.Any(c => TechTree.TechInfo[c.Technology].Category == info.Category))
            throw new UnprocessableEntity($"Your {info.Category} Leader is already working on a project.");

        if(await db.UserTechnologies.AnyAsync(t => t.UserId == session.UserId && t.Technology == request.Research, cToken))
            throw new UnprocessableEntity("You've already researched that technology!");

        if (info.PreReq != null)
        {
            if (!await db.UserTechnologies.AnyAsync(t => t.UserId == session.UserId && t.Technology == info.PreReq, cToken))
                throw new UnprocessableEntity("You don't meet the prereqs for that project!");
        }

        var cost = info.ResearchCost(leader.Nature);
        var costTypes = cost.Select(c => c.Type).ToList();

        var resources = await db.Resources
            .Where(r => r.UserId == session.UserId && costTypes.Contains(r.Type))
            .ToListAsync(cToken);

        ResourceHelper.PayOrThrow(resources, cost);

        db.UserResearches.Add(new()
        {
            UserId = session.UserId,
            Technology = request.Research,
        });

        PersonalLogHelper.Create(db, session.UserId, $"{leader.Name} started work on the {info.Title} project.", new[]
        {
            PersonalLogActivityType.Vassal,
            PersonalLogActivityType.Leader,
            PersonalLogActivityType.StartedProject
        });

        await db.SaveChangesAsync(cToken);

        return new();
    }

    public sealed record Request(TechnologyType Research);
}