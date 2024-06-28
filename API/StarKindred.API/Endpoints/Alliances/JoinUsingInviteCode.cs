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
public sealed class JoinUsingInviteCode
{
    [HttpPost("alliances/join")]
    public async Task<ApiResponse<Response>> _(
        Request request,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        if (await db.UserAlliances.AnyAsync(ua => ua.UserId == session.UserId, cToken))
            throw new UnprocessableEntity("You're already in an Alliance. You must leave your current Alliance before joining a new one.");

        var recruiting = await db.AllianceRecruitStatuses
            .FirstOrDefaultAsync(s => s.InviteCodeActive && s.InviteCode.ToLower() == request.InviteCode.ToLower(), cToken)
            ?? throw new NotFoundException("That Invite Code is not valid.");

        var memberCount = await db.UserAlliances.CountAsync(ua => ua.AllianceId == recruiting.AllianceId, cToken);

        if (memberCount >= Alliance.MaxMemberCount)
            throw new NotFoundException("That Invite Code is not valid.");

        db.UserAlliances.Add(new()
        {
            UserId = session.UserId,
            AllianceId = recruiting.AllianceId,
        });

        db.AllianceLogs.Add(new()
        {
            AllianceId = recruiting.AllianceId,
            ActivityType = AllianceLogActivityType.NewMember,
            Message = $"{session.Name} joined the Alliance!"
        });

        if (memberCount + 1 >= Alliance.MaxMemberCount)
        {
            recruiting.InviteCodeActive = false;
            recruiting.OpenInvitationActive = false;
        }

        await db.SaveChangesAsync(cToken);

        return new(new(recruiting.AllianceId));
    }

    public sealed record Request(string InviteCode)
    {
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.InviteCode).Length(7).WithMessage("Invite Codes are always exactly 7 characters.");
            }
        }
    }

    public sealed record Response(Guid AllianceId);
}