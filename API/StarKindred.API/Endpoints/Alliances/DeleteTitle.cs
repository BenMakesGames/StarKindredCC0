using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.Alliances;

[ApiController]
public sealed class DeleteTitle
{
    [HttpPost("alliances/titles/{id:guid}/delete")]
    public async Task<ApiResponse> _(
        Guid id,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var alliance = await db.UserAlliances
            .Include(ua => ua.Alliance!)
                .ThenInclude(a => a.AllianceRanks!)
            .Where(a => a.UserId == session.UserId)
            .Select(a => a.Alliance)
            .FirstOrDefaultAsync(cToken)
            ?? throw new UnprocessableEntity("You're not in an Alliance.");

        if (alliance.LeaderId != session.UserId)
            throw new AccessDeniedException("Only the Alliance leader may manage Titles.");

        var rankToDelete = alliance.AllianceRanks!.FirstOrDefault(r => r.Id == id)
            ?? throw new UnprocessableEntity("There is no such Title.");

        db.AllianceLogs.Add(new()
        {
            AllianceId = alliance.Id,
            ActivityType = AllianceLogActivityType.TitleDeleted,
            Message = $"{session.Name} deleted the Title `{rankToDelete.Title}`."
        });

        db.AllianceRanks.Remove(rankToDelete);

        await db.SaveChangesAsync(cToken);

        return new();
    }

}