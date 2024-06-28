using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Alliances;

[ApiController]
public sealed class Create
{
    [HttpPost("/alliances/create")]
    public async Task<ApiResponse<ResponseDto>> _(
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Random rng,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        if(await db.UserAlliances.AnyAsync(ua => ua.UserId == session.UserId, cToken))
            throw new UnprocessableEntity("You are already in an Alliance.");

        var palace = await db.Buildings.FirstOrDefaultAsync(b => b.Type == BuildingType.Palace && b.UserId == session.UserId, cToken);
        
        if(palace is not { Level: >= 10 })
            throw new AccessDeniedException("Your Palace must be level 10 or higher to create an Alliance.");
        
        var resources = await db.Resources
            .Where(r => r.UserId == session.UserId && r.Type == ResourceType.Gold)
            .ToListAsync(cToken);

        ResourceHelper.PayOrThrow(resources, new() { new(ResourceType.Gold, 1000) });
        
        var alliance = new Alliance()
        {
            Name = "",
            LeaderId = session.UserId,
        };

        var membership = new UserAlliance()
        {
            Alliance = alliance,
            UserId = session.UserId,
        };

        db.Alliances.Add(alliance);
        db.UserAlliances.Add(membership);
        
        db.AllianceRecruitStatuses.Add(new()
        {
            Alliance = alliance,
            InviteCode = InviteCodeGenerator.Generate(rng),
        });

        db.AllianceLogs.Add(new()
        {
            ActivityType = AllianceLogActivityType.AllianceCreated,
            Alliance = alliance,
            Message = $"The Alliance was created; it was created by {session.Name}.",
        });

        await db.SaveChangesAsync(cToken);

        return new(new(alliance.Id));
    }

    public sealed record ResponseDto(Guid AllianceId);
}