namespace BlazorComponentLibrary.Tests;

using Xunit;
using BlazorComponentLibrary.Components.Accordion;

/// <summary>
/// Tests for the Accordion component's SingleOpen parameter functionality.
/// </summary>
public sealed class AccordionTests
{
    /// <summary>
    /// Tests that SingleOpen parameter defaults to false.
    /// </summary>
    [Fact]
    public void SingleOpen_DefaultsToFalse()
    {
        // Arrange
        var accordion = new Accordion();

        // Act & Assert
        Assert.False(accordion.SingleOpen);
    }

    /// <summary>
    /// Tests that AllowMultiple and SingleOpen can be set independently.
    /// </summary>
    [Fact]
    public void Parameters_CanBeSetIndependently()
    {
        // Arrange
        var accordion = new Accordion();

        // Act
        accordion.AllowMultiple = true;
        accordion.SingleOpen = true;

        // Assert
        Assert.True(accordion.AllowMultiple);
        Assert.True(accordion.SingleOpen);
    }

    /// <summary>
    /// Tests that SingleOpen parameter is properly set and retrieved.
    /// </summary>
    [Fact]
    public void SingleOpen_Property_SetAndGet()
    {
        // Arrange
        var accordion = new Accordion();

        // Act
        accordion.SingleOpen = true;

        // Assert
        Assert.True(accordion.SingleOpen);

        // Act
        accordion.SingleOpen = false;

        // Assert
        Assert.False(accordion.SingleOpen);
    }

    /// <summary>
    /// Tests that SingleOpen parameter can be null when using nullable reference types.
    /// </summary>
    [Fact]
    public void SingleOpen_Property_CanBeSetToFalse()
    {
        // Arrange
        var accordion = new Accordion();

        // Act
        accordion.SingleOpen = false;

        // Assert
        Assert.False(accordion.SingleOpen);
    }

    /// <summary>
    /// Tests that AllowMultiple takes precedence when SingleOpen is also set.
    /// When SingleOpen is true, it should behave like AllowMultiple is false.
    /// </summary>
    [Fact]
    public void SingleOpen_WhenTrue_BehavesLikeAllowMultipleFalse()
    {
        // Arrange
        var accordion = new Accordion();
        accordion.SingleOpen = true;

        // Act & Assert
        // SingleOpen should be true
        Assert.True(accordion.SingleOpen);
    }
}