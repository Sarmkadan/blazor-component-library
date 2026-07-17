namespace BlazorComponentLibrary.Components.DragDropList;

/// <summary>
/// Provides useful extension methods for <see cref="DragDropList{TItem}"/> components.
/// </summary>
public static class DragDropListExtensions
{
    /// <summary>
    /// Moves an item from one index to another in the list, using the <see cref="DragDropList{TItem}.Reorder"/> method.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the list.</typeparam>
    /// <param name="list">The <see cref="DragDropList{TItem}"/> instance.</param>
    /// <param name="item">The item to move.</param>
    /// <param name="fromIndex">The current index of the item.</param>
    /// <param name="toIndex">The target index where the item should be moved.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> is null.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="fromIndex"/> or <paramref name="toIndex"/> is outside the bounds of the list.
    /// </exception>
    public static void MoveItem<TItem>(this DragDropList<TItem> list, TItem item, int fromIndex, int toIndex)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(item);

        if (fromIndex < 0 || fromIndex >= list.Items.Count)
            throw new ArgumentOutOfRangeException(nameof(fromIndex));

        if (toIndex < 0 || toIndex >= list.Items.Count)
            throw new ArgumentOutOfRangeException(nameof(toIndex));

        if (fromIndex == toIndex)
            return;

        var reordered = DragDropList<TItem>.Reorder(list.Items, fromIndex, toIndex);
        list.Items = reordered;

        if (list.OnOrderChanged.HasDelegate)
            _ = list.OnOrderChanged.InvokeAsync(reordered);
    }

    /// <summary>
    /// Moves an item to the beginning of the list.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the list.</typeparam>
    /// <param name="list">The <see cref="DragDropList{TItem}"/> instance.</param>
    /// <param name="item">The item to move to the beginning.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> is null.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
    public static void MoveToBeginning<TItem>(this DragDropList<TItem> list, TItem item)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(item);

        var index = list.Items.IndexOf(item);
        if (index < 0)
            return;

        list.MoveItem(item, index, 0);
    }

    /// <summary>
    /// Moves an item to the end of the list.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the list.</typeparam>
    /// <param name="list">The <see cref="DragDropList{TItem}"/> instance.</param>
    /// <param name="item">The item to move to the end.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> is null.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
    public static void MoveToEnd<TItem>(this DragDropList<TItem> list, TItem item)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(item);

        var index = list.Items.IndexOf(item);
        if (index < 0)
            return;

        list.MoveItem(item, index, list.Items.Count - 1);
    }

    /// <summary>
    /// Swaps two items in the list by their indices.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the list.</typeparam>
    /// <param name="list">The <see cref="DragDropList{TItem}"/> instance.</param>
    /// <param name="index1">The first index.</param>
    /// <param name="index2">The second index.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="index1"/> or <paramref name="index2"/> is outside the bounds of the list.
    /// </exception>
    public static void SwapItems<TItem>(this DragDropList<TItem> list, int index1, int index2)
    {
        ArgumentNullException.ThrowIfNull(list);

        if (index1 < 0 || index1 >= list.Items.Count)
            throw new ArgumentOutOfRangeException(nameof(index1));

        if (index2 < 0 || index2 >= list.Items.Count)
            throw new ArgumentOutOfRangeException(nameof(index2));

        if (index1 == index2)
            return;

        var reordered = DragDropList<TItem>.Reorder(list.Items, index1, index2);
        list.Items = reordered;

        if (list.OnOrderChanged.HasDelegate)
            _ = list.OnOrderChanged.InvokeAsync(reordered);
    }

    /// <summary>
    /// Gets the current index of a specific item in the list.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the list.</typeparam>
    /// <param name="list">The <see cref="DragDropList{TItem}"/> instance.</param>
    /// <param name="item">The item to find.</param>
    /// <returns>The zero-based index of the item, or -1 if not found.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> is null.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
    public static int IndexOf<TItem>(this DragDropList<TItem> list, TItem item)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(item);

        return list.Items.IndexOf(item);
    }

    /// <summary>
    /// Determines whether the list contains a specific item.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the list.</typeparam>
    /// <param name="list">The <see cref="DragDropList{TItem}"/> instance.</param>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the item is in the list; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> is null.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
    public static bool Contains<TItem>(this DragDropList<TItem> list, TItem item)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(item);

        return list.Items.Contains(item);
    }

    /// <summary>
    /// Gets the number of items in the list.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the list.</typeparam>
    /// <param name="list">The <see cref="DragDropList{TItem}"/> instance.</param>
    /// <returns>The number of items in the list.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> is null.</exception>
    public static int Count<TItem>(this DragDropList<TItem> list)
    {
        ArgumentNullException.ThrowIfNull(list);
        return list.Items.Count;
    }

    /// <summary>
    /// Gets a read-only view of the items in the list.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the list.</typeparam>
    /// <param name="list">The <see cref="DragDropList{TItem}"/> instance.</param>
    /// <returns>An <see cref="IReadOnlyList{T}"/> of items.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> is null.</exception>
    public static IReadOnlyList<TItem> AsReadOnly<TItem>(this DragDropList<TItem> list)
    {
        ArgumentNullException.ThrowIfNull(list);
        return list.Items.AsReadOnly();
    }
}