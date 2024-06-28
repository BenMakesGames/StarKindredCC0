using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;

namespace StarKindred.API.Endpoints.Accounts;

[ApiController]
public sealed class SendMagicEmailLink
{
    [HttpPost("/accounts/emailMagicLink")]
    public async Task<ApiResponse> _(
        [FromBody] Request request,
        [FromServices] IStarKindredMailer mailer,
        [FromServices] Db db,
        [FromServices] IAddressHelper addressHelper,
        CancellationToken cToken
    )
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cToken)
            ?? throw new NotFoundException("An account does not exist with that email address.");

        var magicLogins = await db.MagicLogins
            .Where(l => l.UserId == user.Id && l.ExpiresOn > DateTime.UtcNow)
            .CountAsync(cToken);

        if(magicLogins >= 3)
            throw new UnprocessableEntity("You have already requested 3 magic logins. Please try again later.");

        var newMagicLogin = new MagicLogin()
        {
            UserId = user.Id,
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
        };

        db.MagicLogins.Add(newMagicLogin);

        await db.SaveChangesAsync(cToken);

        var linkUrl = $"{addressHelper.ServerUrl}/accounts/magicLogIn?token=" + newMagicLogin.Id;

        await mailer.SendEmailAsync(
            user.Email,
            "★Kindred Magic Login",

            "You requested a magic login link... and here it is!\n\n" +
            $"Click to log in: {linkUrl} ",

            "<p>You requested a magic login... and here it is!</p>" +
            $"<p><a href=\"{linkUrl}\" style=\"display:inline-block;color:#eee;text-decoration:none;background-color:#900;padding:0.5em 1em;border-radius:1.1em;\">Log In</a></p>",

            CancellationToken.None
        );

        return new();
    }

    public sealed record Request(string Email)
    {
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Must provide an email address.");
            }
        }
    }
}