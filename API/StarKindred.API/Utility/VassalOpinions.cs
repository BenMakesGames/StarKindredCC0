using StarKindred.Common.Entities;

namespace StarKindred.API.Utility;

public static class VassalOpinions
{
    public static readonly Dictionary<VassalOpinionKey, string> Lines = new()
    {
        { new(AstrologicalSign.Cat, Nature.Loner, MissionType.Oracle), "That Oracle talks much too much. Living out here in the mountains must be nice, though..." },
        { new(AstrologicalSign.Crown, Nature.Loner, MissionType.Oracle), "The Oracle helps guide our people... I just wish they weren't such a hassle to listen to." },
        { new(AstrologicalSign.Kundrav, Nature.Loner, MissionType.Oracle), "It takes all my strength to listen to that Oracle prattle on..." },
        { new(AstrologicalSign.Mountain, Nature.Loner, MissionType.Oracle), "I could make a good Oracle. Sitting out on the mountain top, dispensing wisdom on my own terms? Ah!" },
    };
}

public sealed record VassalOpinionKey(AstrologicalSign Sign, Nature Nature, MissionType Mission);