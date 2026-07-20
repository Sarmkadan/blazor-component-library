using BlazorComponentLibrary.Components.DataTable;
using FluentAssertions;
using Xunit;

/// <summary>
/// Tests for column visibility toggling in the DataTable component.
/// </summary>
public class DataTableColumnVisibilityTests
{
    /// <summary>
    /// Verifies that the HiddenColumns property can be set and retrieved.
    /// </summary>
    [Fact]
    public void HiddenColumns_ShouldBeSetAndRetrieved()
    {
        // Arrange
        var table = new DataTable<int>();
        var hiddenColumns = new HashSet<string> { "Column1", "Column2" };

        // Act
        table.HiddenColumns = hiddenColumns;

        // Assert
        table.HiddenColumns.Should().BeEquivalentTo(hiddenColumns);
    }

    /// <summary>
    /// Verifies that setting HiddenColumns to null creates a new empty set.
    /// </summary>
    [Fact]
    public void HiddenColumns_SetToNull_ShouldCreateEmptySet()
    {
        // Arrange
        var table = new DataTable<int>();
        table.HiddenColumns = new HashSet<string> { "Column1" };

        // Act
        table.HiddenColumns = null!;

        // Assert
        table.HiddenColumns.Should().NotBeNull();
        table.HiddenColumns.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that ToggleColumn adds a column to HiddenColumns when it's not present.
    /// </summary>
    [Fact]
    public void ToggleColumn_ShouldAddColumnWhenNotPresent()
    {
        // Arrange
        var table = new DataTable<int>();
        table.SetData(new List<int> { 1, 2, 3 });

        // Act
        table.ToggleColumn("TestColumn");

        // Assert
        table.HiddenColumns.Should().Contain("TestColumn");
    }

    /// <summary>
    /// Verifies that ToggleColumn removes a column from HiddenColumns when it's present.
    /// </summary>
    [Fact]
    public void ToggleColumn_ShouldRemoveColumnWhenPresent()
    {
        // Arrange
        var table = new DataTable<int>();
        table.SetData(new List<int> { 1, 2, 3 });
        table.HiddenColumns = new HashSet<string> { "TestColumn" };

        // Act
        table.ToggleColumn("TestColumn");

        // Assert
        table.HiddenColumns.Should().NotContain("TestColumn");
    }

    /// <summary>
    /// Verifies that ToggleColumn toggles column visibility correctly.
    /// </summary>
    [Fact]
    public void ToggleColumn_ShouldToggleVisibility()
    {
        // Arrange
        var table = new DataTable<int>();
        table.SetData(new List<int> { 1, 2, 3 });

        // Act & Assert - First toggle adds to hidden
        table.ToggleColumn("Column1");
        table.HiddenColumns.Should().Contain("Column1");

        // Act & Assert - Second toggle removes from hidden
        table.ToggleColumn("Column1");
        table.HiddenColumns.Should().NotContain("Column1");

        // Act & Assert - Third toggle adds again
        table.ToggleColumn("Column1");
        table.HiddenColumns.Should().Contain("Column1");
    }

    /// <summary>
    /// Verifies that ToggleColumn throws ArgumentException for null or whitespace column names.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ToggleColumn_ShouldThrowForInvalidColumnName(string? invalidName)
    {
        // Arrange
        var table = new DataTable<int>();
        table.SetData(new List<int> { 1, 2, 3 });

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => table.ToggleColumn(invalidName!));
    }

    /// <summary>
    /// Verifies that ToggleColumn notifies state change.
    /// </summary>
    [Fact]
    public void ToggleColumn_ShouldNotifyStateChanged()
    {
        // Arrange
        var table = new DataTable<int>();
        table.SetData(new List<int> { 1, 2, 3 });

        // Act
        var act = () => table.ToggleColumn("Column1");

        // Assert - Should not throw
        act.Should().NotThrow();
    }

    /// <summary>
    /// Verifies that interface method ToggleColumn is implemented correctly.
    /// </summary>
    [Fact]
    public void IDataTable_ToggleColumn_ShouldBeImplemented()
    {
        // Arrange
        IDataTable<int> table = new DataTable<int>();
        table.SetData(new List<int> { 1, 2, 3 });

        // Act
        table.ToggleColumn("TestColumn");

        // Assert
        table.HiddenColumns.Should().Contain("TestColumn");
    }

    /// <summary>
    /// Verifies that interface property HiddenColumns is implemented correctly.
    /// </summary>
    [Fact]
    public void IDataTable_HiddenColumns_ShouldBeImplemented()
    {
        // Arrange
        IDataTable<int> table = new DataTable<int>();
        var hiddenColumns = new HashSet<string> { "Column1", "Column2" };

        // Act
        table.HiddenColumns = hiddenColumns;

        // Assert
        table.HiddenColumns.Should().BeEquivalentTo(hiddenColumns);
    }
}