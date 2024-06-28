using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;

namespace StarKindred.API.Utility;

public static class IQueryableExtensionsForPaginatedResults
{
    public static PaginatedResults<T> AsPaginatedResults<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        var totalNumberOfRecords = query.Count();
        
        var results = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        return new PaginatedResults<T>(results, pageNumber, pageSize, totalNumberOfRecords);
    }

    public static async Task<PaginatedResults<T>> AsPaginatedResultsAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var totalNumberOfRecords = query.Count();
        
        var results = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return new PaginatedResults<T>(results, pageNumber, pageSize, totalNumberOfRecords);
    }
}