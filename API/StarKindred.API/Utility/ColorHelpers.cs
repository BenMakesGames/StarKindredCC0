namespace StarKindred.API.Utility;

public static class ColorHelpers
{
    public static bool IsValid(string color) =>
        color.Length == 6 &&
        color.All(c => c is (>= '0' and <= '9') or (>= 'a' and <= 'f') or (>= 'A' and <= 'F'))
    ;
}
