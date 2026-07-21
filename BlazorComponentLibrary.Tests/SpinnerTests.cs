using Bunit;
using Xunit;
using BlazorComponentLibrary.Components.Spinner;

namespace BlazorComponentLibrary.Tests;

/// <summary>
/// Tests for the <see cref="Spinner"/> component.
/// </summary>
public sealed class SpinnerTests : TestContext
{
    /// <summary>
    /// Verifies that the component renders with the default values.
    /// </summary>
    [Fact]
    public void DefaultRender_HasMediumSizeAndCurrentColor()
    {
        // Arrange & Act
        var cut = RenderComponent<Spinner>();

        // Assert
        var div = cut.Find("div.spinner");
        Assert.Contains("medium", div.ClassList);
        Assert.DoesNotContain("small", div.ClassList);
        Assert.DoesNotContain("large", div.ClassList);
        Assert.Equal("currentColor", div.GetAttribute("style").Replace(" ", "").Replace(";", ""));
        Assert.False(div.HasAttribute("aria-label"));
    }

    /// <summary>
    /// Verifies that setting <see cref="Spinner.Size"/> to <see cref="Spinner.SpinnerSize.Small"/> renders the small class.
    /// </summary>
    [Fact]
    public void Size_Small_RendersSmallClass()
    {
        // Arrange
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Size, Spinner.SpinnerSize.Small));

        // Act
        var div = cut.Find("div.spinner");

        // Assert
        Assert.Contains("small", div.ClassList);
        Assert.DoesNotContain("medium", div.ClassList);
        Assert.DoesNotContain("large", div.ClassList);
    }

    /// <summary>
    /// Verifies that setting <see cref="Spinner.Size"/> to <see cref="Spinner.SpinnerSize.Large"/> renders the large class.
    /// </summary>
    [Fact]
    public void Size_Large_RendersLargeClass()
    {
        // Arrange
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Size, Spinner.SpinnerSize.Large));

        // Act
        var div = cut.Find("div.spinner");

        // Assert
        Assert.Contains("large", div.ClassList);
        Assert.DoesNotContain("medium", div.ClassList);
        Assert.DoesNotContain("small", div.ClassList);
    }

    /// <summary>
    /// Verifies that the <see cref="Spinner.Color"/> parameter sets the inline style correctly.
    /// </summary>
    [Fact]
    public void Color_Set_RendersCorrectStyle()
    {
        // Arrange
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Color, "red"));

        // Act
        var div = cut.Find("div.spinner");

        // Assert
        var style = div.GetAttribute("style");
        Assert.Contains("color:red", style);
    }

    /// <summary>
    /// Verifies that the <see cref="Spinner.Label"/> parameter sets the aria-label attribute.
    /// </summary>
    [Fact]
    public void Label_Set_RendersAriaLabel()
    {
        // Arrange
        var cut = RenderComponent<Spinner>(parameters => parameters
            .Add(p => p.Label, "Loading"));

        // Act
        var div = cut.Find("div.spinner");

        // Assert
        Assert.True(div.HasAttribute("aria-label"));
        Assert.Equal("Loading", div.GetAttribute("aria-label"));
    }

    /// <summary>
    /// Verifies that the default values are as documented.
    /// </summary>
    [Fact]
    public void DefaultValues_AreAsDocumented()
    {
        // Arrange
        var cut = RenderComponent<Spinner>();

        // Act
        var div = cut.Find("div.spinner");

        // Assert
        Assert.Equal(Spinner.SpinnerSize.Medium, cut.Instance.Size);
        Assert.Equal("currentColor", cut.Instance.Color);
        Assert.Null(cut.Instance.Label);
    }
}
