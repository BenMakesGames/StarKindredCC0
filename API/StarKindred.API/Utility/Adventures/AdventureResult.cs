using StarKindred.API.Entities;

namespace StarKindred.API.Utility.Adventures;

public sealed record AdventureResult(string? Text, List<string> Collected, List<MissionReward> Rewards);