namespace StarKindred.Common.Entities.Db;

public class MagicLogin: IEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public DateTimeOffset ExpiresOn { get; set; }
}