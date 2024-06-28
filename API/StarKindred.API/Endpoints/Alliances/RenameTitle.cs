using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.Alliances;

[ApiController]
public sealed class RenameTitle
{
    [HttpPost("alliances/titles/{id:guid}/rename")]
    public async Task<ApiResponse> _(
        Guid id,
        [FromBody] RequestDto request,
        [FromServices] ICurrentUser currentUser,
        [FromServices] Db db,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var alliance = await db.UserAlliances
            .Include(ua => ua.Alliance!)
                .ThenInclude(a => a.AllianceRanks)
            .Where(a => a.UserId == session.UserId)
            .Select(a => a.Alliance)
            .FirstOrDefaultAsync(cToken)
            ?? throw new UnprocessableEntity("You're not in an Alliance.");

        if (alliance.LeaderId != session.UserId)
            throw new AccessDeniedException("Only the Alliance leader may manage Titles.");

        var titleToRename = alliance.AllianceRanks!.FirstOrDefault(r => r.Id == id)
            ?? throw new UnprocessableEntity("There is no such Title.");

        var newTitle = request.Title.Trim();

        if(alliance.AllianceRanks!.Any(r => r.Title.ToLower() == newTitle.ToLower() || newTitle.ToLower() == "no title" || newTitle.ToLower() == "leader"))
            throw new UnprocessableEntity("There is already a Title with that name.");

        db.AllianceLogs.Add(new()
        {
            AllianceId = alliance.Id,
            ActivityType = AllianceLogActivityType.TitleRenamed,
            Message = $"{session.Name} renamed the Title `{titleToRename.Title}` to `{newTitle}`."
        });

        titleToRename.Title = newTitle;

        await db.SaveChangesAsync(cToken);

        return new();
    }

    public sealed record RequestDto(string Title)
    {
        public sealed class Validator : AbstractValidator<RequestDto>
        {
            public Validator()
            {
                Transform(x => x.Title, title => title.Trim())
                    .NotEmpty()
                    .WithMessage("Title must have a name.")
                    .MaximumLength(20)
                    .WithMessage("Title may not be longer than 20 characters.")
                ;
            }
        }
    }
}