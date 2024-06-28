using StarKindred.Common.Entities;

namespace StarKindred.API.Entities;

public sealed record ResourceQuantity(ResourceType Type, int Quantity);
public sealed record TreasureQuantity(TreasureType Type, int Quantity);