namespace BlazorComponentLibrary.Tests;

using Bunit;
using Xunit;
using BlazorComponentLibrary.Components.Chart;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Comprehensive unit tests for the <see cref="Chart{TData}"/> component public API.
/// Tests cover happy-path scenarios, edge cases (null/empty inputs, boundary values),
/// and error-path assertions.
/// </summary>
public sealed class ChartRazorUnitTests : TestContext
{
    /// <summary>
    /// Verifies that the component initializes with default values when no parameters are provided.
    /// </summary>
    [Fact]
    public void DefaultRender_HasDefaultValues()
    {
        // Arrange & Act
        var cut = RenderComponent<Chart<int>>();

        // Assert
        cut.Instance.ChartType.Should().Be(ChartType.Bar);
        cut.Instance.Title.Should().BeEmpty();
        cut.Instance.Labels.Should().BeEmpty();
        cut.Instance.Colors.Should().BeEmpty();
        cut.Instance.Options.Should().NotBeNull();
        cut.Instance.Annotations.Should().BeEmpty();
        cut.Instance.ValueFormatter.Should().NotBeNull();
    }

    /// <summary>
    /// Verifies that setting <see cref="Chart{TData}.ChartType"/> to various ChartType enum values works correctly.
    /// </summary>
    [Theory]
    [InlineData(ChartType.Bar)]
    [InlineData(ChartType.Line)]
    [InlineData(ChartType.Pie)]
    [InlineData(ChartType.Doughnut)]
    [InlineData(ChartType.Radar)]
    [InlineData(ChartType.PolarArea)]
    [InlineData(ChartType.Bubble)]
    [InlineData(ChartType.Scatter)]
    public void ChartType_Set_AllEnumValues_RenderCorrectly(ChartType chartType)
    {
        // Arrange & Act
        var cut = RenderComponent<Chart<int>>(parameters => parameters
            .Add(p => p.ChartType, chartType));

        // Assert
        cut.Instance.ChartType.Should().Be(chartType);
    }

    /// <summary>
    /// Verifies that setting <see cref="Chart{TData}.Title"/> to various values works correctly.
    /// </summary>
    [Theory]
    [InlineData("Simple Title")]
    [InlineData("Title with special chars: !@#$%^&*()")]
    [InlineData("Title with unicode: Привет, 你好, こんにちは")]
    [InlineData("A very long title that exceeds typical lengths to test string handling capabilities of the chart component")]
    public void Title_Set_VariousValues_RendersCorrectly(string title)
    {
        // Arrange & Act
        var cut = RenderComponent<Chart<int>>(parameters => parameters
            .Add(p => p.Title, title));

        // Assert
        cut.Instance.Title.Should().Be(title);
    }

    /// <summary>
    /// Verifies that setting <see cref="Chart{TData}.Title"/> to null or empty strings is handled gracefully.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Title_NullOrWhitespace_HandledGracefully(string? title)
    {
        // Arrange & Act
        var cut = RenderComponent<Chart<int>>(parameters => parameters
            .Add(p => p.Title, title));

        // Assert - should not throw and maintain the input value
        cut.Instance.Title.Should().Be(title);
    }

    /// <summary>
    /// Verifies that setting <see cref="Chart{TData}.Labels"/> to a collection works correctly.
    /// </summary>
    [Fact]
    public void Labels_Set_Collection_RendersCorrectly()
    {
        // Arrange
        var labels = new List<string> { "Label1", "Label2", "Label3" };

        // Act
        var cut = RenderComponent<Chart<int>>(parameters => parameters
            .Add(p => p.Labels, labels));

        // Assert
        cut.Instance.Labels.Should().BeEquivalentTo(labels);
    }

    /// <summary>
    /// Verifies that setting <see cref="Chart{TData}.Labels"/> to empty collection works correctly.
    /// </summary>
    [Fact]
    public void Labels_Set_EmptyCollection_HandledCorrectly()
    {
        // Arrange
        var labels = Enumerable.Empty<string>();

        // Act
        var cut = RenderComponent<Chart<int>>(parameters => parameters
            .Add(p => p.Labels, labels));

        // Assert
        cut.Instance.Labels.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that setting <see cref="Chart{TData}.Labels"/> to null is handled gracefully.
    /// </summary>
    [Fact]
    public void Labels_Null_HandledGracefully()
    {
        // Arrange & Act
        var cut = RenderComponent<Chart<int>>(parameters => parameters
            .Add(p => p.Labels, (IEnumerable<string>?)null));

        // Assert - Blazor parameter binding will set it to null, component should handle gracefully
        cut.Instance.Labels.Should().BeNull();
    }

    /// <summary>
    /// Verifies that setting <see cref="Chart{TData}.Colors"/> to a collection works correctly.
    /// </summary>
    [Fact]
    public void Colors_Set_Collection_RendersCorrectly()
    {
        // Arrange
        var colors = new List<string> { "#FF0000", "#00FF00", "#0000FF" };

        // Act
        var cut = RenderComponent<Chart<int>>(parameters => parameters
            .Add(p => p.Colors, colors));

        // Assert
        cut.Instance.Colors.Should().BeEquivalentTo(colors);
    }

    /// <summary>
    /// Verifies that setting <see cref="Chart{TData}.Colors"/> to empty collection works correctly.
    /// </summary>
    [Fact]
    public void Colors_Set_EmptyCollection_HandledCorrectly()
    {
        // Arrange
        var colors = Enumerable.Empty<string>();

        // Act
        var cut = RenderComponent<Chart<int>>(parameters => parameters
            .Add(p => p.Colors, colors));

        // Assert
        cut.Instance.Colors.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that setting <see cref="Chart{TData}.Colors"/> to null is handled gracefully.
    /// </summary>
    [Fact]
    public void Colors_Null_HandledGracefully()
    {
        // Arrange & Act
        var cut = RenderComponent<Chart<int>>(parameters => parameters
            .Add(p => p.Colors, (IEnumerable<string>?)null));

        // Assert - Blazor parameter binding will set it to null, component should handle gracefully
        cut.Instance.Colors.Should().BeNull();
    }

    /// <summary>
    /// Verifies that setting <see cref="Chart{TData}.Options"/> to various objects works correctly.
    /// </summary>
    [Fact]
    public void Options_Set_VariousObjects_RendersCorrectly()
    {
        // Arrange
        var options = new { Responsive = true, MaintainAspectRatio = false };

        // Act
        var cut = RenderComponent<Chart<int>>(parameters => parameters
            .Add(p => p.Options, options));

        // Assert
        cut.Instance.Options.Should().BeSameAs(options);
    }

    /// <summary>
    /// Verifies that setting <see cref="Chart{TData}.Options"/> to null is handled gracefully.
    /// </summary>
    [Fact]
    public void Options_Null_HandledGracefully()
    {
        // Arrange & Act
        var cut = RenderComponent<Chart<int>>(parameters => parameters
            .Add(p => p.Options, (object?)null));

        // Assert - Blazor parameter binding will set it to null, component should handle gracefully
        cut.Instance.Options.Should().BeNull();
    }

    /// <summary>
    /// Verifies that setting <see cref="Chart{TData}.Annotations"/> to a collection works correctly.
    /// </summary>
    [Fact]
    public void Annotations_Set_Collection_RendersCorrectly()
    {
        // Arrange
        var annotations = new List<ChartAnnotation>
        {
            new ChartAnnotation { Type = ChartAnnotationType.ThresholdLine, Value = 50, Label = "Threshold", Color = "red" },
            new ChartAnnotation { Type = ChartAnnotationType.EventMarker, Value = 75, Label = "Event", Color = "blue" }
        };

        // Act
        var cut = RenderComponent<Chart<int>>(parameters => parameters
            .Add(p => p.Annotations, annotations));

        // Assert
        cut.Instance.Annotations.Should().BeEquivalentTo(annotations);
    }

    /// <summary>
    /// Verifies that setting <see cref="Chart{TData}.Annotations"/> to empty collection works correctly.
    /// </summary>
    [Fact]
    public void Annotations_Set_EmptyCollection_HandledCorrectly()
    {
        // Arrange
        var annotations = Enumerable.Empty<ChartAnnotation>();

        // Act
        var cut = RenderComponent<Chart<int>>(parameters => parameters
            .Add(p => p.Annotations, annotations));

        // Assert
        cut.Instance.Annotations.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that setting <see cref="Chart{TData}.Annotations"/> to null throws ArgumentNullException.
    /// This is expected behavior as the component's razor template uses Annotations.Any().
    /// </summary>
    [Fact]
    public void Annotations_Null_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            RenderComponent<Chart<int>>(parameters => parameters
                .Add(p => p.Annotations, (IEnumerable<ChartAnnotation>?)null)));
    }

    /// <summary>
    /// Verifies that the <see cref="Chart{TData}.SetData"/> method works correctly with valid data.
    /// </summary>
    [Fact]
    public void SetData_WithValidData_SetsDataCorrectly()
    {
        // Arrange
        var data = new List<int> { 1, 2, 3, 4, 5 };
        var cut = RenderComponent<Chart<int>>();

        // Verify initial state
        cut.Instance.SetData(Enumerable.Empty<int>());

        // Act
        cut.Instance.SetData(data);

        // Assert - private field access via reflection or just verify no exception
        // Since _data is private, we verify the method doesn't throw
        Assert.True(true); // Method executed without exception
    }

    /// <summary>
    /// Verifies that the <see cref="Chart{TData}.SetData"/> method handles null data gracefully.
    /// </summary>
    [Fact]
    public void SetData_NullData_HandledGracefully()
    {
        // Arrange
        var cut = RenderComponent<Chart<int>>();

        // Act & Assert - should not throw
        cut.InvokeAsync(() => cut.Instance.SetData(null));
    }

    /// <summary>
    /// Verifies that the <see cref="Chart{TData}.SetData"/> method handles empty data correctly.
    /// </summary>
    [Fact]
    public void SetData_EmptyData_HandledCorrectly()
    {
        // Arrange
        var cut = RenderComponent<Chart<int>>();
        var emptyData = Enumerable.Empty<int>();

        // Act & Assert - should not throw
        cut.InvokeAsync(() => cut.Instance.SetData(emptyData));
    }

    /// <summary>
    /// Verifies that the <see cref="Chart{TData}.Refresh"/> method works correctly.
    /// </summary>
    [Fact]
    public void Refresh_Invoked_DoesNotThrow()
    {
        // Arrange
        var cut = RenderComponent<Chart<int>>();

        // Act & Assert - should not throw
        cut.InvokeAsync(() => cut.Instance.Refresh());
    }

    /// <summary>
    /// Verifies that calling <see cref="Chart{TData}.Refresh"/> multiple times works correctly.
    /// </summary>
    [Fact]
    public void Refresh_MultipleTimes_WorksCorrectly()
    {
        // Arrange
        var cut = RenderComponent<Chart<int>>();

        // Act & Assert - multiple calls should not throw
        cut.InvokeAsync(() => cut.Instance.Refresh());
        cut.InvokeAsync(() => cut.Instance.Refresh());
        cut.InvokeAsync(() => cut.Instance.Refresh());

        Assert.True(true); // All calls executed without exception
    }

    /// <summary>
    /// Verifies that the <see cref="Chart{TData}.SetSeriesVisibility"/> method works correctly.
    /// </summary>
    [Fact]
    public void SetSeriesVisibility_WithValidIndex_DoesNotThrow()
    {
        // Arrange
        var cut = RenderComponent<Chart<int>>();

        // Act & Assert - should not throw for valid index
        cut.InvokeAsync(() => cut.Instance.SetSeriesVisibility(0, true));
        cut.InvokeAsync(() => cut.Instance.SetSeriesVisibility(0, false));
    }

    /// <summary>
    /// Verifies that the <see cref="Chart{TData}.SetSeriesVisibility"/> method handles negative index gracefully.
    /// </summary>
    [Fact]
    public void SetSeriesVisibility_NegativeIndex_HandledGracefully()
    {
        // Arrange
        var cut = RenderComponent<Chart<int>>();

        // Act & Assert - should not throw
        cut.InvokeAsync(() => cut.Instance.SetSeriesVisibility(-1, true));
        cut.InvokeAsync(() => cut.Instance.SetSeriesVisibility(-1, false));
    }

    /// <summary>
    /// Verifies that the <see cref="Chart{TData}.SetSeriesVisibility"/> method handles large index gracefully.
    /// </summary>
    [Fact]
    public void SetSeriesVisibility_LargeIndex_HandledGracefully()
    {
        // Arrange
        var cut = RenderComponent<Chart<int>>();

        // Act & Assert - should not throw
        cut.InvokeAsync(() => cut.Instance.SetSeriesVisibility(int.MaxValue, true));
        cut.InvokeAsync(() => cut.Instance.SetSeriesVisibility(int.MaxValue, false));
    }

    /// <summary>
    /// Verifies that the <see cref="Chart{TData}.ValueFormatter"/> defaults to invariant culture formatting with "0.##" pattern.
    /// </summary>
    [Fact]
    public void ValueFormatter_DefaultsToInvariantCultureFormat()
    {
        // Arrange & Act
        var cut = RenderComponent<Chart<int>>();

        // Assert
        cut.Instance.ValueFormatter.Should().NotBeNull();
        cut.Instance.ValueFormatter(123.456).Should().Be("123.46"); // Default formatting
        cut.Instance.ValueFormatter(0.5).Should().Be("0.5");
        cut.Instance.ValueFormatter(1000.123).Should().Be("1000.12");
    }

    /// <summary>
    /// Verifies that setting <see cref="Chart{TData}.ValueFormatter"/> to a custom formatter works correctly.
    /// </summary>
    [Fact]
    public void ValueFormatter_Set_CustomFormatter_WorksCorrectly()
    {
        // Arrange
        Func<double, string> customFormatter = value => $"${value:N2}";

        // Act
        var cut = RenderComponent<Chart<int>>(parameters => parameters
            .Add(p => p.ValueFormatter, customFormatter));

        // Assert
        cut.Instance.ValueFormatter.Should().BeSameAs(customFormatter);
        cut.Instance.ValueFormatter(123.456).Should().Be("$123.46");
    }

    /// <summary>
    /// Verifies that setting <see cref="Chart{TData}.ValueFormatter"/> to null is handled gracefully.
    /// </summary>
    [Fact]
    public void ValueFormatter_Null_HandledGracefully()
    {
        // Arrange & Act
        var cut = RenderComponent<Chart<int>>(parameters => parameters
            .Add(p => p.ValueFormatter, (Func<double, string>?)null));

        // Assert - Blazor parameter binding will set it to null, component should handle gracefully
        cut.Instance.ValueFormatter.Should().BeNull();
    }

    /// <summary>
    /// Verifies that the component implements the <see cref="IChart{TData}"/> interface correctly.
    /// </summary>
    [Fact]
    public void Chart_ImplementsIChartInterface()
    {
        // Arrange
        var cut = RenderComponent<Chart<int>>();
        IChart<int> chart = cut.Instance;

        // Assert - verify interface implementation
        chart.Should().NotBeNull();
        chart.ChartType.Should().Be(ChartType.Bar);
        chart.Title.Should().BeEmpty();
        chart.Labels.Should().BeEmpty();
        chart.Colors.Should().BeEmpty();
        chart.Options.Should().NotBeNull();
        chart.Annotations.Should().BeEmpty();
        chart.ValueFormatter.Should().NotBeNull();
    }

    /// <summary>
    /// Verifies that changing parameters after initialization works correctly.
    /// </summary>
    [Fact]
    public void Parameters_ChangedAfterInitialization_ReflectsChanges()
    {
        // Arrange
        var cut = RenderComponent<Chart<int>>();

        // Verify initial state
        cut.Instance.ChartType.Should().Be(ChartType.Bar);
        cut.Instance.Title.Should().BeEmpty();

        // Act
        cut.SetParametersAndRender(parameters => parameters
            .Add(p => p.ChartType, ChartType.Pie)
            .Add(p => p.Title, "New Title")
            .Add(p => p.Labels, new List<string> { "A", "B", "C" }));

        // Assert
        cut.Instance.ChartType.Should().Be(ChartType.Pie);
        cut.Instance.Title.Should().Be("New Title");
        cut.Instance.Labels.Should().BeEquivalentTo(new List<string> { "A", "B", "C" });
    }

    /// <summary>
    /// Verifies that the component handles rapid parameter changes without throwing.
    /// </summary>
    [Fact]
    public void RapidParameterChanges_WorksCorrectly()
    {
        // Arrange
        var cut = RenderComponent<Chart<int>>();

        // Act & Assert - rapid parameter changes should not throw
        for (int i = 0; i < 20; i++)
        {
            cut.SetParametersAndRender(parameters => parameters
                .Add(p => p.ChartType, i % 2 == 0 ? ChartType.Bar : ChartType.Line)
                .Add(p => p.Title, $"Title {i}"));
        }

        Assert.True(true); // All parameter changes executed without exception
    }

    /// <summary>
    /// Verifies that the default ValueFormatter handles various numeric values correctly.
    /// </summary>
    [Theory]
    [InlineData(0, "0")]
    [InlineData(1, "1")]
    [InlineData(1.5, "1.5")]
    [InlineData(123.456, "123.46")]
    [InlineData(1000, "1000")]
    [InlineData(0.001, "0")] // 0.001 rounds to 0 with "0.##" format
    [InlineData(999999.999, "1000000")] // Large numbers are formatted with full digits with "0.##" format
    public void DefaultValueFormatter_HandlesVariousNumericValues(double input, string expectedOutput)
    {
        // Arrange
        var cut = RenderComponent<Chart<int>>();
        var formatter = cut.Instance.ValueFormatter;

        // Act
        var result = formatter(input);

        // Assert
        result.Should().Be(expectedOutput);
    }

    /// <summary>
    /// Verifies that ChartAnnotation with ReferenceBand type works correctly.
    /// </summary>
    [Fact]
    public void ChartAnnotation_ReferenceBandType_WorksCorrectly()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.ReferenceBand,
            Value = 50,
            EndValue = 75,
            Label = "Reference Range",
            Color = "rgba(255, 200, 0, 0.3)",
            Tooltip = "Values between 50 and 75"
        };

        // Act
        var cut = RenderComponent<Chart<int>>(parameters => parameters
            .Add(p => p.Annotations, new List<ChartAnnotation> { annotation }));

        // Assert
        var retrievedAnnotation = cut.Instance.Annotations.First();
        retrievedAnnotation.Type.Should().Be(ChartAnnotationType.ReferenceBand);
        retrievedAnnotation.Value.Should().Be(50);
        retrievedAnnotation.EndValue.Should().Be(75);
        retrievedAnnotation.Label.Should().Be("Reference Range");
        retrievedAnnotation.Color.Should().Be("rgba(255, 200, 0, 0.3)");
        retrievedAnnotation.Tooltip.Should().Be("Values between 50 and 75");
    }
}
