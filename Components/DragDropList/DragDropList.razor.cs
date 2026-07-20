namespace BlazorComponentLibrary.Components.DragDropList;

using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A reorderable list component that uses the HTML5 Drag and Drop API.
/// Users grab any row by its handle and drop it at the desired position.
/// The <see cref="OnOrderChanged"/> callback fires with the fully reordered list so the
/// parent component can persist the new order without computing a diff.
/// </summary>
/// <typeparam name="TItem">The type of each list item.</typeparam>
public sealed partial class DragDropList<TItem> : ComponentBase, IDragDropList<TItem>
{
    internal int _draggingIndex = -1;
    internal int _dragOverIndex = -1;
    internal string? _dragOverListGroup;

    /// <summary>
    /// Gets or sets the group name that this list belongs to.
    /// Items can only be dropped into lists sharing the same Group name.
    /// </summary>
    [Parameter]
    public string? Group { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public IList<TItem> Items { get; set; } = new List<TItem>();

    /// <inheritdoc/>
    [Parameter]
    public RenderFragment<TItem> ItemTemplate { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter]
    public EventCallback<IList<TItem>> OnOrderChanged { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public bool Enabled { get; set; } = true;

    /// <summary>Optional CSS class(es) appended to the root <c>&lt;ul&gt;</c> element.</summary>
    [Parameter]
    public string? CssClass { get; set; }

    private string RootClass =>
        string.IsNullOrEmpty(CssClass) ? "bcl-dnd-list" : $"bcl-dnd-list {CssClass}";

    internal IEnumerable<(TItem item, int index)> IndexedItems =>
        Items.Select((item, i) => (item, i));

    internal void HandleDragStart(int index)
    {
        if (!Enabled) return;
        _draggingIndex = index;
    }

    internal void HandleDragOver(int index, string? dragOverListGroup)
    {
        if (!Enabled || _draggingIndex < 0) return;
        _dragOverIndex = index;
        _dragOverListGroup = dragOverListGroup;
    }

    internal async Task HandleDrop()
    {
        if (!Enabled || _draggingIndex < 0 || _dragOverIndex < 0 ||
            _draggingIndex == _dragOverIndex)
        {
            ResetDragState();
            return;
        }

        // Check if the drop is allowed (same group or no group specified)
        if (Group != null && _dragOverListGroup != null && !string.Equals(Group, _dragOverListGroup, StringComparison.Ordinal))
        {
            ResetDragState();
            return;
        }

        var reordered = Reorder(Items, _draggingIndex, _dragOverIndex);
        ResetDragState();

        Items = reordered;
        StateHasChanged();

        if (OnOrderChanged.HasDelegate)
            await OnOrderChanged.InvokeAsync(reordered);
    }

    internal void HandleDragEnd() => ResetDragState();

    private void ResetDragState()
    {
        _draggingIndex = -1;
        _dragOverIndex = -1;
        _dragOverListGroup = null;
    }

    /// <summary>
    /// Returns a new list with the item originally at <paramref name="fromIndex"/>
    /// moved to <paramref name="toIndex"/>. The source list is not mutated.
    /// </summary>
    /// <param name="source">The source list.</param>
    /// <param name="fromIndex">Zero-based index of the item to move.</param>
    /// <param name="toIndex">Zero-based target index after the move.</param>
    /// <returns>A new <see cref="List{T}"/> reflecting the reordered items.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="fromIndex"/> or <paramref name="toIndex"/> is outside the bounds of <paramref name="source"/>.
    /// </exception>
    public static List<TItem> Reorder(IList<TItem> source, int fromIndex, int toIndex)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (fromIndex < 0 || fromIndex >= source.Count)
            throw new ArgumentOutOfRangeException(nameof(fromIndex));
        if (toIndex < 0 || toIndex >= source.Count)
            throw new ArgumentOutOfRangeException(nameof(toIndex));

        var list = new List<TItem>(source);
        var item = list[fromIndex];
        list.RemoveAt(fromIndex);
        list.Insert(toIndex, item);
        return list;
    }
}
