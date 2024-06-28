namespace StarKindred.Common.Entities.Db;

public class UserAdventureStepCompleted: IEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid AdventureStepId { get; set; }
    public AdventureStep? AdventureStep { get; set; }

    public DateTimeOffset CompletedOn { get; set; } = DateTimeOffset.UtcNow;
}