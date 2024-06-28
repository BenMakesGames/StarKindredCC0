using StarKindred.Common.Entities.Db;
using StarKindred.API.Entities;

namespace StarKindred.API.Utility;

public static class AllianceRightsHelper
{
    public static List<AllianceRight> GetRights(Guid leaderId, UserAlliance user)
    {
        if (leaderId == user.UserId)
            return new List<AllianceRight>() { AllianceRight.TrackGiants, AllianceRight.Recruit, AllianceRight.Kick, AllianceRight.PromoteDemote };

        if(user.AllianceRank == null)
            return new();
        
        var rights = new List<AllianceRight>();

        if(user.AllianceRank.CanKick) rights.Add(AllianceRight.Kick);
        if(user.AllianceRank.CanRecruit) rights.Add(AllianceRight.Recruit);
        if(user.AllianceRank.CanTrackGiants) rights.Add(AllianceRight.TrackGiants);
        
        return rights;
    }

    public static int GetRank(Guid leaderId, UserAlliance user)
        => user.UserId == leaderId ? int.MaxValue : user.AllianceRank?.Rank ?? 0;
}