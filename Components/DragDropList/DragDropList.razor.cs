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
public partial class DragDropList<TItem> : ComponentBase, IDragDropList<TItem>
{
    internal int _draggingIndex = -1;
    internal int _dragOverIndex = -1;

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

    internal void HandleDragOver(int index)
    {
        if (!Enabled || _draggingIndex < 0) return;
        _dragOverIndex = index;
    }

    internal async Task HandleDrop()
    {
        if (!Enabled || _draggingIndex < 0 || _dragOverIndex < 0 ||
            _draggingIndex == _dragOverIndex)
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
    }

    /// <summary>
    /// Returns a new list with the item originally at <paramref name="fromIndex"/>
    /// moved to <paramref name="toIndex"/>. The source list is not mutated.
    /// </summary>
    /// <param name="source">The source list.</param>
    /// <param name="fromIndex">Zero-based index of the item to move.</param>
    /// <param name="toIndex">Zero-based target index after the move.</param>
    /// <returns>A new <see cref="List{T}"/> reflecting the reordered items.</returns>
    public static List<TItem> Reorder(IList<TItem> source, int fromIndex, int toIndex)
    {
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
