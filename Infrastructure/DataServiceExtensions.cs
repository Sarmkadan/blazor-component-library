// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

namespace BlazorComponentLibrary.Infrastructure;

/// <summary>
/// Extension methods for data service operations.
/// Provides pagination, filtering, and sorting utilities.
/// Simplifies common data manipulation patterns.
/// </summary>
public static class DataServiceExtensions
{
    /// <summary>
    /// Applies pagination to enumerable collection.
    /// Returns subset of items for the specified page.
    /// </summary>
    public static List<T> Paginate<T>(
        this IEnumerable<T> items,
        int page,
        int pageSize) where T : class
    {
        if (page <= 0 || pageSize <= 0)
            throw new ArgumentException("Page and PageSize must be greater than 0");

        return items
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    /// <summary>
    /// Applies sorting to collection based on property name.
    /// Supports ascending/descending order.
    /// </summary>
    public static IEnumerable<T> OrderByProperty<T>(
        this IEnumerable<T> items,
        string propertyName,
        bool descending = false) where T : class
    {
        if (string.IsNullOrEmpty(propertyName))
            return items;

        var property = typeof(T).GetProperty(propertyName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public);
        if (property == null)
            return items;

        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T));
        var propertyAccess = System.Linq.Expressions.Expression.Property(parameter, property);
        var lambda = System.Linq.Expressions.Expression.Lambda(propertyAccess, parameter);

        var orderByMethod = descending ? "OrderByDescending" : "OrderBy";
        var method = typeof(System.Linq.Enumerable)
            .GetMethods()
            .First(m => m.Name == orderByMethod && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.PropertyType);

        return (IEnumerable<T>)method.Invoke(null, new object[] { items, lambda.Compile() })!;
    }

    /// <summary>
    /// Applies multiple filter conditions to collection.
    /// Uses predicate builder pattern.
    /// </summary>
    public static IEnumerable<T> ApplyFilters<T>(
        this IEnumerable<T> items,
        Dictionary<string, object>? filters) where T : class
    {
        if (filters == null || filters.Count == 0)
            return items;

        var result = items;

        foreach (var filter in filters)
        {
            var property = typeof(T).GetProperty(filter.Key, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public);
            if (property == null)
                continue;

            result = result.Where(item =>
            {
                var value = property.GetValue(item);
                return value?.Equals(filter.Value) ?? (filter.Value == null);
            });
        }

        return result;
    }

    /// <summary>
    /// Searches collection by multiple fields.
    /// Case-insensitive string matching.
    /// </summary>
    public static IEnumerable<T> Search<T>(
        this IEnumerable<T> items,
        string searchTerm,
        params string[] searchableFields) where T : class
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || searchableFields.Length == 0)
            return items;

        var term = searchTerm.ToLower();

        return items.Where(item =>
        {
            foreach (var fieldName in searchableFields)
            {
                var property = typeof(T).GetProperty(fieldName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public);
                if (property != null)
                {
                    var value = property.GetValue(item)?.ToString()?.ToLower();
                    if (value?.Contains(term) == true)
                        return true;
                }
            }
            return false;
        });
    }

    /// <summary>
    /// Applies complete data query with filtering, sorting, and pagination.
    /// Chains all operations efficiently.
    /// </summary>
    public static PagedResult<T> Query<T>(
        this IEnumerable<T> items,
        string? searchTerm = null,
        Dictionary<string, object>? filters = null,
        string? sortBy = null,
        bool sortDescending = false,
        int page = 1,
        int pageSize = 10,
        params string[] searchableFields) where T : class
    {
        var query = items;

        // Apply search
        if (!string.IsNullOrWhiteSpace(searchTerm) && searchableFields.Length > 0)
        {
            query = query.Search(searchTerm, searchableFields);
        }

        // Apply filters
        if (filters?.Count > 0)
        {
            query = query.ApplyFilters(filters);
        }

        // Get total count before pagination
        var totalCount = query.Count();

        // Apply sorting
        if (!string.IsNullOrEmpty(sortBy))
        {
            query = query.OrderByProperty(sortBy, sortDescending);
        }

        // Apply pagination
        var items_paged = query.Paginate(page, pageSize);

        return new PagedResult<T>
        {
            Items = items_paged,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (totalCount + pageSize - 1) / pageSize
        };
    }

    /// <summary>
    /// Batches collection into chunks for processing.
    /// Useful for batch operations and bulk inserts.
    /// </summary>
    public static IEnumerable<List<T>> BatchItems<T>(
        this IEnumerable<T> items,
        int batchSize) where T : class
    {
        if (batchSize <= 0)
            throw new ArgumentException("Batch size must be greater than 0");

        var batch = new List<T>(batchSize);

        foreach (var item in items)
        {
            batch.Add(item);
            if (batch.Count >= batchSize)
            {
                yield return batch;
                batch = new List<T>(batchSize);
            }
        }

        if (batch.Count > 0)
            yield return batch;
    }

    /// <summary>
    /// Selects distinct items by specified key.
    /// </summary>
    public static IEnumerable<T> DistinctBy<T, TKey>(
        this IEnumerable<T> items,
        Func<T, TKey> keySelector) where T : class
    {
        var seen = new HashSet<TKey>();

        foreach (var item in items)
        {
            var key = keySelector(item);
            if (seen.Add(key!))
                yield return item;
        }
    }

    /// <summary>
    /// Groups items and counts occurrences.
    /// </summary>
    public static Dictionary<TKey, int> CountBy<T, TKey>(
        this IEnumerable<T> items,
        Func<T, TKey> keySelector) where T : class where TKey : notnull
    {
        return items
            .GroupBy(keySelector)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}

/// <summary>
/// Represents a page of results with pagination metadata.
/// </summary>
public class PagedResult<T> where T : class
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
