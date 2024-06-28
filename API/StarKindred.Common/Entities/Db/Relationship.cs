namespace StarKindred.Common.Entities.Db;

public class Relationship: IEntity
{
    public Guid Id { get; set; }

    public int Level { get; set; }
    public int Minutes { get; set; }

    public List<Vassal>? Vassals { get; set; }
}