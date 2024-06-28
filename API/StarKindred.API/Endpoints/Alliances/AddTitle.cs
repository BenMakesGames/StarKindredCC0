using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;

namespace StarKindred.API.Endpoints.Alliances;

[ApiController]
public sealed class AddTitle
{
    [HttpPost("alliances/titles")]
    public async Task<ApiResponse<TitleDto>> _(
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
            throw new AccessDeniedException("Only the Alliance leader may manage Title.");

        if (alliance.AllianceRanks!.Count >= 5)
            throw new UnprocessableEntity("An Alliance may only have up to 5 Titles.");

        var titleName = request.Title.Trim();

        if(alliance.AllianceRanks!.Any(r => r.Title.ToLower() == titleName.ToLower() || titleName.ToLower() == "no title" || titleName.ToLower() == "leader"))
            throw new UnprocessableEntity("There is already a Title with that name.");

        var title = new AllianceRank()
        {
            AllianceId = alliance.Id,
            Title = titleName,
            Rank = request.Rank,
            CanRecruit = request.CanRecruit,
            CanKick = request.CanKick,
            CanTrackGiants = request.CanTrackGiants,
        };

        db.AllianceRanks.Add(title);

        db.AllianceLogs.Add(new()
        {
            AllianceId = alliance.Id,
            ActivityType = AllianceLogActivityType.TitleCreated,
            Message = $"{session.Name} created a new Title: `{titleName}` (Rank {request.Rank})."
        });

        await db.SaveChangesAsync(cToken);

        return new(new(
            title.Id,
            title.Title,
            title.Rank,
            title.CanRecruit,
            title.CanKick,
            title.CanTrackGiants
        ));
    }

    public sealed record RequestDto(string Title, int Rank, bool CanRecruit, bool CanKick, bool CanTrackGiants)
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

                RuleFor(x => x.Rank).InclusiveBetween(0, 9999).WithMessage("Rank must be between 0 and 9999.");
            }
        }
    }

    public sealed record TitleDto(Guid Id, string Title, int Rank, bool CanRecruit, bool CanKick, bool CanTrackGiants);
}