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

    /// <summary>
    /// Verifies that multi-column sorting works correctly with AddSortKey.
    /// </summary>
    [Fact]
    public void AddSortKey_ShouldApplyMultiColumnSorting()
    {
        // Arrange
        var people = new List<Person>
        {
            new Person { FirstName = "John", LastName = "Doe", Age = 30 },
            new Person { FirstName = "Jane", LastName = "Doe", Age = 25 },
            new Person { FirstName = "John", LastName = "Smith", Age = 30 },
            new Person { FirstName = "Alice", LastName = "Doe", Age = 25 },
            new Person { FirstName = "Bob", LastName = "Smith", Age = 35 }
        };

        var table = new DataTable<Person>();
        table.SetData(people);
        table.IsSortable = true;

        // Act - First sort by LastName ascending
        table.SortBy(p => p.LastName, SortDirection.Ascending);

        // Assert - All "Doe" should come before "Smith"
        var field = typeof(DataTable<Person>).GetField("_currentViewData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var viewData = (IEnumerable<Person>)field!.GetValue(table)!;
        var sortedList = viewData.ToList();

        sortedList.Should().HaveCount(5);
        sortedList.Select(p => p.LastName).Should().Equal(new[] { "Doe", "Doe", "Doe", "Smith", "Smith" });

        // Act - Add secondary sort by Age ascending
        table.AddSortKey(p => p.Age, SortDirection.Ascending);

        // Assert - Within same last name, sort by age ascending
        viewData = (IEnumerable<Person>)field!.GetValue(table)!;
        sortedList = viewData.ToList();

        sortedList.Select(p => new { p.LastName, p.Age }).Should().Equal(new[]
        {
            new { LastName = "Doe", Age = 25 },
            new { LastName = "Doe", Age = 25 },
            new { LastName = "Doe", Age = 30 },
            new { LastName = "Smith", Age = 30 },
            new { LastName = "Smith", Age = 35 }
        });

        // Act - Add tertiary sort by FirstName ascending
        table.AddSortKey(p => p.FirstName, SortDirection.Ascending);

        // Assert - Within same last name and age, sort by first name
        viewData = (IEnumerable<Person>)field!.GetValue(table)!;
        sortedList = viewData.ToList();

        sortedList.Select(p => new { p.LastName, p.Age, p.FirstName }).Should().Equal(new[]
        {
            new { LastName = "Doe", Age = 25, FirstName = "Alice" },
            new { LastName = "Doe", Age = 25, FirstName = "Jane" },
            new { LastName = "Doe", Age = 30, FirstName = "John" },
            new { LastName = "Smith", Age = 30, FirstName = "John" },
            new { LastName = "Smith", Age = 35, FirstName = "Bob" }
        });
    }

    /// <summary>
    /// Verifies that ClearSort resets the sorting.
    /// </summary>
    [Fact]
    public void ClearSort_ShouldResetSorting()
    {
        // Arrange
        var people = new List<Person>
        {
            new Person { FirstName = "John", LastName = "Doe", Age = 30 },
            new Person { FirstName = "Jane", LastName = "Doe", Age = 25 }
        };

        var table = new DataTable<Person>();
        table.SetData(people);
        table.IsSortable = true;

        // Act - Sort the data
        table.SortBy(p => p.LastName, SortDirection.Ascending);

        // Assert - Data should be sorted
        var field = typeof(DataTable<Person>).GetField("_currentViewData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var viewData = (IEnumerable<Person>)field!.GetValue(table)!;
        viewData.Should().BeInAscendingOrder(p => p.LastName);

        // Act - Clear the sort
        table.ClearSort();

        // Assert - Data should be in original order
        viewData = (IEnumerable<Person>)field!.GetValue(table)!;
        viewData.Should().Equal(people);
    }

    private class Person
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int Age { get; set; }
    }
}
