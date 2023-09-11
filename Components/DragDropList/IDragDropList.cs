namespace BlazorComponentLibrary.Components.DragDropList;

using Microsoft.AspNetCore.Components;

/// <summary>
/// Contract for the drag-and-drop reorderable list component.
/// </summary>
/// <typeparam name="TItem">The type of items in the list.</typeparam>
public interface IDragDropList<TItem>
{
    /// <summary>Gets or sets the ordered collection of items to display.</summary>
    IList<TItem> Items { get; set; }

    /// <summary>Gets or sets the render template for a single list item.</summary>
    RenderFragment<TItem> ItemTemplate { get; set; }

    /// <summary>
    /// Event raised after the user drops an item at a new position.
    /// The argument is the fully reordered list so the parent component can
    /// persist the new order without computing a diff.
    /// </summary>
    EventCallback<IList<TItem>> OnOrderChanged { get; set; }

    /// <summary>
    /// Gets or sets whether drag-and-drop reordering is currently enabled.
    /// When set to <c>false</c> the drag handle is still rendered but dragging
    /// has no effect, giving a disabled-but-visible appearance.
    /// </summary>
    bool Enabled { get; set; }
}
