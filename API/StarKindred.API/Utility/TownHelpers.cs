using StarKindred.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace StarKindred.API.Utility;

public static class TownHelpers
{
    public static readonly List<int> LevelRequiredForPosition = new()
    {
        0, 0, 0, 0, 1, 1, 1, 2, 2, 1, 2
    };

    public static async Task<bool> MakeDecorable(Db db, Guid userId, CancellationToken cToken)
    {
        var town = await db.Towns.FirstAsync(t => t.UserId == userId, cToken);

        if (town.CanDecorate)
            return false;
        
        town.CanDecorate = true;

        return true;
    }
}
