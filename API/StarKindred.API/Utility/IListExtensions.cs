using System.Text;

namespace StarKindred.API.Utility;

public static class IListExtensions
{
    public static string ToNiceString<T>(this IList<T> list)
    {
        var sb = new StringBuilder();

        for(int i = 0; i < list.Count; i++)
        {
            sb.Append(list[i]);

            if (i == list.Count - 2)
                sb.Append(", and ");
            else if(i < list.Count - 1)
                sb.Append(", ");
        }

        return sb.ToString();
    }
}