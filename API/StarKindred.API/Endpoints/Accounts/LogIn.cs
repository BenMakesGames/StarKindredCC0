using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Accounts;

[ApiController]
public sealed class LogIn
{
    [HttpPost("/accounts/logIn")]
    public async Task<ApiResponse<Response>> _(
        Request request, [FromServices] Db db, [FromServices] IPassphraseHasher passphraseHasher,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken cToken
    )
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email.Trim(), cToken);

        if (user == null || !passphraseHasher.Verify(request.Passphrase, user.Passphrase))
            throw new UnprocessableEntity("Email address and/or passphrase is incorrect.");

        await UserHelper.DeleteOldSessionsAndMagicLinks(db, user.Id, cToken);

        var session = new UserSession()
        {
            UserId = user.Id,
            ExpiresOn = DateTimeOffset.UtcNow.AddDays(3)
        };

        db.UserSessions.Add(session);
        
        PersonalLogHelper.Create(db, session.UserId, $"You logged in, using your passphrase, from `{httpContextAccessor.HttpContext!.Connection.RemoteIpAddress}`.", new[]
        {
            PersonalLogActivityType.AccountActivity,
            PersonalLogActivityType.LoggedIn
        });
        
        await db.SaveChangesAsync(cToken);

        return new(new(session.Id));
    }

    public sealed record Request(string Email, string Passphrase)
    {
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Must provide an email address.");
                RuleFor(x => x.Passphrase).MinimumLength(10).WithMessage("Passphrase must be at least 10 characters.");
            }
        }
    }

    public sealed record Response(Guid SessionId);
}