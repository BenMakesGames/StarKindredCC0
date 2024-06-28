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
public sealed class ChangePassphrase
{
    [HttpPost("/accounts/changePassphrase")]
    public async Task<ApiResponse> _(
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        [FromServices] IPassphraseHasher passphraseHasher,
        Request request,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        var user = await db.Users.FirstAsync(u => u.Id == session.UserId, cToken);

        if (passphraseHasher.Verify(request.NewPassphrase, user.Passphrase))
            throw new UnprocessableEntity("That's already your passphrase!");

        user.Passphrase = passphraseHasher.Hash(request.NewPassphrase);

        PersonalLogHelper.Create(db, session.UserId, $"You changed your Passphrase.", new[]
        {
            PersonalLogActivityType.AccountActivity,
            PersonalLogActivityType.UpdatedPassphrase
        });

        await db.SaveChangesAsync(cToken);

        return new();
    }

    public sealed record Request(string NewPassphrase)
    {
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.NewPassphrase).MinimumLength(10).WithMessage("New passphrase must be at least 10 characters.");
            }
        }
    }
}