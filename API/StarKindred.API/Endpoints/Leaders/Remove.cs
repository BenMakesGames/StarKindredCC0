using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility.Technologies;
using StarKindred.Common.Services;

namespace StarKindred.API.Endpoints.Leaders;

[ApiController]
public sealed class Remove
{
    [HttpPost("/leaders/{vassalId:guid}/remove")]
    public async Task<ApiResponse> _(
        Guid vassalId,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var leader = await db.TownLeaders
            .FirstOrDefaultAsync(l => l.UserId == session.UserId && l.VassalId == vassalId, cToken)
            ?? throw new NotFoundException("Leader not found");

        var allProjects = await db.UserResearches.Where(t => t.UserId == session.UserId).ToListAsync(cToken);

        if (allProjects.Any(r => TechTree.TechInfo[r.Technology].Category == leader.Position))
            throw new UnprocessableEntity("You must cancel the leader's project before removing them.");

        db.TownLeaders.Remove(leader);

        await db.SaveChangesAsync(cToken);

        return new();
    }
}