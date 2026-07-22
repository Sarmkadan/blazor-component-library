namespace BlazorComponentLibrary.Tests;

using Xunit;
using BlazorComponentLibrary.Components.Chart;
using FluentAssertions;
using System;

/// <summary>
/// Comprehensive unit tests for <see cref="ChartAnnotationExtensions"/> extension methods.
/// Tests cover happy-path scenarios, edge cases (null/empty inputs, boundary values),
/// and error-path assertions including empty series scenarios.
/// </summary>
public sealed class ChartAnnotationExtensionsTests
{
    /// <summary>
    /// Verifies that GetDisplayText returns correct text for ThresholdLine annotation without label.
    /// </summary>
    [Fact]
    public void GetDisplayText_ThresholdLineWithoutLabel_ReturnsFormattedText()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.ThresholdLine,
            Value = 75.5,
            Label = string.Empty
        };

        // Act
        var displayText = annotation.GetDisplayText();

        // Assert
        displayText.Should().Be("Threshold: 75.5");
    }

    /// <summary>
    /// Verifies that GetDisplayText returns correct text for ThresholdLine annotation with label.
    /// </summary>
    [Fact]
    public void GetDisplayText_ThresholdLineWithLabel_ReturnsLabel()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.ThresholdLine,
            Value = 75.5,
            Label = "Warning Threshold"
        };

        // Act
        var displayText = annotation.GetDisplayText();

        // Assert
        displayText.Should().Be("Warning Threshold");
    }

    /// <summary>
    /// Verifies that GetDisplayText returns correct text for EventMarker annotation without label.
    /// </summary>
    [Fact]
    public void GetDisplayText_EventMarkerWithoutLabel_ReturnsFormattedText()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.EventMarker,
            Value = 25.7,
            Label = string.Empty
        };

        // Act
        var displayText = annotation.GetDisplayText();

        // Assert
        displayText.Should().Be("Event at 25.7");
    }

    /// <summary>
    /// Verifies that GetDisplayText returns correct text for EventMarker annotation with label.
    /// </summary>
    [Fact]
    public void GetDisplayText_EventMarkerWithLabel_ReturnsLabel()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.EventMarker,
            Value = 25.7,
            Label = "Important Event"
        };

        // Act
        var displayText = annotation.GetDisplayText();

        // Assert
        displayText.Should().Be("Important Event");
    }

    /// <summary>
    /// Verifies that GetDisplayText returns correct text for ReferenceBand annotation without label.
    /// </summary>
    [Fact]
    public void GetDisplayText_ReferenceBandWithoutLabel_ReturnsFormattedText()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.ReferenceBand,
            Value = 25.0,
            EndValue = 75.0,
            Label = string.Empty
        };

        // Act
        var displayText = annotation.GetDisplayText();

        // Assert
        displayText.Should().Be("Range: 25 - 75");
    }

    /// <summary>
    /// Verifies that GetDisplayText returns correct text for ReferenceBand annotation with label.
    /// </summary>
    [Fact]
    public void GetDisplayText_ReferenceBandWithLabel_ReturnsLabel()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.ReferenceBand,
            Value = 25.0,
            EndValue = 75.0,
            Label = "Acceptable Range"
        };

        // Act
        var displayText = annotation.GetDisplayText();

        // Assert
        displayText.Should().Be("Acceptable Range");
    }

    /// <summary>
    /// Verifies that GetDisplayText returns correct text for unknown annotation type.
    /// </summary>
    [Fact]
    public void GetDisplayText_UnknownType_ReturnsTypeOrLabel()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Type = (ChartAnnotationType)999, // Unknown type
            Value = 50.0,
            Label = "Custom Type"
        };

        // Act
        var displayText = annotation.GetDisplayText();

        // Assert
        displayText.Should().Be("Custom Type");
    }

    /// <summary>
    /// Verifies that GetDisplayText throws ArgumentNullException when annotation is null.
    /// </summary>
    [Fact]
    public void GetDisplayText_NullAnnotation_ThrowsArgumentNullException()
    {
        // Arrange
        ChartAnnotation annotation = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => annotation.GetDisplayText());
    }

    /// <summary>
    /// Verifies that IsValid returns true for valid ThresholdLine annotation.
    /// </summary>
    [Fact]
    public void IsValid_ThresholdLineWithValidValue_ReturnsTrue()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.ThresholdLine,
            Value = 75.5
        };

        // Act
        var isValid = annotation.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that IsValid returns false for ThresholdLine annotation with NaN value.
    /// </summary>
    [Fact]
    public void IsValid_ThresholdLineWithNaNValue_ReturnsFalse()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.ThresholdLine,
            Value = double.NaN
        };

        // Act
        var isValid = annotation.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that IsValid returns false for ThresholdLine annotation with Infinity value.
    /// </summary>
    [Fact]
    public void IsValid_ThresholdLineWithInfinityValue_ReturnsFalse()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.ThresholdLine,
            Value = double.PositiveInfinity
        };

        // Act
        var isValid = annotation.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that IsValid returns true for valid EventMarker annotation.
    /// </summary>
    [Fact]
    public void IsValid_EventMarkerWithValidValue_ReturnsTrue()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.EventMarker,
            Value = 25.7
        };

        // Act
        var isValid = annotation.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that IsValid returns true for valid ReferenceBand annotation.
    /// </summary>
    [Fact]
    public void IsValid_ReferenceBandWithValidValues_ReturnsTrue()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.ReferenceBand,
            Value = 25.0,
            EndValue = 75.0
        };

        // Act
        var isValid = annotation.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that IsValid returns false for ReferenceBand annotation with NaN start value.
    /// </summary>
    [Fact]
    public void IsValid_ReferenceBandWithNaNStartValue_ReturnsFalse()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.ReferenceBand,
            Value = double.NaN,
            EndValue = 75.0
        };

        // Act
        var isValid = annotation.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that IsValid returns false for ReferenceBand annotation with NaN end value.
    /// </summary>
    [Fact]
    public void IsValid_ReferenceBandWithNaNEndValue_ReturnsFalse()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.ReferenceBand,
            Value = 25.0,
            EndValue = double.NaN
        };

        // Act
        var isValid = annotation.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that IsValid returns false for ReferenceBand annotation with null EndValue.
    /// </summary>
    [Fact]
    public void IsValid_ReferenceBandWithNullEndValue_ReturnsFalse()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.ReferenceBand,
            Value = 25.0,
            EndValue = null
        };

        // Act
        var isValid = annotation.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that IsValid returns false for unknown annotation type.
    /// </summary>
    [Fact]
    public void IsValid_UnknownType_ReturnsFalse()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Type = (ChartAnnotationType)999, // Unknown type
            Value = 50.0
        };

        // Act
        var isValid = annotation.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that IsValid throws ArgumentNullException when annotation is null.
    /// </summary>
    [Fact]
    public void IsValid_NullAnnotation_ThrowsArgumentNullException()
    {
        // Arrange
        ChartAnnotation annotation = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => annotation.IsValid());
    }

    /// <summary>
    /// Verifies that Clone creates a deep copy of the annotation.
    /// </summary>
    [Fact]
    public void Clone_CreatesDeepCopy()
    {
        // Arrange
        var original = new ChartAnnotation
        {
            Type = ChartAnnotationType.ThresholdLine,
            Value = 75.5,
            EndValue = null,
            Label = "Original Label",
            Color = "#ff0000",
            Tooltip = "Original tooltip"
        };

        // Act
        var clone = original.Clone();

        // Assert
        clone.Should().NotBeSameAs(original);
        clone.Type.Should().Be(original.Type);
        clone.Value.Should().Be(original.Value);
        clone.EndValue.Should().Be(original.EndValue);
        clone.Label.Should().Be(original.Label);
        clone.Color.Should().Be(original.Color);
        clone.Tooltip.Should().Be(original.Tooltip);
    }

    /// <summary>
    /// Verifies that Clone creates a deep copy that can be modified independently.
    /// </summary>
    [Fact]
    public void Clone_CanModifyWithoutAffectingOriginal()
    {
        // Arrange
        var original = new ChartAnnotation
        {
            Type = ChartAnnotationType.ThresholdLine,
            Value = 75.5,
            Label = "Original"
        };

        // Act
        var clone = original.Clone();
        clone.Label = "Modified Clone";
        clone.Value = 100.0;

        // Assert
        original.Label.Should().Be("Original");
        original.Value.Should().Be(75.5);
        clone.Label.Should().Be("Modified Clone");
        clone.Value.Should().Be(100.0);
    }

    /// <summary>
    /// Verifies that Clone throws ArgumentNullException when annotation is null.
    /// </summary>
    [Fact]
    public void Clone_NullAnnotation_ThrowsArgumentNullException()
    {
        // Arrange
        ChartAnnotation annotation = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => annotation.Clone());
    }

    /// <summary>
    /// Verifies that SetColor updates the annotation color correctly.
    /// </summary>
    [Fact]
    public void SetColor_UpdatesColorCorrectly()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Color = "#ff0000"
        };

        // Act
        annotation.SetColor("#00ff00");

        // Assert
        annotation.Color.Should().Be("#00ff00");
    }

    /// <summary>
    /// Verifies that SetColor throws ArgumentNullException when annotation is null.
    /// </summary>
    [Fact]
    public void SetColor_NullAnnotation_ThrowsArgumentNullException()
    {
        // Arrange
        ChartAnnotation annotation = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => annotation.SetColor("#ff0000"));
    }

    /// <summary>
    /// Verifies that SetColor throws ArgumentException when color is null or empty.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void SetColor_NullOrEmptyColor_ThrowsArgumentException(string? color)
    {
        // Arrange
        var annotation = new ChartAnnotation();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => annotation.SetColor(color!));
    }

    /// <summary>
    /// Verifies that SetTooltip updates the annotation tooltip correctly.
    /// </summary>
    [Fact]
    public void SetTooltip_UpdatesTooltipCorrectly()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Tooltip = "Original tooltip"
        };

        // Act
        annotation.SetTooltip("New tooltip text");

        // Assert
        annotation.Tooltip.Should().Be("New tooltip text");
    }

    /// <summary>
    /// Verifies that SetTooltip handles null tooltip by converting to empty string.
    /// </summary>
    [Fact]
    public void SetTooltip_NullTooltip_ConvertsToEmptyString()
    {
        // Arrange
        var annotation = new ChartAnnotation();

        // Act
        annotation.SetTooltip(null);

        // Assert
        annotation.Tooltip.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that SetTooltip throws ArgumentNullException when annotation is null.
    /// </summary>
    [Fact]
    public void SetTooltip_NullAnnotation_ThrowsArgumentNullException()
    {
        // Arrange
        ChartAnnotation annotation = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => annotation.SetTooltip("tooltip"));
    }

    /// <summary>
    /// Verifies that GetValueText returns correct formatted value for ThresholdLine/EventMarker.
    /// </summary>
    [Fact]
    public void GetValueText_ThresholdLineEventMarker_ReturnsFormattedValue()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.ThresholdLine,
            Value = 75.5
        };

        // Act
        var valueText = annotation.GetValueText();

        // Assert
        valueText.Should().Be("75.5");
    }

    /// <summary>
    /// Verifies that GetValueText returns correct formatted range for ReferenceBand.
    /// </summary>
    [Fact]
    public void GetValueText_ReferenceBand_ReturnsFormattedRange()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.ReferenceBand,
            Value = 25.0,
            EndValue = 75.0
        };

        // Act
        var valueText = annotation.GetValueText();

        // Assert
        valueText.Should().Be("25 - 75");
    }

    /// <summary>
    /// Verifies that GetValueText throws ArgumentNullException when annotation is null.
    /// </summary>
    [Fact]
    public void GetValueText_NullAnnotation_ThrowsArgumentNullException()
    {
        // Arrange
        ChartAnnotation annotation = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => annotation.GetValueText());
    }

    /// <summary>
    /// Verifies that HasLabel returns true when annotation has a non-empty label.
    /// </summary>
    [Fact]
    public void HasLabel_WithNonEmptyLabel_ReturnsTrue()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Label = "Important Label"
        };

        // Act
        var hasLabel = annotation.HasLabel();

        // Assert
        hasLabel.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that HasLabel returns false when annotation has an empty label.
    /// </summary>
    [Fact]
    public void HasLabel_WithEmptyLabel_ReturnsFalse()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Label = string.Empty
        };

        // Act
        var hasLabel = annotation.HasLabel();

        // Assert
        hasLabel.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that HasLabel returns false when annotation has a null label.
    /// </summary>
    [Fact]
    public void HasLabel_WithNullLabel_ReturnsFalse()
    {
        // Arrange
        var annotation = new ChartAnnotation
        {
            Label = null!
        };

        // Act
        var hasLabel = annotation.HasLabel();

        // Assert
        hasLabel.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that HasLabel throws ArgumentNullException when annotation is null.
    /// </summary>
    [Fact]
    public void HasLabel_NullAnnotation_ThrowsArgumentNullException()
    {
        // Arrange
        ChartAnnotation annotation = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => annotation.HasLabel());
    }
}
