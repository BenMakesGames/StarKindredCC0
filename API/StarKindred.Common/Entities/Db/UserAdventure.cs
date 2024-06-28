namespace StarKindred.Common.Entities.Db;

public class UserAdventure: IEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid AdventureId { get; set; }
    public Adventure? Adventure { get; set; }
}