using Microsoft.EntityFrameworkCore;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}
public static class IQueryableExtensions
{
    public static async Task<PagedResult<T>> GetPagedAsync<T>(this IQueryable<T> query, int page, int pageSize)
    {
        var result = new PagedResult<T>();
        result.CurrentPage = page;
        result.PageSize = pageSize;
        result.TotalCount = await query.CountAsync(); // Total item count
        result.TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize);

        result.Items = await query.Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToListAsync(); // Get the paged result

        return result;
    }
}