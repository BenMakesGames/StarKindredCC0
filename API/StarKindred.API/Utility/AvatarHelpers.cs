using StarKindred.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace StarKindred.API.Utility;

public static class AvatarHelpers
{
    public static readonly string[] Avatars = {
        "black-and-white/golden-ratio",
        "black-and-white/leaves",
        "black-and-white/music-notes",
        "black-and-white/owl",
        "black-and-white/peach"
    };

    public static readonly string[] SubscriptionAvatars =
    {
        "subscriber/focused",
        "subscriber/heart-dimension",
        "subscriber/ok",
        "subscriber/redhead"
    };

    public static async Task<bool> UnlockAvatar(Db db, Guid userId, string avatar, CancellationToken cToken)
    {
        var existingUnlock = await db.UserUnlockedAvatars
            .FirstOrDefaultAsync(a => a.Avatar == avatar && a.UserId == userId, cToken);

        if (existingUnlock != null)
            return false;

        db.UserUnlockedAvatars.Add(new()
        {
            Avatar = avatar,
            UserId = userId,
        });

        return true;
    }

    public static async Task<List<string>> GetAvailable(Db db, Guid userId, CancellationToken cToken)
    {
        var now = DateTimeOffset.UtcNow;
        var available = new List<string>();

        var unlocked = await db.UserUnlockedAvatars
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.UnlockedOn)
            .Select(a => a.Avatar)
            .ToListAsync(cToken);

        available.AddRange(unlocked);

        var isSubscribed = await db.UserSubscriptions
            .AnyAsync(s => s.UserId == userId && s.StartDate <= now && s.EndDate >= now, cToken);

        if (isSubscribed)
            available.AddRange(SubscriptionAvatars);

        available.AddRange(Avatars);

        return available;
    }
}
