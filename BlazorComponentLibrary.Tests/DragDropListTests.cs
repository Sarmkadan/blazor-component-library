namespace BlazorComponentLibrary.Tests;

using Xunit;
using BlazorComponentLibrary.Components.DragDropList;

/// <summary>
/// Unit tests for the <see cref="DragDropList{T}"/> static class.
/// Tests verify the reordering functionality works correctly for various scenarios including
/// moving items between different indices, boundary conditions, and error cases.
/// </summary>
public sealed class DragDropListTests
{
    /// <summary>
    /// Tests that an item can be moved from a lower index to a higher index in the list.
    /// Verifies that the item is correctly removed from the source position and inserted
    /// at the target position while maintaining the order of other items.
    /// </summary>
    [Fact]
    public void Reorder_MovesItemFromLowerToHigherIndex()
    {
        var items = new List<string> { "A", "B", "C", "D" };
        var result = DragDropList<string>.Reorder(items, fromIndex: 0, toIndex: 2);
        Assert.Equal(new[] { "B", "C", "A", "D" }, result);
    }

    /// <summary>
    /// Tests that an item can be moved from a higher index to a lower index in the list.
    /// Verifies that the item is correctly removed from the source position and inserted
    /// at the target position while maintaining the order of other items.
    /// </summary>
    [Fact]
    public void Reorder_MovesItemFromHigherToLowerIndex()
    {
        var items = new List<string> { "A", "B", "C", "D" };
        var result = DragDropList<string>.Reorder(items, fromIndex: 3, toIndex: 1);
        Assert.Equal(new[] { "A", "D", "B", "C" }, result);
    }

    /// <summary>
    /// Tests that the source list is not mutated during the reordering operation.
    /// Verifies that the original list remains unchanged after calling Reorder, ensuring
    /// that the method creates a new list rather than modifying the input list in-place.
    /// </summary>
    [Fact]
    public void Reorder_DoesNotMutateSourceList()
    {
        var items = new List<string> { "A", "B", "C" };
        DragDropList<string>.Reorder(items, fromIndex: 0, toIndex: 2);
        Assert.Equal(new[] { "A", "B", "C" }, items);
    }

    /// <summary>
    /// Tests that when fromIndex and toIndex are the same, the list remains unchanged.
    /// Verifies that the Reorder method returns a new list with the same order when
    /// the source and target indices are identical, effectively being a no-op operation.
    /// </summary>
    [Fact]
    public void Reorder_SameIndex_ReturnsSameOrder()
    {
        var items = new List<int> { 1, 2, 3 };
        var result = DragDropList<int>.Reorder(items, fromIndex: 1, toIndex: 1);
        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    /// <summary>
    /// Tests that a negative fromIndex value throws an ArgumentOutOfRangeException.
    /// Verifies that the Reorder method validates input parameters and throws appropriate
    /// exceptions when invalid values are provided.
    /// </summary>
    [Fact]
    public void Reorder_NegativeFromIndex_ThrowsArgumentOutOfRange()
    {
        var items = new List<string> { "A", "B" };
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DragDropList<string>.Reorder(items, fromIndex: -1, toIndex: 0));
    }

    /// <summary>
    /// Tests that an out-of-bounds fromIndex value throws an ArgumentOutOfRangeException.
    /// Verifies that the Reorder method validates that fromIndex is within the bounds of the list
    /// and throws appropriate exceptions when invalid values are provided.
    /// </summary>
    [Fact]
    public void Reorder_FromIndexOutOfBounds_ThrowsArgumentOutOfRange()
    {
        var items = new List<string> { "A", "B" };
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DragDropList<string>.Reorder(items, fromIndex: 5, toIndex: 0));
    }

    /// <summary>
    /// Tests that an out-of-bounds toIndex value throws an ArgumentOutOfRangeException.
    /// Verifies that the Reorder method validates that toIndex is within the bounds of the list
    /// and throws appropriate exceptions when invalid values are provided.
    /// </summary>
    [Fact]
    public void Reorder_ToIndexOutOfBounds_ThrowsArgumentOutOfRange()
    {
        var items = new List<string> { "A", "B" };
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DragDropList<string>.Reorder(items, fromIndex: 0, toIndex: 99));
    }

    /// <summary>
    /// Tests that a single-item list returns the same item when reordered.
    /// Verifies that the Reorder method correctly handles edge cases with minimal input
    /// and returns a list containing the single item.
    /// </summary>
    [Fact]
    public void Reorder_SingleItemList_ReturnsSingleItem()
    {
        var items = new List<int> { 42 };
        var result = DragDropList<int>.Reorder(items, fromIndex: 0, toIndex: 0);
        Assert.Single(result);
        Assert.Equal(42, result[0]);
    }
}
