using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Services;
using StarKindred.API.Utility.Technologies;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;

namespace StarKindred.API.Endpoints.Leaders;

[ApiController]
public sealed class CompletedResearch
{
    [HttpGet("leaders/completedProjects")]
    public async Task<ApiResponse<Response>> _(
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        
        var techs = await db.UserTechnologies
            .Where(t => t.UserId == session.UserId)
            .ToListAsync(cToken);
            
        var completed = techs
            .Select(t =>
            {
                var info = TechTree.TechInfo[t.Technology];

                return new CompletedProjectDto(info.Title, t.Technology, t.CompletedOn);
            })
            .ToList();

        return new(new(completed));
    }
    
    public sealed record Response(List<CompletedProjectDto> Projects);
    public sealed record CompletedProjectDto(string Title, TechnologyType Technology, DateTimeOffset CompletedOn);
}
