namespace BlazorComponentLibrary.Components.DataTable;

using BlazorComponentLibrary.Exceptions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Linq;

public enum SortDirection
{
    Ascending,
    Descending
}

/// <summary>
/// Compares two objects in a null-safe manner, placing nulls last.
/// </summary>
public sealed class NullSafeComparer : IComparer<object?>
{
    public static readonly NullSafeComparer Instance = new();

    public int Compare(object? x, object? y)
    {
        if (x is null && y is null) return 0;
        if (x is null) return 1;
        if (y is null) return -1;
        if (x is IComparable cx) return cx.CompareTo(y);
        return string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal);
    }
}

public sealed partial class DataTable<TItem> : ComponentBase, IDataTable<TItem>
{
    private IEnumerable<TItem> _data = Enumerable.Empty<TItem>();
    private IEnumerable<TItem> _currentViewData = Enumerable.Empty<TItem>();
    private List<(Func<TItem, object?> KeySelector, SortDirection Direction)> _sortKeys = new();
    private bool _isShiftKeyPressed = false;

    [Parameter]
    public RenderFragment TableHeader { get; set; }

    [Parameter]
    public RenderFragment<TItem> RowTemplate { get; set; }

    [Parameter]
    public EventCallback<TItem> OnRowClick { get; set; }

    [Parameter]
    public bool IsSortable { get; set; } = false;

    [Parameter]
    public bool IsFilterable { get; set; } = false;

    [Parameter]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// When true, uses Blazor's Virtualize component to render only visible rows,
    /// enabling efficient display of large datasets (thousands of rows) without
    /// layout thrashing. Pagination is disabled when virtualization is active.
    /// </summary>
    [Parameter]
    public bool EnableVirtualization { get; set; } = false;

    /// <summary>
    /// Sets the data source for the data table.
    /// </summary>
    /// <param name="data">The enumerable collection of data items.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
    public void SetData(IEnumerable<TItem> data)
    {
        ArgumentNullException.ThrowIfNull(data);
        _data = data;
        ApplyView();
        NotifyStateChanged();
    }

    /// <summary>
    /// Refreshes the data table, re-rendering its content.
    /// </summary>
    public void Refresh()
    {
        ApplyView();
        NotifyStateChanged();
    }

    /// <summary>
    /// Sorts the data table by the specified key selector. Null values are placed last,
    /// preventing NullReferenceException when columns contain null entries.
    /// </summary>
    /// <param name="keySelector">A function that extracts the sort key from a row item.</param>
    /// <param name="direction">The sort direction.</param>
    public void SortBy(Func<TItem, object?> keySelector, SortDirection direction = SortDirection.Ascending)
    {
        _sortKeys.Clear();
        _sortKeys.Add((keySelector, direction));
        ApplyView();
        NotifyStateChanged();
    }

    /// <summary>
    /// Adds a secondary sort key to the existing sort keys.
    /// </summary>
    /// <param name="keySelector">A function that extracts the sort key from a row item.</param>
    /// <param name="direction">The sort direction.</param>
    public void AddSortKey(Func<TItem, object?> keySelector, SortDirection direction = SortDirection.Ascending)
    {
        _sortKeys.Add((keySelector, direction));
        ApplyView();
        NotifyStateChanged();
    }

    /// <summary>
    /// Clears all sort keys and returns to the original data order.
    /// </summary>
    public void ClearSort()
    {
        _sortKeys.Clear();
        ApplyView();
        NotifyStateChanged();
    }

    /// <summary>
    /// Notifies the component that its state has changed.
    /// </summary>
    private void NotifyStateChanged()
    {
        try
        {
            StateHasChanged();
        }
        catch (InvalidOperationException)
        {
            // Ignore if component not yet initialized (e.g. during benchmark setup)
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        ApplyView();
    }

    private void ApplyView()
    {
        IEnumerable<TItem> result = _data;

        if (_sortKeys.Count > 0)
        {
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

            result = ordered ?? result;
        }

        // When virtualization is enabled, expose all rows — the Virtualize component
        // handles windowing. Pagination only applies in non-virtualized mode.
        _currentViewData = EnableVirtualization ? result : result.Take(PageSize);
    }

    protected async Task OnRowClickHandler(TItem item)
    {
        if (OnRowClick.HasDelegate)
        {
            await OnRowClick.InvokeAsync(item);
        }
    }

    protected void OnKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Shift")
        {
            _isShiftKeyPressed = true;
        }
    }

    protected void OnKeyUp(KeyboardEventArgs args)
    {
        if (args.Key == "Shift")
        {
            _isShiftKeyPressed = false;
        }
    }
}
