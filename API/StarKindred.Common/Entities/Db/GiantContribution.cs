namespace StarKindred.Common.Entities.Db;

public class GiantContribution: IEntity
{
    public Guid Id { get; set; }
    
    public DateTimeOffset AttackDate { get; set; } = DateTimeOffset.UtcNow;
    
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public int Damage { get; set; }
}