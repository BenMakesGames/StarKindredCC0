using StarKindred.Common.Services;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Exceptions;

namespace StarKindred.API.Services;

public interface ICurrentUser
{
    Guid? GetSessionId();
    Task<CurrentSessionDto?> GetSession(CancellationToken cToken);
    Task ClearSessionOrThrow(CancellationToken cToken);

    public sealed record CurrentSessionDto(Guid UserId, string Name, DateTimeOffset? ExpiresOn);
}

public sealed class CurrentUser: ICurrentUser
{
    private IHttpContextAccessor HttpContextAccessor { get; }
    private Db Db { get; }

    public CurrentUser(IHttpContextAccessor httpContextAccessor, Db db)
    {
        HttpContextAccessor = httpContextAccessor;
        Db = db;
    }
    
    public async Task<ICurrentUser.CurrentSessionDto?> GetSession(CancellationToken cToken)
    {
        var sessionId = GetSessionId();

        if (sessionId == null)
            return null;

        var session = await Db.UserSessions
            .Where(s => s.Id == sessionId)
            .Select(s => new ICurrentUser.CurrentSessionDto(
                s.UserId,
                s.User!.Name,
                s.ExpiresOn
            ))
            .FirstOrDefaultAsync(cToken);

        if (session == null || session.ExpiresOn < DateTimeOffset.UtcNow)
            return null;

        return new(session.UserId, session.Name, session.ExpiresOn);
    }

    public async Task ClearSessionOrThrow(CancellationToken cToken)
    {
        var sessionId = this.GetSessionIdOrThrow();

        var session = await Db.UserSessions.FirstOrDefaultAsync(s => s.Id == sessionId, cToken)
            ?? throw new NotLoggedInException("Session expired.");

        Db.UserSessions.Remove(session);
    }

    public Guid? GetSessionId()
    {
        var auth = HttpContextAccessor.HttpContext!.Request.Headers.Authorization.FirstOrDefault() ?? "";

        if (!auth.StartsWith("Bearer "))
            return null;

        if (!Guid.TryParse(auth[7..], out var sessionId))
            return null;

        return sessionId;
    }
}

public static class CurrentUserExtensions
{
    public static Guid GetSessionIdOrThrow(this ICurrentUser currentUser) => currentUser.GetSessionId() ?? throw new NotLoggedInException("Not logged in.");

    public static async Task<ICurrentUser.CurrentSessionDto> GetSessionOrThrow(this ICurrentUser currentUser, CancellationToken cToken)
        => await currentUser.GetSession(cToken) ?? throw new NotLoggedInException("Session expired.");
}