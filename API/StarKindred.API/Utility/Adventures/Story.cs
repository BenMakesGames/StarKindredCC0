namespace StarKindred.API.Utility.Adventures;

public static class Story
{
    public static AdventureResult Do(string narrative)
    {
        return new(narrative, new(), new());
    }
}