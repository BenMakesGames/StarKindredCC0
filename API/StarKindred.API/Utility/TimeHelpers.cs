namespace StarKindred.API.Utility;

public static class TimeHelpers
{
    public static string DescribeInterval(double totalSeconds)
    {
        var seconds = (int)totalSeconds;
        var parts = new List<string>();

        if(seconds > 60 * 60 * 24)
        {
            parts.Add($"{seconds / (60 * 60 * 24)} days");
            seconds %= 60 * 60 * 24;
        }

        if(seconds > 60 * 60 && parts.Count < 2)
        {
            parts.Add($"{seconds / (60 * 60)} hours");
            seconds %= 60 * 60;
        }

        if(seconds > 60 && parts.Count < 2)
        {
            parts.Add($"{seconds / 60} minutes");
            seconds %= 60;
        }

        if(parts.Count < 2)
            parts.Add($"{seconds} seconds");

        return string.Join(' ', parts);
    }
}