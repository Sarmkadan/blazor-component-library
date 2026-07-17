using BlazorComponentLibrary.Components.DataTable;
using FluentAssertions;
using Xunit;

/// <summary>
/// Tests for the NullSafeComparer class.
/// </summary>
public class NullSafeComparerTests
{
    /// <summary>
    /// Verifies that the Compare method places nulls last.
    /// </summary>
    [Fact]
    public void Compare_ShouldPlaceNullsLast()
    {
        var comparer = NullSafeComparer.Instance;

        comparer.Compare("a", null).Should().Be(-1);
        comparer.Compare(null, "a").Should().Be(1);
        comparer.Compare(null, null).Should().Be(0);
    }

    /// <summary>
    /// Verifies that the Compare method compares values correctly.
    /// </summary>
    [Fact]
    public void Compare_ShouldCompareValuesCorrectly()
    {
        var comparer = NullSafeComparer.Instance;

        comparer.Compare(1, 2).Should().Be(-1);
        comparer.Compare(2, 1).Should().Be(1);
        comparer.Compare(1, 1).Should().Be(0);
    }
}

/// <summary>
/// Tests for the DataTable class.
/// </summary>
public class DataTableTests
{
    /// <summary>
    /// Verifies that the SortBy method sorts data correctly.
    /// </summary>
    [Fact]
    public void SortBy_ShouldSortDataCorrectly()
    {
        // Arrange
        var table = new DataTable<int>();
        table.SetData(new List<int> { 3, 1, 2 });
        table.IsSortable = true;

        // Act
        table.SortBy(x => x, SortDirection.Ascending);

        // Assert
        var field = typeof(DataTable<int>).GetField("_currentViewData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var viewData = (IEnumerable<int>)field!.GetValue(table)!;
        
        viewData.Should().Equal(new List<int> { 1, 2, 3 });
    }
}
