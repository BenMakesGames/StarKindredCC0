using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Accounts;

[ApiController]
public sealed class MagicLogIn
{
    [HttpGet("/accounts/magicLogIn")]
    public async Task<IActionResult> _(
        [FromQuery] Guid token, [FromServices] Db db,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAddressHelper addressHelper,
        CancellationToken cToken
    )
    {
        var magicLogin = await db.MagicLogins.FirstOrDefaultAsync(l => l.Id == token, cToken);

        if (magicLogin == null)
            return new RedirectResult($"{addressHelper.ClientUrl}/magicLogin?result=invalid");

        if(magicLogin.ExpiresOn <= DateTime.UtcNow)
            return new RedirectResult($"{addressHelper.ClientUrl}/magicLogin?result=expired");

        await UserHelper.DeleteOldSessionsAndMagicLinks(db, magicLogin.UserId, cToken);

        var session = new UserSession()
        {
            UserId = magicLogin.UserId,
            ExpiresOn = DateTimeOffset.UtcNow.AddDays(3)
        };

        db.MagicLogins.Remove(magicLogin);
        db.UserSessions.Add(session);

        PersonalLogHelper.Create(db, session.UserId, $"You logged in, using a magic login link, from `{httpContextAccessor.HttpContext!.Connection.RemoteIpAddress}`.", new[]
        {
            PersonalLogActivityType.AccountActivity,
            PersonalLogActivityType.LoggedIn
        });
        
        await db.SaveChangesAsync(cToken);

        return new RedirectResult($"{addressHelper.ClientUrl}/magicLogin?result={session.Id}");
    }
}
