namespace BlazorComponentLibrary.Tests;

using Xunit;
using BlazorComponentLibrary.Components.DragDropList;

public sealed class DragDropListTests
{
    [Fact]
    public void Reorder_MovesItemFromLowerToHigherIndex()
    {
        var items = new List<string> { "A", "B", "C", "D" };
        var result = DragDropList<string>.Reorder(items, fromIndex: 0, toIndex: 2);
        Assert.Equal(new[] { "B", "C", "A", "D" }, result);
    }

    [Fact]
    public void Reorder_MovesItemFromHigherToLowerIndex()
    {
        var items = new List<string> { "A", "B", "C", "D" };
        var result = DragDropList<string>.Reorder(items, fromIndex: 3, toIndex: 1);
        Assert.Equal(new[] { "A", "D", "B", "C" }, result);
    }

    [Fact]
    public void Reorder_DoesNotMutateSourceList()
    {
        var items = new List<string> { "A", "B", "C" };
        DragDropList<string>.Reorder(items, fromIndex: 0, toIndex: 2);
        Assert.Equal(new[] { "A", "B", "C" }, items);
    }

    [Fact]
    public void Reorder_SameIndex_ReturnsSameOrder()
    {
        var items = new List<int> { 1, 2, 3 };
        var result = DragDropList<int>.Reorder(items, fromIndex: 1, toIndex: 1);
        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    [Fact]
    public void Reorder_NegativeFromIndex_ThrowsArgumentOutOfRange()
    {
        var items = new List<string> { "A", "B" };
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DragDropList<string>.Reorder(items, fromIndex: -1, toIndex: 0));
    }

    [Fact]
    public void Reorder_FromIndexOutOfBounds_ThrowsArgumentOutOfRange()
    {
        var items = new List<string> { "A", "B" };
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DragDropList<string>.Reorder(items, fromIndex: 5, toIndex: 0));
    }

    [Fact]
    public void Reorder_ToIndexOutOfBounds_ThrowsArgumentOutOfRange()
    {
        var items = new List<string> { "A", "B" };
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DragDropList<string>.Reorder(items, fromIndex: 0, toIndex: 99));
    }

    [Fact]
    public void Reorder_SingleItemList_ReturnsSingleItem()
    {
        var items = new List<int> { 42 };
        var result = DragDropList<int>.Reorder(items, fromIndex: 0, toIndex: 0);
        Assert.Single(result);
        Assert.Equal(42, result[0]);
    }
}
