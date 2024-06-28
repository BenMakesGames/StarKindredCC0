namespace StarKindred.Common.Entities.Db;

public class AllianceRank: IEntity
{
    public Guid Id { get; set; }

    public Guid AllianceId { get; set; }
    public Alliance? Alliance { get; set; }

    public int Rank { get; set; }

    public string Title { get; set; } = null!;

    public bool CanRecruit { get; set; }
    public bool CanKick { get; set; }
    public bool CanTrackGiants { get; set; }

    public List<UserAlliance>? UserAlliances { get; set; }
}