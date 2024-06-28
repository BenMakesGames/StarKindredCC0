using StarKindred.Common.Entities;
using StarKindred.Common.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Accounts;

[ApiController]
public sealed class ChangeEmail
{
    [HttpPost("/accounts/changeEmail")]
    public async Task<ApiResponse<Response>> _(
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        [FromServices] IPassphraseHasher passphraseHasher,
        [FromBody] Request request,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        var user = await db.Users.FirstAsync(u => u.Id == session.UserId, cToken);

        var newEmail = request.Email.Trim();

        if(user.Email == newEmail)
            throw new UnprocessableEntity("That's already your e-mail address!");

        if(await db.Users.AnyAsync(u => u.Email == newEmail, cToken))
            throw new UnprocessableEntity("That e-mail address is already in use.");

        var oldEmail = user.Email;

        user.Email = newEmail;

        PersonalLogHelper.Create(db, session.UserId, $"You changed your Email Address from `{oldEmail}` to `{newEmail}`.", new[]
        {
            PersonalLogActivityType.AccountActivity,
            PersonalLogActivityType.UpdatedEmail
        });

        await db.SaveChangesAsync(cToken);

        return new(new(newEmail));
    }

    public sealed record Response(string Email);

    public sealed record Request(string Email)
    {
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                Transform(x => x.Email, n => n.Trim())
                    .NotEmpty().EmailAddress().WithMessage("Must provide an email address.")
                    .MaximumLength(100).WithMessage("Your email address must be 100 characters or less.")
                ;
            }
        }
    }
}