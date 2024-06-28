using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace StarKindred.API.Utility;

public static class UserHelper
{
    public static async Task ComputeLevel(Db db, User user, CancellationToken cToken)
    {
        var highestThreeTotal = await db.Vassals
            .Where(v => v.UserId == user.Id)
            .OrderByDescending(v => v.Level)
            .Take(3)
            .SumAsync(v => v.Level, cToken);

        user.Level = highestThreeTotal;
    }

    public static async Task DeleteOldSessionsAndMagicLinks(Db db, Guid userId, CancellationToken cToken)
    {
        var oldSessions = await db.UserSessions.Where(s => s.UserId == userId && s.ExpiresOn < DateTimeOffset.UtcNow).ToListAsync(cToken);
        var oldMagicLogins = await db.MagicLogins.Where(l => l.UserId == userId && l.ExpiresOn < DateTimeOffset.UtcNow).ToListAsync(cToken);

        db.UserSessions.RemoveRange(oldSessions);
        db.MagicLogins.RemoveRange(oldMagicLogins);
    }
}