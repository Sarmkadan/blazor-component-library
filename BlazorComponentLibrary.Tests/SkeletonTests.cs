using Bunit;
using Xunit;
using BlazorComponentLibrary.Components.Skeleton;
using FluentAssertions;

namespace BlazorComponentLibrary.Tests;

/// <summary>
/// Tests for the <see cref="Skeleton"/> component.
/// </summary>
public sealed class SkeletonTests : TestContext
{
    /// <summary>
    /// Verifies that the component renders with default values.
    /// </summary>
    [Fact]
    public void DefaultRender_HasCorrectDefaultValues()
    {
        // Arrange & Act
        var cut = RenderComponent<Skeleton>();

        // Assert
        var div = cut.Find("div.bcl-skeleton");

        // Default values from component
        Assert.Equal(SkeletonType.Text, cut.Instance.Type);
        Assert.Equal(SkeletonShape.Rect, cut.Instance.Shape);
        Assert.Equal("100%", cut.Instance.Width);
        Assert.Equal("auto", cut.Instance.Height);
        Assert.Equal(3, cut.Instance.Lines);
        Assert.True(cut.Instance.Animated);

        // CSS classes
        Assert.Contains("bcl-skeleton", div.ClassList);
        Assert.Contains("bcl-skeleton--animated", div.ClassList);
        Assert.Contains("bcl-skeleton--text", div.ClassList);

        // Attributes
        Assert.Equal("Loading…", div.GetAttribute("aria-label"));
        Assert.Contains("width: 100%", div.GetAttribute("style"));
    }

    /// <summary>
    /// Verifies that setting Type to Circle renders correctly.
    /// </summary>
    [Fact]
    public void Type_Circle_RendersCircleShape()
    {
        // Arrange & Act
        var cut = RenderComponent<Skeleton>(parameters => parameters
            .Add(p => p.Type, SkeletonType.Circle));

        // Assert
        var div = cut.Find("div.bcl-skeleton");
        Assert.Contains("bcl-skeleton--circle", div.ClassList);
        Assert.DoesNotContain("bcl-skeleton--text", div.ClassList);
        Assert.DoesNotContain("bcl-skeleton--rectangle", div.ClassList);
    }

    /// <summary>
    /// Verifies that setting Type to Rectangle renders correctly.
    /// </summary>
    [Fact]
    public void Type_Rectangle_RendersRectangleShape()
    {
        // Arrange & Act
        var cut = RenderComponent<Skeleton>(parameters => parameters
            .Add(p => p.Type, SkeletonType.Rectangle));

        // Assert
        var div = cut.Find("div.bcl-skeleton");
        Assert.Contains("bcl-skeleton--rectangle", div.ClassList);
        Assert.DoesNotContain("bcl-skeleton--text", div.ClassList);
        Assert.DoesNotContain("bcl-skeleton--circle", div.ClassList);
    }

    /// <summary>
    /// Verifies that Shape parameter maps to correct CSS classes.
    /// </summary>
    [Theory]
    [InlineData(SkeletonShape.Text, "bcl-skeleton--text")]
    [InlineData(SkeletonShape.Circle, "bcl-skeleton--circle")]
    [InlineData(SkeletonShape.Rect, "bcl-skeleton--rectangle")]
    [InlineData(SkeletonShape.Card, "bcl-skeleton--card")]
    public void Shape_Parameter_MapsToCorrectClass(SkeletonShape shape, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderComponent<Skeleton>(parameters => parameters
            .Add(p => p.Shape, shape));

        // Assert
        var div = cut.Find("div.bcl-skeleton");
        Assert.Contains(expectedClass, div.ClassList);
    }

    /// <summary>
    /// Verifies that Width parameter is applied correctly.
    /// </summary>
    [Theory]
    [InlineData("100%")]
    [InlineData("50%")]
    [InlineData("200px")]
    [InlineData("10rem")]
    public void Width_Parameter_IsAppliedCorrectly(string width)
    {
        // Arrange & Act
        var cut = RenderComponent<Skeleton>(parameters => parameters
            .Add(p => p.Width, width));

        // Assert
        var div = cut.Find("div.bcl-skeleton");
        var style = div.GetAttribute("style");
        Assert.Contains($"width: {width}", style);
    }

    /// <summary>
    /// Verifies that Height parameter is applied correctly for Circle type.
    /// </summary>
    [Theory]
    [InlineData("auto", "100%")] // When Height is auto, it should equal Width for Circle
    [InlineData("80px", "80px")]
    [InlineData("5rem", "5rem")]
    public void Height_Parameter_IsAppliedCorrectly_ForCircle(string height, string expectedHeight)
    {
        // Arrange & Act
        var cut = RenderComponent<Skeleton>(parameters => parameters
            .Add(p => p.Type, SkeletonType.Circle)
            .Add(p => p.Width, "100%")
            .Add(p => p.Height, height));

        // Assert
        var div = cut.Find("div.bcl-skeleton");
        var style = div.GetAttribute("style");
        Assert.Contains($"height: {expectedHeight}", style);
    }

    /// <summary>
    /// Verifies that Height parameter is applied correctly for Rectangle type.
    /// </summary>
    [Theory]
    [InlineData("auto", "1.5rem")] // When Height is auto, default should be 1.5rem for Rectangle
    [InlineData("80px", "80px")]
    [InlineData("5rem", "5rem")]
    public void Height_Parameter_IsAppliedCorrectly_ForRectangle(string height, string expectedHeight)
    {
        // Arrange & Act
        var cut = RenderComponent<Skeleton>(parameters => parameters
            .Add(p => p.Type, SkeletonType.Rectangle)
            .Add(p => p.Width, "100%")
            .Add(p => p.Height, height));

        // Assert
        var div = cut.Find("div.bcl-skeleton");
        var style = div.GetAttribute("style");
        Assert.Contains($"height: {expectedHeight}", style);
    }

    /// <summary>
    /// Verifies that Lines parameter controls the number of text lines rendered.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Lines_Parameter_ControlsNumberOfTextLines(int lines)
    {
        // Arrange & Act
        var cut = RenderComponent<Skeleton>(parameters => parameters
            .Add(p => p.Type, SkeletonType.Text)
            .Add(p => p.Lines, lines));

        // Assert
        var linesElements = cut.FindAll("div.bcl-skeleton__line");
        Assert.Equal(lines, linesElements.Count);

        // Last line should have the short modifier
        var lastLine = linesElements[^1];
        Assert.Contains("bcl-skeleton__line--short", lastLine.ClassList);
    }

    /// <summary>
    /// Verifies that Animated parameter controls the CSS animation class.
    /// </summary>
    [Theory]
    [InlineData(true, "bcl-skeleton--animated")]
    [InlineData(false, "bcl-skeleton")]
    public void Animated_Parameter_ControlsAnimationClass(bool animated, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderComponent<Skeleton>(parameters => parameters
            .Add(p => p.Animated, animated));

        // Assert
        var div = cut.Find("div.bcl-skeleton");
        if (animated)
        {
            Assert.Contains(expectedClass, div.ClassList);
        }
        else
        {
            Assert.DoesNotContain("bcl-skeleton--animated", div.ClassList);
        }
    }

    /// <summary>
    /// Verifies that Text type renders multiple line elements.
    /// </summary>
    [Fact]
    public void TextType_RendersMultipleLineElements()
    {
        // Arrange & Act
        var cut = RenderComponent<Skeleton>(parameters => parameters
            .Add(p => p.Type, SkeletonType.Text)
            .Add(p => p.Lines, 4));

        // Assert
        var lines = cut.FindAll("div.bcl-skeleton__line");
        Assert.Equal(4, lines.Count);

        // Verify line classes
        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            Assert.Contains("bcl-skeleton__line", line.ClassList);
        }
    }

    /// <summary>
    /// Verifies that Circle type renders a single div with width and height.
    /// </summary>
    [Fact]
    public void CircleType_RendersSingleDivWithWidthAndHeight()
    {
        // Arrange & Act
        var cut = RenderComponent<Skeleton>(parameters => parameters
            .Add(p => p.Type, SkeletonType.Circle)
            .Add(p => p.Width, "100px")
            .Add(p => p.Height, "100px"));

        // Assert
        var divs = cut.FindAll("div.bcl-skeleton");
        Assert.Single(divs);

        var div = divs[0];
        var styleParts = div.GetAttribute("style")?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        Assert.Contains("width: 100px", styleParts);
        Assert.Contains("height: 100px", styleParts);
    }

    /// <summary>
    /// Verifies that Rectangle type renders a single div with default height when Height is auto.
    /// </summary>
    [Fact]
    public void RectangleType_RendersSingleDivWithDefaultHeight()
    {
        // Arrange & Act
        var cut = RenderComponent<Skeleton>(parameters => parameters
            .Add(p => p.Type, SkeletonType.Rectangle));

        // Assert
        var divs = cut.FindAll("div.bcl-skeleton");
        Assert.Single(divs);

        var div = divs[0];
        var style = div.GetAttribute("style");
        Assert.Contains("height: 1.5rem", style);
    }

    /// <summary>
    /// Verifies that aria-label attribute is always present with correct value.
    /// </summary>
    [Fact]
    public void AriaLabel_IsAlwaysPresentWithLoadingText()
    {
        // Arrange & Act
        var cut = RenderComponent<Skeleton>();

        // Assert
        var divs = cut.FindAll("div[aria-label]");
        Assert.NotEmpty(divs);
        Assert.All(divs, div => Assert.Equal("Loading…", div.GetAttribute("aria-label")));
    }

    /// <summary>
    /// Verifies that role="status" is present for accessibility.
    /// </summary>
    [Fact]
    public void Role_Status_IsPresentForAccessibility()
    {
        // Arrange & Act
        var cut = RenderComponent<Skeleton>();

        // Assert
        var divs = cut.FindAll("div[role=status]");
        Assert.NotEmpty(divs);
    }

    /// <summary>
    /// Verifies that default values match component documentation.
    /// </summary>
    [Fact]
    public void DefaultValues_AreAsDocumented()
    {
        // Arrange
        var cut = RenderComponent<Skeleton>();

        // Act
        var instance = cut.Instance;

        // Assert
        instance.Type.Should().Be(SkeletonType.Text);
        instance.Shape.Should().Be(SkeletonShape.Rect);
        instance.Width.Should().Be("100%");
        instance.Height.Should().Be("auto");
        instance.Lines.Should().Be(3);
        instance.Animated.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that all shape types render without errors.
    /// </summary>
    [Fact]
    public void AllShapeTypes_RenderWithoutErrors()
    {
        // Test all shape values
        foreach (SkeletonShape shape in Enum.GetValues(typeof(SkeletonShape)))
        {
            // Arrange & Act
            var cut = RenderComponent<Skeleton>(parameters => parameters
                .Add(p => p.Shape, shape));

            // Assert - should render without throwing
            var div = cut.Find("div.bcl-skeleton");
            Assert.NotNull(div);
        }
    }
}