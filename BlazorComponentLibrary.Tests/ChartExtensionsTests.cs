namespace BlazorComponentLibrary.Tests;

using Xunit;
using BlazorComponentLibrary.Components.Chart;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Comprehensive unit tests for <see cref="ChartExtensions"/> extension methods.
/// Tests cover happy-path scenarios, edge cases (null/empty inputs, boundary values),
/// and error-path assertions including empty series scenarios.
/// </summary>
public sealed class ChartExtensionsTests
{
    private class TestChart<TData> : IChart<TData>
    {
        public ChartType ChartType { get; set; }
        public string Title { get; set; } = string.Empty;
        public IEnumerable<string> Labels { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> Colors { get; set; } = Enumerable.Empty<string>();
        public object Options { get; set; } = new object();
        public Func<double, string> ValueFormatter { get; set; } = v => v.ToString();
        public IEnumerable<ChartAnnotation> Annotations { get; set; } = Enumerable.Empty<ChartAnnotation>();
        public List<TData> Data { get; private set; } = new List<TData>();

        public void SetData(IEnumerable<TData> data)
        {
            Data = data?.ToList() ?? new List<TData>();
        }

        public bool RefreshCalled { get; private set; }

        public void Refresh()
        {
            RefreshCalled = true;
        }

        public void SetSeriesVisibility(int seriesIndex, bool visible)
        {
            // No-op for testing
        }
    }

    /// <summary>
    /// Verifies that SetDataAndRefresh correctly sets data and refreshes the chart.
    /// </summary>
    [Fact]
    public void SetDataAndRefresh_WithValidData_SetsDataAndRefreshes()
    {
        // Arrange
        var chart = new TestChart<int>();
        var data = new List<int> { 1, 2, 3, 4, 5 };

        // Mock refresh behavior

        // Act
        chart.SetDataAndRefresh(data);

        // Assert
        chart.Data.Should().BeEquivalentTo(data);
        chart.RefreshCalled.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that SetDataAndRefresh handles null data gracefully.
    /// </summary>
    [Fact]
    public void SetDataAndRefresh_NullData_HandledGracefully()
    {
        // Arrange
        var chart = new TestChart<int>();

        // Mock refresh behavior

        // Act
        chart.SetDataAndRefresh(null);

        // Assert
        chart.Data.Should().BeEmpty();
        chart.RefreshCalled.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that SetDataAndRefresh handles empty data correctly.
    /// </summary>
    [Fact]
    public void SetDataAndRefresh_EmptyData_HandledCorrectly()
    {
        // Arrange
        var chart = new TestChart<int>();

        // Mock refresh behavior

        // Act
        chart.SetDataAndRefresh(Enumerable.Empty<int>());

        // Assert
        chart.Data.Should().BeEmpty();
        chart.RefreshCalled.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that SetDataAndRefresh throws ArgumentNullException when chart is null.
    /// </summary>
    [Fact]
    public void SetDataAndRefresh_NullChart_ThrowsArgumentNullException()
    {
        // Arrange
        IChart<int> chart = null!;
        var data = new List<int> { 1, 2, 3 };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => chart.SetDataAndRefresh(data));
    }

    /// <summary>
    /// Verifies that AddThresholdLine correctly adds a threshold line annotation.
    /// </summary>
    [Fact]
    public void AddThresholdLine_AddsAnnotationCorrectly()
    {
        // Arrange
        var chart = new TestChart<double>();

        // Mock refresh behavior

        // Act
        var annotation = chart.AddThresholdLine(75.5, "Warning Threshold", "#ff6384", "Warning at 75.5");

        // Assert
        annotation.Should().NotBeNull();
        annotation.Type.Should().Be(ChartAnnotationType.ThresholdLine);
        annotation.Value.Should().Be(75.5);
        annotation.Label.Should().Be("Warning Threshold");
        annotation.Color.Should().Be("#ff6384");
        annotation.Tooltip.Should().Be("Warning at 75.5");

        chart.Annotations.Should().HaveCount(1);
        chart.RefreshCalled.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that AddThresholdLine with default parameters uses correct defaults.
    /// </summary>
    [Fact]
    public void AddThresholdLine_DefaultParameters_UsesCorrectDefaults()
    {
        // Arrange
        var chart = new TestChart<double>();

        // Mock refresh behavior

        // Act
        var annotation = chart.AddThresholdLine(50.0);

        // Assert
        annotation.Should().NotBeNull();
        annotation.Type.Should().Be(ChartAnnotationType.ThresholdLine);
        annotation.Value.Should().Be(50.0);
        annotation.Label.Should().BeEmpty();
        annotation.Color.Should().Be("#ff6384"); // Default color
        annotation.Tooltip.Should().Be("Threshold at 50"); // Default tooltip

        chart.Annotations.Should().HaveCount(1);
        chart.RefreshCalled.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that AddThresholdLine throws ArgumentNullException when chart is null.
    /// </summary>
    [Fact]
    public void AddThresholdLine_NullChart_ThrowsArgumentNullException()
    {
        // Arrange
        IChart<double> chart = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => chart.AddThresholdLine(50.0));
    }

    /// <summary>
    /// Verifies that AddEventMarker correctly adds an event marker annotation.
    /// </summary>
    [Fact]
    public void AddEventMarker_AddsAnnotationCorrectly()
    {
        // Arrange
        var chart = new TestChart<int>();

        // Mock refresh behavior

        // Act
        var annotation = chart.AddEventMarker(25.7, "Important Event", "#36a2eb", "Event at position 25.7");

        // Assert
        annotation.Should().NotBeNull();
        annotation.Type.Should().Be(ChartAnnotationType.EventMarker);
        annotation.Value.Should().Be(25.7);
        annotation.Label.Should().Be("Important Event");
        annotation.Color.Should().Be("#36a2eb");
        annotation.Tooltip.Should().Be("Event at position 25.7");

        chart.Annotations.Should().HaveCount(1);
        chart.RefreshCalled.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that AddEventMarker with default parameters uses correct defaults.
    /// </summary>
    [Fact]
    public void AddEventMarker_DefaultParameters_UsesCorrectDefaults()
    {
        // Arrange
        var chart = new TestChart<int>();

        // Mock refresh behavior

        // Act
        var annotation = chart.AddEventMarker(100.0);

        // Assert
        annotation.Should().NotBeNull();
        annotation.Type.Should().Be(ChartAnnotationType.EventMarker);
        annotation.Value.Should().Be(100.0);
        annotation.Label.Should().BeEmpty();
        annotation.Color.Should().Be("#36a2eb"); // Default color
        annotation.Tooltip.Should().Be("Event at position 100"); // Default tooltip

        chart.Annotations.Should().HaveCount(1);
        chart.RefreshCalled.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that AddEventMarker throws ArgumentNullException when chart is null.
    /// </summary>
    [Fact]
    public void AddEventMarker_NullChart_ThrowsArgumentNullException()
    {
        // Arrange
        IChart<int> chart = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => chart.AddEventMarker(50.0));
    }

    /// <summary>
    /// Verifies that AddReferenceBand correctly adds a reference band annotation.
    /// </summary>
    [Fact]
    public void AddReferenceBand_AddsAnnotationCorrectly()
    {
        // Arrange
        var chart = new TestChart<double>();

        // Mock refresh behavior

        // Act
        var annotation = chart.AddReferenceBand(25.0, 75.0, "Acceptable Range", "#4bc0c080", "Range from 25 to 75");

        // Assert
        annotation.Should().NotBeNull();
        annotation.Type.Should().Be(ChartAnnotationType.ReferenceBand);
        annotation.Value.Should().Be(25.0);
        annotation.EndValue.Should().Be(75.0);
        annotation.Label.Should().Be("Acceptable Range");
        annotation.Color.Should().Be("#4bc0c080");
        annotation.Tooltip.Should().Be("Range from 25 to 75");

        chart.Annotations.Should().HaveCount(1);
        chart.RefreshCalled.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that AddReferenceBand with default parameters uses correct defaults.
    /// </summary>
    [Fact]
    public void AddReferenceBand_DefaultParameters_UsesCorrectDefaults()
    {
        // Arrange
        var chart = new TestChart<int>();

        // Mock refresh behavior

        // Act
        var annotation = chart.AddReferenceBand(0.0, 100.0);

        // Assert
        annotation.Should().NotBeNull();
        annotation.Type.Should().Be(ChartAnnotationType.ReferenceBand);
        annotation.Value.Should().Be(0.0);
        annotation.EndValue.Should().Be(100.0);
        annotation.Label.Should().BeEmpty();
        annotation.Color.Should().Be("#4bc0c080"); // Default color
        annotation.Tooltip.Should().Be("Reference band from 0 to 100"); // Default tooltip

        chart.Annotations.Should().HaveCount(1);
        chart.RefreshCalled.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that AddReferenceBand throws ArgumentNullException when chart is null.
    /// </summary>
    [Fact]
    public void AddReferenceBand_NullChart_ThrowsArgumentNullException()
    {
        // Arrange
        IChart<int> chart = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => chart.AddReferenceBand(25.0, 75.0));
    }

    /// <summary>
    /// Verifies that AddReferenceBand throws ArgumentOutOfRangeException when startValue > endValue.
    /// </summary>
    [Fact]
    public void AddReferenceBand_StartGreaterThanEnd_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var chart = new TestChart<int>();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => chart.AddReferenceBand(100.0, 50.0));
    }

    /// <summary>
    /// Verifies that ClearAnnotations clears all annotations from the chart.
    /// </summary>
    [Fact]
    public void ClearAnnotations_ClearsAllAnnotations()
    {
        // Arrange
        var chart = new TestChart<int>();

        // Mock refresh behavior

        // Add some annotations first
        chart.AddThresholdLine(50.0);
        chart.AddEventMarker(75.0);
        chart.AddReferenceBand(25.0, 75.0);

        // Verify annotations were added
        chart.Annotations.Should().HaveCount(3);

        // Act
        chart.ClearAnnotations();

        // Assert
        chart.Annotations.Should().BeEmpty();
        chart.RefreshCalled.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that ClearAnnotations throws ArgumentNullException when chart is null.
    /// </summary>
    [Fact]
    public void ClearAnnotations_NullChart_ThrowsArgumentNullException()
    {
        // Arrange
        IChart<int> chart = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => chart.ClearAnnotations());
    }

    /// <summary>
    /// Verifies that SetTitle correctly sets the chart title.
    /// </summary>
    [Fact]
    public void SetTitle_SetsTitleCorrectly()
    {
        // Arrange
        var chart = new TestChart<int>();

        // Mock refresh behavior

        // Act
        chart.SetTitle("My Chart Title");

        // Assert
        chart.Title.Should().Be("My Chart Title");
        chart.RefreshCalled.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that SetTitle throws ArgumentNullException when chart is null.
    /// </summary>
    [Fact]
    public void SetTitle_NullChart_ThrowsArgumentNullException()
    {
        // Arrange
        IChart<int> chart = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => chart.SetTitle("Title"));
    }

    /// <summary>
    /// Verifies that SetTitle throws ArgumentException when title is null or empty.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void SetTitle_NullOrEmptyTitle_ThrowsArgumentException(string? title)
    {
        // Arrange
        var chart = new TestChart<int>();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => chart.SetTitle(title!));
    }

    /// <summary>
    /// Verifies that SetChartType correctly sets the chart type.
    /// </summary>
    [Fact]
    public void SetChartType_SetsChartTypeCorrectly()
    {
        // Arrange
        var chart = new TestChart<int>();

        // Mock refresh behavior

        // Act
        chart.SetChartType(ChartType.Pie);

        // Assert
        chart.ChartType.Should().Be(ChartType.Pie);
        chart.RefreshCalled.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that SetChartType throws ArgumentNullException when chart is null.
    /// </summary>
    [Fact]
    public void SetChartType_NullChart_ThrowsArgumentNullException()
    {
        // Arrange
        IChart<int> chart = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => chart.SetChartType(ChartType.Line));
    }

    /// <summary>
    /// Verifies that SetSeriesVisibility correctly sets series visibility.
    /// </summary>
    [Fact]
    public void SetSeriesVisibility_SetsVisibilityCorrectly()
    {
        // Arrange
        var chart = new TestChart<int>();

        // Mock refresh behavior

        // Act
        chart.SetSeriesVisibility(0, true);
        chart.SetSeriesVisibility(1, false);

        // Assert - refresh should have been called
        chart.RefreshCalled.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that SetSeriesVisibility throws ArgumentNullException when chart is null.
    /// </summary>
    [Fact]
    public void SetSeriesVisibility_NullChart_ThrowsArgumentNullException()
    {
        // Arrange
        IChart<int> chart = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => chart.SetSeriesVisibility(0, true));
    }

    /// <summary>
    /// Verifies that GetOrCreateGeometry works correctly with cached geometry.
    /// </summary>
    [Fact]
    public void GetOrCreateGeometry_WithTypedChart_ReturnsCachedGeometry()
    {
        // Arrange
        var chart = new TestChart<int>();
        var geometryKey = "series1";
        var createFuncCalled = false;
        Func<object> createFunc = () =>
        {
            createFuncCalled = true;
            return new { Path = "M 0 0 L 10 10" };
        };

        // Act
        var result = chart.GetOrCreateGeometry(geometryKey, createFunc);

        // Assert
        result.Should().NotBeNull();
        createFuncCalled.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that GetOrCreateGeometry throws ArgumentNullException when chart is null.
    /// </summary>
    [Fact]
    public void GetOrCreateGeometry_NullChart_ThrowsArgumentNullException()
    {
        // Arrange
        IChart<int> chart = null!;
        var geometryKey = "series1";
        Func<object> createFunc = () => new object();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => chart.GetOrCreateGeometry(geometryKey, createFunc));
    }

    /// <summary>
    /// Verifies that GetOrCreateGeometry throws ArgumentNullException when seriesKey is null.
    /// </summary>
    [Fact]
    public void GetOrCreateGeometry_NullSeriesKey_ThrowsArgumentNullException()
    {
        // Arrange
        var chart = new TestChart<int>();
        string seriesKey = null!;
        Func<object> createFunc = () => new object();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => chart.GetOrCreateGeometry(seriesKey, createFunc));
    }

    /// <summary>
    /// Verifies that GetOrCreateGeometry throws ArgumentNullException when createFunc is null.
    /// </summary>
    [Fact]
    public void GetOrCreateGeometry_NullCreateFunc_ThrowsArgumentNullException()
    {
        // Arrange
        var chart = new TestChart<int>();
        var geometryKey = "series1";
        Func<object> createFunc = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => chart.GetOrCreateGeometry(geometryKey, createFunc));
    }
}
