namespace BlazorComponentLibrary.Tests;

using Xunit;
using BlazorComponentLibrary.Components.DragDropList;

/// <summary>
/// Tests for group functionality in DragDropList component.
/// Verifies that items can only be dropped into lists with the same Group name.
/// </summary>
public sealed class DragDropListGroupTests
{
    /// <summary>
    /// Tests that items can be reordered within the same group.
    /// </summary>
    [Fact]
    public void HandleDrop_WithinSameGroup_AllowsDrop()
    {
        // Arrange
        var list = new DragDropList<string> { Group = "test-group" };
        list.Items = new List<string> { "A", "B", "C" };

        // Simulate drag start at index 0
        list.HandleDragStart(0);

        // Simulate drag over index 2 with same group
        list.HandleDragOver(2, "test-group");

        // Act - should not throw and should allow the drop
        list.HandleDrop().Wait();

        // Assert - items should be reordered
        Assert.Equal(new[] { "B", "C", "A" }, list.Items);
    }

    /// <summary>
    /// Tests that items cannot be dropped into a different group.
    /// </summary>
    [Fact]
    public void HandleDrop_ToDifferentGroup_RejectsDrop()
    {
        // Arrange
        var list = new DragDropList<string> { Group = "group-a" };
        list.Items = new List<string> { "A", "B", "C" };

        // Simulate drag start at index 0
        list.HandleDragStart(0);

        // Simulate drag over index 2 with different group
        list.HandleDragOver(2, "group-b");

        // Act - drop should be rejected
        list.HandleDrop().Wait();

        // Assert - items should remain unchanged
        Assert.Equal(new[] { "A", "B", "C" }, list.Items);
    }

    /// <summary>
    /// Tests that items can be dropped when no group is specified (backward compatibility).
    /// </summary>
    [Fact]
    public void HandleDrop_NoGroupSpecified_AllowsDrop()
    {
        // Arrange
        var list = new DragDropList<string>(); // No group specified
        list.Items = new List<string> { "A", "B", "C" };

        // Simulate drag start at index 0
        list.HandleDragStart(0);

        // Simulate drag over index 2 with no group
        list.HandleDragOver(2, null);

        // Act - should allow the drop
        list.HandleDrop().Wait();

        // Assert - items should be reordered
        Assert.Equal(new[] { "B", "C", "A" }, list.Items);
    }

    /// <summary>
    /// Tests that items can be dropped when target has no group but source does (backward compatibility).
    /// </summary>
    [Fact]
    public void HandleDrop_SourceHasGroupTargetDoesNot_AllowsDrop()
    {
        // Arrange
        var list = new DragDropList<string> { Group = "test-group" };
        list.Items = new List<string> { "A", "B", "C" };

        // Simulate drag start at index 0
        list.HandleDragStart(0);

        // Simulate drag over index 2 with no group on target
        list.HandleDragOver(2, null);

        // Act - should allow the drop (backward compatibility)
        list.HandleDrop().Wait();

        // Assert - items should be reordered
        Assert.Equal(new[] { "B", "C", "A" }, list.Items);
    }

    /// <summary>
    /// Tests that items can be dropped when both have no group (backward compatibility).
    /// </summary>
    [Fact]
    public void HandleDrop_BothNoGroup_AllowsDrop()
    {
        // Arrange
        var list = new DragDropList<string>(); // No group on source
        list.Items = new List<string> { "A", "B", "C" };

        // Simulate drag start at index 0
        list.HandleDragStart(0);

        // Simulate drag over index 2 with no group on target
        list.HandleDragOver(2, null);

        // Act - should allow the drop
        list.HandleDrop().Wait();

        // Assert - items should be reordered
        Assert.Equal(new[] { "B", "C", "A" }, list.Items);
    }

    /// <summary>
    /// Tests that Group property is properly set on the component.
    /// </summary>
    [Fact]
    public void Group_Property_SetAndGet()
    {
        // Arrange
        var list = new DragDropList<string>();

        // Act
        list.Group = "test-group";

        // Assert
        Assert.Equal("test-group", list.Group);
    }

    /// <summary>
    /// Tests that Group property can be null.
    /// </summary>
    [Fact]
    public void Group_Property_CanBeNull()
    {
        // Arrange
        var list = new DragDropList<string>();

        // Act
        list.Group = null;

        // Assert
        Assert.Null(list.Group);
    }
}