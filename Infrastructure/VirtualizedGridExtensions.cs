// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;

namespace BlazorComponentLibrary.Infrastructure;

/// <summary>
/// LINQ extension methods that form the server-side query pipeline for the virtualized data grid.
/// <para>
/// The three core operators — <see cref="ApplyGridFilters"/>, <see cref="ApplyGridSearch"/>, and
/// <see cref="ApplyGridSort"/> — are chained sequentially inside
/// <see cref="Services.VirtualizedGridService.QueryAsync"/> to transform a flat
/// <see cref="DataTableRow"/> enumerable into the filtered, sorted slice that is returned to the
/// client for rendering.
/// </para>
/// <para>
/// All operators are lazy (deferred LINQ) and allocate no intermediate collections until
/// materialised by a downstream <c>.ToList()</c> call, keeping the per-query allocation
/// profile proportional to the number of rows that survive the filter stage.
/// </para>
/// </summary>
public static class VirtualizedGridExtensions
{
    // ── Public operators ──────────────────────────────────────────────────────

    /// <summary>
    /// Filters <paramref name="rows"/> by applying every descriptor in <paramref name="filters"/>
    /// as a logical AND — a row survives only when it satisfies <em>all</em> active criteria.
    /// Returns the original sequence unchanged when the filter list is <c>null</c> or empty.
    /// </summary>
    /// <param name="rows">Source row enumerable produced by the data repository.</param>
    /// <param name="filters">
    ///   Column filter descriptors to evaluate.  An empty or <c>null</c> list is a no-op.
    ///   See <see cref="FilterOperator"/> for the full set of supported predicates.
    /// </param>
    /// <returns>Lazy sequence of rows that satisfy every active filter predicate.</returns>
    public static IEnumerable<DataTableRow> ApplyGridFilters(
        this IEnumerable<DataTableRow> rows,
        IReadOnlyList<GridFilterDescriptor> filters)
    {
        if (filters is not { Count: > 0 })
            return rows;

        return rows.Where(row => filters.All(f => EvaluateFilter(row, f)));
    }

    /// <summary>
    /// Retains only rows that contain <paramref name="searchTerm"/> (case-insensitive substring
    /// match) in at least one of the nominated <paramref name="searchFields"/>.
    /// <para>
    /// When <paramref name="searchFields"/> is <c>null</c> or empty, every key present in the
    /// row's <see cref="DataTableRow.Data"/> dictionary is considered as a candidate field.
    /// </para>
    /// Returns the original sequence unchanged when <paramref name="searchTerm"/> is absent or
    /// whitespace-only.
    /// </summary>
    /// <param name="rows">Source row enumerable.</param>
    /// <param name="searchTerm">Case-insensitive substring to locate within cell values.</param>
    /// <param name="searchFields">
    ///   Column keys to restrict the search to.  <c>null</c> searches every field.
    /// </param>
    /// <returns>Lazy sequence of rows whose data contains the search term.</returns>
    public static IEnumerable<DataTableRow> ApplyGridSearch(
        this IEnumerable<DataTableRow> rows,
        string? searchTerm,
        IReadOnlyList<string>? searchFields)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return rows;

        return rows.Where(row => MatchesSearch(row, searchTerm, searchFields));
    }

    /// <summary>
    /// Sorts <paramref name="rows"/> according to the supplied <paramref name="sorts"/> list.
    /// <para>
    /// Multiple descriptors produce a multi-column sort; they are applied in ascending
    /// <see cref="GridSortDescriptor.Priority"/> order so that the descriptor with the lowest
    /// priority value acts as the primary sort key, the next as the secondary key, and so on.
    /// </para>
    /// <para>
    /// Sorting is type-aware: values that implement <see cref="IComparable"/> (numerics, dates,
    /// strings) are compared natively.  When native comparison is not possible the cell values
    /// are compared as ordinal strings.  <c>null</c> cells always sort before non-null cells
    /// regardless of direction.
    /// </para>
    /// Returns the original sequence unchanged when the sort list is <c>null</c> or empty.
    /// </summary>
    /// <param name="rows">Source row enumerable.</param>
    /// <param name="sorts">Sort descriptors to apply.  An empty or <c>null</c> list is a no-op.</param>
    /// <returns>Lazy ordered sequence of rows.</returns>
    public static IEnumerable<DataTableRow> ApplyGridSort(
        this IEnumerable<DataTableRow> rows,
        IReadOnlyList<GridSortDescriptor> sorts)
    {
        if (sorts is not { Count: > 0 })
            return rows;

        var prioritised = sorts.OrderBy(s => s.Priority).ToList();
        var primary     = prioritised[0];

        IOrderedEnumerable<DataTableRow> ordered = primary.Direction == SortDirection.Ascending
            ? rows.OrderBy(r => r.Data.GetValueOrDefault(primary.Field), NaturalSortComparer.Instance)
            : rows.OrderByDescending(r => r.Data.GetValueOrDefault(primary.Field), NaturalSortComparer.Instance);

        foreach (var sort in prioritised.Skip(1))
        {
            var s = sort; // local capture for each iteration's closure
            ordered = s.Direction == SortDirection.Ascending
                ? ordered.ThenBy(r => r.Data.GetValueOrDefault(s.Field), NaturalSortComparer.Instance)
                : ordered.ThenByDescending(r => r.Data.GetValueOrDefault(s.Field), NaturalSortComparer.Instance);
        }

        return ordered;
    }

    // ── Private evaluation helpers ────────────────────────────────────────────

    private static bool EvaluateFilter(DataTableRow row, GridFilterDescriptor filter)
    {
        row.Data.TryGetValue(filter.Field, out var rawValue);

        return filter.Operator switch
        {
            FilterOperator.IsNull    => rawValue is null,
            FilterOperator.IsNotNull => rawValue is not null,
            _                        => rawValue is not null && EvaluateNonNullFilter(rawValue, filter)
        };
    }

    private static bool EvaluateNonNullFilter(object rawValue, GridFilterDescriptor filter)
    {
        var comparison = filter.CaseSensitive
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        var cellStr   = rawValue.ToString() ?? string.Empty;
        var filterStr = filter.Value?.ToString() ?? string.Empty;

        return filter.Operator switch
        {
            FilterOperator.Equals             => string.Equals(cellStr, filterStr, comparison),
            FilterOperator.NotEquals          => !string.Equals(cellStr, filterStr, comparison),
            FilterOperator.Contains           => cellStr.Contains(filterStr, comparison),
            FilterOperator.StartsWith         => cellStr.StartsWith(filterStr, comparison),
            FilterOperator.EndsWith           => cellStr.EndsWith(filterStr, comparison),
            FilterOperator.In                 => EvaluateInFilter(cellStr, filterStr, comparison),
            FilterOperator.GreaterThan        => CompareValues(rawValue, filter.Value) >  0,
            FilterOperator.LessThan           => CompareValues(rawValue, filter.Value) <  0,
            FilterOperator.GreaterThanOrEqual => CompareValues(rawValue, filter.Value) >= 0,
            FilterOperator.LessThanOrEqual    => CompareValues(rawValue, filter.Value) <= 0,
            _                                 => true
        };
    }

    private static bool EvaluateInFilter(
        string cellStr,
        string allowlistCsv,
        StringComparison comparison)
    {
        var allowed = allowlistCsv.Split(
            ',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        return allowed.Any(v => string.Equals(cellStr, v, comparison));
    }

    private static int CompareValues(object cell, object? filterValue)
    {
        if (filterValue is null) return 1;

        if (cell is IComparable comparable)
        {
            try
            {
                var converted = Convert.ChangeType(filterValue, cell.GetType());
                return comparable.CompareTo(converted);
            }
            catch (InvalidCastException) { }
            catch (FormatException)      { }
            catch (OverflowException)    { }
        }

        return string.Compare(cell.ToString(), filterValue.ToString(), StringComparison.Ordinal);
    }

    private static bool MatchesSearch(
        DataTableRow row,
        string term,
        IReadOnlyList<string>? fields)
    {
        IEnumerable<string> keys = fields is { Count: > 0 }
            ? (IEnumerable<string>)fields
            : row.Data.Keys;

        return keys.Any(key =>
            row.Data.TryGetValue(key, out var value) &&
            value?.ToString()?.Contains(term, StringComparison.OrdinalIgnoreCase) == true);
    }

    // ── Sort key comparison ───────────────────────────────────────────────────

    /// <summary>
    /// Type-aware comparer used as the LINQ key selector for multi-column grid sorting.
    /// Attempts a native <see cref="IComparable"/> comparison first; falls back to ordinal
    /// string comparison when the underlying types are incompatible.
    /// Treats <c>null</c> as the lowest possible value regardless of sort direction so that
    /// sparse rows always sort consistently to the leading or trailing edge.
    /// </summary>
    private sealed class NaturalSortComparer : IComparer<object?>
    {
        internal static readonly NaturalSortComparer Instance = new();

        /// <inheritdoc />
        public int Compare(object? x, object? y)
        {
            if (x is null && y is null) return  0;
            if (x is null)              return -1;
            if (y is null)              return  1;

            if (x is IComparable cx)
            {
                try   { return cx.CompareTo(y); }
                catch (ArgumentException) { /* incompatible types — fall through to string compare */ }
            }

            return string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal);
        }
    }
}
