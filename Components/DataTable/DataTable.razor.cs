namespace BlazorComponentLibrary.Components.DataTable;

using BlazorComponentLibrary.Exceptions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

public sealed partial class DataTable<TItem> : ComponentBase, IDataTable<TItem>, IDisposable, IAsyncDisposable
{
    private IEnumerable<TItem> _data = Enumerable.Empty<TItem>();
    private IEnumerable<TItem> _currentViewData = Enumerable.Empty<TItem>();
    private readonly SortState<TItem> _sortState = new();
    private bool _isShiftKeyPressed = false;
    private ISet<string> _hiddenColumns = new HashSet<string>();
    private int _dataVersion = 0;
    private int _sortVersion = 0;
    private int _pageVersion = 0;
    private bool _disposed;

    // Cache for compiled property accessors to avoid reflection per cell render
    private static readonly ConcurrentDictionary<(Type ItemType, string PropertyName), Func<object, object?>> _propertyAccessorCache = new();
    private static readonly ConcurrentDictionary<(Type ItemType, string PropertyName), Action<object, object?>> _propertySetterCache = new();

    /// <summary>
    /// Gets the value of a property from an item using a cached compiled delegate.
    /// </summary>
    /// <param name="item">The item to get the property from.</param>
    /// <param name="propertyName">The name of the property to access.</param>
    /// <returns>The property value, or null if the property doesn't exist.</returns>
    private static object? GetPropertyValue(TItem item, string propertyName)
    {
        if (item == null)
        {
            return null;
        }

        var cacheKey = (typeof(TItem), propertyName);
        if (!_propertyAccessorCache.TryGetValue(cacheKey, out var accessor))
        {
            accessor = CreatePropertyAccessor(typeof(TItem), propertyName);
            _propertyAccessorCache.TryAdd(cacheKey, accessor);
        }

        return accessor(item);
    }

    /// <summary>
    /// Creates a compiled delegate to access a property value.
    /// </summary>
    private static Func<object, object?> CreatePropertyAccessor(Type itemType, string propertyName)
    {
        var propertyInfo = itemType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        if (propertyInfo == null)
        {
            return _ => null;
        }

        var param = Expression.Parameter(typeof(object), "item");
        var castParam = Expression.Convert(param, itemType);
        var access = Expression.Property(castParam, propertyInfo);
        var convert = Expression.Convert(access, typeof(object));
        var lambda = Expression.Lambda<Func<object, object?>>(convert, param);

        return lambda.Compile();
    }

    /// <summary>
    /// Sets the value of a property on an item using a cached compiled delegate.
    /// </summary>
    /// <param name="item">The item to set the property on.</param>
    /// <param name="propertyName">The name of the property to set.</param>
    /// <param name="value">The value to set.</param>
    private static void SetPropertyValue(TItem item, string propertyName, object? value)
    {
        if (item == null)
        {
            return;
        }

        var cacheKey = (typeof(TItem), propertyName);
        if (!_propertySetterCache.TryGetValue(cacheKey, out var setter))
        {
            setter = CreatePropertySetter(typeof(TItem), propertyName);
            _propertySetterCache.TryAdd(cacheKey, setter);
        }

        setter(item, value);
    }

    /// <summary>
    /// Creates a compiled delegate to set a property value.
    /// </summary>
    private static Action<object, object?> CreatePropertySetter(Type itemType, string propertyName)
    {
        var propertyInfo = itemType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        if (propertyInfo == null || !propertyInfo.CanWrite)
        {
            return (_, _) => { };
        }

        var paramItem = Expression.Parameter(typeof(object), "item");
        var paramValue = Expression.Parameter(typeof(object), "value");
        var castItem = Expression.Convert(paramItem, itemType);
        var castValue = Expression.Convert(paramValue, propertyInfo.PropertyType);
        var assign = Expression.Assign(Expression.Property(castItem, propertyInfo), castValue);
        var lambda = Expression.Lambda<Action<object, object?>>(assign, paramItem, paramValue);

        return lambda.Compile();
    }

    [Parameter]
    public RenderFragment TableHeader { get; set; } = null!;

    [Parameter]
    public RenderFragment<TItem> RowTemplate { get; set; } = null!;

    [Parameter]
    public EventCallback<TItem> OnRowClick { get; set; }

    [Parameter]
    public RenderFragment? EmptyTemplate { get; set; } = EmptyContent;

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
    /// Gets or sets the names of columns that should be hidden.
    /// </summary>
    [Parameter]
    public ISet<string> HiddenColumns
    {
        get => _hiddenColumns;
        set => _hiddenColumns = value ?? new HashSet<string>();
    }

    /// <summary>
    /// Sets the data source for the data table.
    /// </summary>
    /// <param name="data">The enumerable collection of data items.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
    public void SetData(IEnumerable<TItem> data)
    {
        ArgumentNullException.ThrowIfNull(data);
        _data = data;
        _dataVersion++;
        _sortState.SetData(data);
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
        _sortState.SortBy(keySelector, direction);
        _sortVersion++;
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
        _sortState.AddSortKey(keySelector, direction);
        _sortVersion++;
        ApplyView();
        NotifyStateChanged();
    }

    /// <summary>
    /// Clears all sort keys and returns to the original data order.
    /// </summary>
    public void ClearSort()
    {
        _sortState.ClearSort();
        _sortVersion++;
        ApplyView();
        NotifyStateChanged();
    }

    /// <summary>
    /// Toggles the visibility of a column by name.
    /// If the column is currently hidden, it will be shown. If it is currently shown, it will be hidden.
    /// </summary>
    /// <param name="columnName">The name of the column to toggle.</param>
    public void ToggleColumn(string columnName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);

        if (_hiddenColumns.Contains(columnName))
        {
            _hiddenColumns.Remove(columnName);
        }
        else
        {
            _hiddenColumns.Add(columnName);
        }

        _pageVersion++;
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
        _pageVersion++;
        ApplyView();
    }

    protected override bool ShouldRender()
    {
        // Track the version stamps at the time of last successful render
        if (_lastRenderDataVersion == _dataVersion &&
            _lastRenderSortVersion == _sortVersion &&
            _lastRenderPageVersion == _pageVersion)
        {
            return false;
        }

        // Update the last rendered version stamps
        _lastRenderDataVersion = _dataVersion;
        _lastRenderSortVersion = _sortVersion;
        _lastRenderPageVersion = _pageVersion;

        return true;
    }

    private int _lastRenderDataVersion = -1;
    private int _lastRenderSortVersion = -1;
    private int _lastRenderPageVersion = -1;
    private IDisposable? _virtualizeRegistration;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _virtualizeRegistration?.Dispose();
        _disposed = true;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        if (_virtualizeRegistration != null)
        {
            _virtualizeRegistration.Dispose();
            _virtualizeRegistration = null;
        }

        _disposed = true;
        await ValueTask.CompletedTask;
    }

    private void ApplyView()
    {
        // Apply sorting if any sort keys are set
        IEnumerable<TItem> sortedData = _sortState.ApplySort();

        // When virtualization is enabled, expose all rows — the Virtualize component
        // handles windowing. Pagination only applies in non-virtualized mode.
        _currentViewData = EnableVirtualization ? sortedData : sortedData.Take(PageSize);
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

    /// <summary>
    /// Default content to render when the data table has no items to display.
    /// Shows a "No data" message spanning all columns.
    /// </summary>
    private static RenderFragment EmptyContent => builder =>
    {
        builder.OpenElement(0, "tr");
        builder.AddAttribute(1, "colspan", "100%");
        builder.AddContent(2, "No data");
        builder.CloseElement();
    };
}