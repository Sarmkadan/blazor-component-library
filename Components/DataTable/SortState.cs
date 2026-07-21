namespace BlazorComponentLibrary.Components.DataTable;

/// <summary>
/// Manages sorting state and logic for the DataTable component.
/// This internal class encapsulates all sorting-related functionality to enable better testability.
/// </summary>
/// <typeparam name="TItem">The type of items in the data table.</typeparam>
internal sealed class SortState<TItem>
{
    private readonly List<(Func<TItem, object?> KeySelector, SortDirection Direction)> _sortKeys = new();
    private IEnumerable<TItem> _data = Enumerable.Empty<TItem>();

    /// <summary>
    /// Gets the current sort keys.
    /// </summary>
    public IReadOnlyList<(Func<TItem, object?> KeySelector, SortDirection Direction)> SortKeys => _sortKeys.AsReadOnly();

    /// <summary>
    /// Sets the data source for sorting.
    /// </summary>
    /// <param name="data">The data to sort.</param>
    public void SetData(IEnumerable<TItem> data)
    {
        _data = data ?? Enumerable.Empty<TItem>();
    }

    /// <summary>
    /// Sorts the data by the specified key selector.
    /// </summary>
    /// <param name="keySelector">A function that extracts the sort key from a row item.</param>
    /// <param name="direction">The sort direction.</param>
    public void SortBy(Func<TItem, object?> keySelector, SortDirection direction = SortDirection.Ascending)
    {
        _sortKeys.Clear();
        _sortKeys.Add((keySelector, direction));
    }

    /// <summary>
    /// Adds a secondary sort key to the existing sort keys.
    /// </summary>
    /// <param name="keySelector">A function that extracts the sort key from a row item.</param>
    /// <param name="direction">The sort direction.</param>
    public void AddSortKey(Func<TItem, object?> keySelector, SortDirection direction = SortDirection.Ascending)
    {
        _sortKeys.Add((keySelector, direction));
    }

    /// <summary>
    /// Clears all sort keys and returns to the original data order.
    /// </summary>
    public void ClearSort()
    {
        _sortKeys.Clear();
    }

    /// <summary>
    /// Applies the current sorting to the data and returns the sorted result.
    /// </summary>
    /// <returns>The sorted data.</returns>
    public IEnumerable<TItem> ApplySort()
    {
        if (_sortKeys.Count == 0)
        {
            return _data;
        }

        IEnumerable<TItem> result = _data;
        IOrderedEnumerable<TItem>? ordered = null;

        foreach (var (keySelector, direction) in _sortKeys)
        {
            if (ordered == null)
            {
                ordered = direction == SortDirection.Ascending
                    ? result.OrderBy(keySelector, NullSafeComparer.Instance)
                    : result.OrderByDescending(keySelector, NullSafeComparer.Instance);
            }
            else
            {
                ordered = direction == SortDirection.Ascending
                    ? ordered.ThenBy(keySelector, NullSafeComparer.Instance)
                    : ordered.ThenByDescending(keySelector, NullSafeComparer.Instance);
            }
        }

        return ordered ?? result;
    }
}