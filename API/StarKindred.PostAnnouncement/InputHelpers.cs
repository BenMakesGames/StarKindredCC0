using StarKindred.Common.Entities;

namespace StarKindred.PostAnnouncement;

public static class InputHelpers
{
    public static AnnouncementType GetAnnouncementType()
    {
        do
        {
            var selection = Console.ReadLine()?.Trim().ToLower();

            if(selection == "c") return AnnouncementType.ChangeLog;
            if(selection == "s") return AnnouncementType.ServerIssue;
        } while (true);
    }
}