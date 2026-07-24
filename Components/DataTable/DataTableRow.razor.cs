namespace BlazorComponentLibrary.Components.DataTable;

using Microsoft.AspNetCore.Components;

/// <summary>
/// Renders a single row of a <see cref="DataTable{TItem}"/>. Receives the row model and its
/// selection flag as independent parameters (rather than the whole table state) and overrides
/// <see cref="ShouldRender"/> so that re-rendering only happens when the item reference/value or
/// the selection flag actually changes. Combined with <c>@key</c> on the row's identity in the
/// parent's render loop, this keeps a single row-selection toggle from forcing every row in the
/// table to be diffed and re-rendered.
/// </summary>
/// <typeparam name="TItem">The type of the row model.</typeparam>
public sealed partial class DataTableRow<TItem> : ComponentBase
{
    private TItem? _lastRenderedItem;
    private bool _lastRenderedIsSelected;
    private bool _hasRenderedOnce;

    /// <summary>
    /// Gets or sets the row's data model.
    /// </summary>
    [Parameter]
    public TItem Item { get; set; } = default!;

    /// <summary>
    /// Gets or sets a value indicating whether this row is currently selected.
    /// </summary>
    [Parameter]
    public bool IsSelected { get; set; }

    /// <summary>
    /// Gets or sets the render fragment used to render the row's cells for <see cref="Item"/>.
    /// </summary>
    [Parameter]
    public RenderFragment<TItem> RowTemplate { get; set; } = null!;

    /// <summary>
    /// Gets or sets the callback invoked when the row is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<TItem> OnRowClick { get; set; }

    /// <summary>
    /// Determines whether the row needs to be re-rendered by comparing the current
    /// <see cref="Item"/> and <see cref="IsSelected"/> values against the values used for the
    /// last successful render. Unrelated rows in the parent's foreach loop keep their previous
    /// render output untouched.
    /// </summary>
    /// <returns><see langword="true"/> if the row's state changed since the last render; otherwise <see langword="false"/>.</returns>
    protected override bool ShouldRender()
    {
        if (_hasRenderedOnce &&
            EqualityComparer<TItem>.Default.Equals(_lastRenderedItem, Item) &&
            _lastRenderedIsSelected == IsSelected)
        {
            return false;
        }

        _lastRenderedItem = Item;
        _lastRenderedIsSelected = IsSelected;
        _hasRenderedOnce = true;
        return true;
    }

    /// <summary>
    /// Invokes <see cref="OnRowClick"/> with the row's <see cref="Item"/> when the row is clicked.
    /// </summary>
    /// <returns>A task representing the asynchronous invocation.</returns>
    private Task HandleClickAsync() => OnRowClick.HasDelegate ? OnRowClick.InvokeAsync(Item) : Task.CompletedTask;
}
