namespace StarKindred.API.Entities;

public sealed record MissionReward(string Image, int Quantity = 1)
{
    public static List<MissionReward> CreateFromResources(List<ResourceQuantity> resourceQuantities)
    {
        return resourceQuantities
            .Where(q => q.Quantity > 0)
            .Select(q => new MissionReward($"resources/{q.Type.ToString().ToLower()}", q.Quantity))
            .ToList()
        ;
    }
}