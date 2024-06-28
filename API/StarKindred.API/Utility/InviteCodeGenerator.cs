using BenMakesGames.RandomHelpers;

namespace StarKindred.API.Utility;

public static class InviteCodeGenerator
{
    public const string Letters = "ABCDEFGHJMNPQRTWY34678";

    public static string Generate(Random rng) => $"{rng.NextString(Letters, 7)}";
}