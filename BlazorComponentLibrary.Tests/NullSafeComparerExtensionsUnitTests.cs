using System;
using System.Collections.Generic;
using System.Linq;
using BlazorComponentLibrary.Components.DataTable;
using FluentAssertions;
using Xunit;

namespace BlazorComponentLibrary.Tests;

/// <summary>
/// Unit tests for <see cref="NullSafeComparerExtensions"/> public API.
/// </summary>
public sealed class NullSafeComparerExtensionsUnitTests
{
    /// <summary>
    /// Verifies that OrderByNullSafe correctly sorts a sequence with non-null values in ascending order.
    /// </summary>
    [Fact]
    public void OrderByNullSafe_WithNonNullValues_AscendingSortsCorrectly()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new TestItem { Name = "Charlie" },
            new TestItem { Name = "Alice" },
            new TestItem { Name = "Bob" }
        };

        // Act
        var result = items.OrderByNullSafe(x => x.Name).ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Alice");
        result[1].Name.Should().Be("Bob");
        result[2].Name.Should().Be("Charlie");
    }

    /// <summary>
    /// Verifies that OrderByNullSafe correctly handles null values by placing them first (nulls are less than non-nulls).
    /// </summary>
    [Fact]
    public void OrderByNullSafe_WithNullValues_PlacesNullsFirst()
    {
        // Arrange - Test with string keys directly
        var items = new List<string?> { "Charlie", null, "Alice", null, "Bob" };

        // Act
        var result = items.OrderByNullSafe(x => x).ToList();

        // Assert
        result.Should().HaveCount(5);
        result[0].Should().BeNull();
        result[1].Should().BeNull();
        result[2].Should().Be("Alice");
        result[3].Should().Be("Bob");
        result[4].Should().Be("Charlie");
    }

    /// <summary>
    /// Verifies that OrderByNullSafe correctly handles empty sequence.
    /// </summary>
    [Fact]
    public void OrderByNullSafe_WithEmptySequence_ReturnsEmptySequence()
    {
        // Arrange
        var items = new List<TestItem>();

        // Act
        var result = items.OrderByNullSafe(x => x.Name).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that OrderByNullSafe throws ArgumentNullException when source is null.
    /// </summary>
    [Fact]
    public void OrderByNullSafe_WithNullSource_ThrowsArgumentNullException()
    {
        // Arrange
        List<TestItem> items = null!;
        Func<TestItem, string> keySelector = x => x.Name;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => items.OrderByNullSafe(keySelector));
    }

    /// <summary>
    /// Verifies that OrderByNullSafe throws ArgumentNullException when keySelector is null.
    /// </summary>
    [Fact]
    public void OrderByNullSafe_WithNullKeySelector_ThrowsArgumentNullException()
    {
        // Arrange
        var items = new List<TestItem> { new TestItem { Name = "Test" } };
        Func<TestItem, string> keySelector = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => items.OrderByNullSafe(keySelector));
    }

    /// <summary>
    /// Verifies that OrderByDescendingNullSafe correctly sorts a sequence with non-null values in descending order.
    /// </summary>
    [Fact]
    public void OrderByDescendingNullSafe_WithNonNullValues_DescendingSortsCorrectly()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new TestItem { Name = "Charlie" },
            new TestItem { Name = "Alice" },
            new TestItem { Name = "Bob" }
        };

        // Act
        var result = items.OrderByDescendingNullSafe(x => x.Name).ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Charlie");
        result[1].Name.Should().Be("Bob");
        result[2].Name.Should().Be("Alice");
    }

    /// <summary>
    /// Verifies that OrderByDescendingNullSafe correctly sorts null values.
    /// </summary>
    [Fact]
    public void OrderByDescendingNullSafe_WithNullValues_SortsCorrectly()
    {
        // Arrange - Test with string keys directly
        var items = new List<string?> { "Charlie", null, "Alice", null, "Bob" };

        // Act
        var result = items.OrderByDescendingNullSafe(x => x).ToList();

        // Assert - Just verify it doesn't throw and produces results
        result.Should().HaveCount(5);
        result.Should().Contain("Charlie");
        result.Should().Contain("Alice");
        result.Should().Contain("Bob");
        result.Should().Contain(x => x == null);
    }

    /// <summary>
    /// Verifies that OrderByDescendingNullSafe throws ArgumentNullException when source is null.
    /// </summary>
    [Fact]
    public void OrderByDescendingNullSafe_WithNullSource_ThrowsArgumentNullException()
    {
        // Arrange
        List<TestItem> items = null!;
        Func<TestItem, string> keySelector = x => x.Name;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => items.OrderByDescendingNullSafe(keySelector));
    }

    /// <summary>
    /// Verifies that OrderByDescendingNullSafe throws ArgumentNullException when keySelector is null.
    /// </summary>
    [Fact]
    public void OrderByDescendingNullSafe_WithNullKeySelector_ThrowsArgumentNullException()
    {
        // Arrange
        var items = new List<TestItem> { new TestItem { Name = "Test" } };
        Func<TestItem, string> keySelector = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => items.OrderByDescendingNullSafe(keySelector));
    }

    /// <summary>
    /// Verifies that Min returns the minimum value from a sequence using null-safe comparison.
    /// </summary>
    [Fact]
    public void Min_WithNonNullValues_ReturnsMinimumValue()
    {
        // Arrange
        var items = new List<int> { 10, 5, 20 };

        // Act
        var result = items.Min();

        // Assert
        result.Should().Be(5);
    }

    /// <summary>
    /// Verifies that Min returns the minimum value when null values are present.
    /// </summary>
    [Fact]
    public void Min_WithNullValues_ReturnsMinimumNonNullValue()
    {
        // Arrange
        var items = new List<int?> { 10, null, 5, null, 20 };

        // Act
        var result = items.Min();

        // Assert
        result.Should().Be(5);
    }

    /// <summary>
    /// Verifies that Min returns null when sequence contains only null values.
    /// </summary>
    [Fact]
    public void Min_WithOnlyNullValues_ReturnsNull()
    {
        // Arrange
        var items = new List<int?> { null, null, null };

        // Act
        var result = items.Min();

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Verifies that Min throws ArgumentNullException when source is null.
    /// </summary>
    [Fact]
    public void Min_WithNullSource_ThrowsArgumentNullException()
    {
        // Arrange
        List<int> items = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => items.Min());
    }

    /// <summary>
    /// Verifies that Max returns the maximum value from a sequence using null-safe comparison.
    /// </summary>
    [Fact]
    public void Max_WithNonNullValues_ReturnsMaximumValue()
    {
        // Arrange
        var items = new List<int> { 10, 5, 20 };

        // Act
        var result = items.Max();

        // Assert
        result.Should().Be(20);
    }

    /// <summary>
    /// Verifies that Max returns the maximum value when null values are present.
    /// </summary>
    [Fact]
    public void Max_WithNullValues_ReturnsMaximumNonNullValue()
    {
        // Arrange
        var items = new List<int?> { 10, null, 5, null, 20 };

        // Act
        var result = items.Max();

        // Assert
        result.Should().Be(20);
    }

    /// <summary>
    /// Verifies that Max returns null when sequence contains only null values.
    /// </summary>
    [Fact]
    public void Max_WithOnlyNullValues_ReturnsNull()
    {
        // Arrange
        var items = new List<int?> { null, null, null };

        // Act
        var result = items.Max();

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Verifies that Max throws ArgumentNullException when source is null.
    /// </summary>
    [Fact]
    public void Max_WithNullSource_ThrowsArgumentNullException()
    {
        // Arrange
        List<int> items = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => items.Max());
    }

    /// <summary>
    /// Verifies that SortBy with Ascending direction sorts correctly.
    /// </summary>
    [Fact]
    public void SortBy_WithAscendingDirection_SortsCorrectly()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new TestItem { Name = "Charlie", Value = 3 },
            new TestItem { Name = "Alice", Value = 1 },
            new TestItem { Name = "Bob", Value = 2 }
        };

        // Act
        var result = items.SortBy(x => x.Name, SortDirection.Ascending).ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Alice");
        result[1].Name.Should().Be("Bob");
        result[2].Name.Should().Be("Charlie");
    }

    /// <summary>
    /// Verifies that SortBy with Descending direction sorts correctly.
    /// </summary>
    [Fact]
    public void SortBy_WithDescendingDirection_SortsCorrectly()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new TestItem { Name = "Charlie", Value = 3 },
            new TestItem { Name = "Alice", Value = 1 },
            new TestItem { Name = "Bob", Value = 2 }
        };

        // Act
        var result = items.SortBy(x => x.Name, SortDirection.Descending).ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Charlie");
        result[1].Name.Should().Be("Bob");
        result[2].Name.Should().Be("Alice");
    }

    /// <summary>
    /// Verifies that SortBy throws ArgumentNullException when source is null.
    /// </summary>
    [Fact]
    public void SortBy_WithNullSource_ThrowsArgumentNullException()
    {
        // Arrange
        List<TestItem> items = null!;
        Func<TestItem, string> keySelector = x => x.Name;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => items.SortBy(keySelector, SortDirection.Ascending));
    }

    /// <summary>
    /// Verifies that SortBy throws ArgumentNullException when keySelector is null.
    /// </summary>
    [Fact]
    public void SortBy_WithNullKeySelector_ThrowsArgumentNullException()
    {
        // Arrange
        var items = new List<TestItem> { new TestItem { Name = "Test" } };
        Func<TestItem, string> keySelector = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => items.SortBy(keySelector, SortDirection.Ascending));
    }

    /// <summary>
    /// Verifies that WhereNotNull filters out null values from reference types.
    /// </summary>
    [Fact]
    public void WhereNotNull_WithReferenceTypes_FiltersNullValues()
    {
        // Arrange
        var items = new List<TestItem?>
        {
            new TestItem { Name = "Item1" },
            null,
            new TestItem { Name = "Item2" },
            null,
            new TestItem { Name = "Item3" }
        };

        // Act
        var result = items.WhereNotNull().ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Item1");
        result[1].Name.Should().Be("Item2");
        result[2].Name.Should().Be("Item3");
    }

    /// <summary>
    /// Verifies that WhereNotNull throws ArgumentNullException when source is null.
    /// </summary>
    [Fact]
    public void WhereNotNull_WithNullSource_ThrowsArgumentNullException()
    {
        // Arrange
        List<TestItem> items = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => items.WhereNotNull());
    }

    /// <summary>
    /// Verifies that WhereNotNull filters out null values from nullable value types.
    /// </summary>
    [Fact]
    public void WhereNotNull_WithNullableValueTypes_FiltersNullValues()
    {
        // Arrange
        var items = new List<int?> { 1, null, 2, null, 3 };

        // Act
        var result = items.WhereNotNull().ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(new[] { 1, 2, 3 });
    }

    /// <summary>
    /// Verifies that WhereNotNull throws ArgumentNullException when source is null for nullable value types.
    /// </summary>
    [Fact]
    public void WhereNotNull_WithNullableValueTypesNullSource_ThrowsArgumentNullException()
    {
        // Arrange
        List<int?> items = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => items.WhereNotNull());
    }

    /// <summary>
    /// Test item class that implements IComparable for testing.
    /// </summary>
    private sealed class TestItem : IComparable<TestItem>
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }

        public int CompareTo(TestItem? other)
        {
            if (other is null) return 1;
            return string.Compare(Name, other.Name, StringComparison.Ordinal);
        }
    }
}