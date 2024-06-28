namespace StarKindred.Common.Entities.Db;

public class UserAdventureStepInProgress: IEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid AdventureStepId { get; set; }
    public AdventureStep? AdventureStep { get; set; }

    public DateTimeOffset StartedOn { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset CompletesOn { get; set; }

    public List<Vassal>? Vassals { get; set; }
}