namespace StarKindred.Common.Extensions;

public static class StringExtensions
{
    public static string UppercaseFirst(this string text)
        => string.IsNullOrEmpty(text) ? string.Empty : (char.ToUpper(text[0]) + text[1..]);
}