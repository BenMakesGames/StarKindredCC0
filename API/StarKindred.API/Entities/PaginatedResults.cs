namespace StarKindred.API.Entities;

public sealed record PaginatedResults<T>(IList<T> Results, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
