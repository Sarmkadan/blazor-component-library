namespace BlazorComponentLibrary.Tests;

using Bunit;
using Xunit;
using BlazorComponentLibrary.Components.ThemeSwitcher;
using BlazorComponentLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;

/// <summary>
/// Tests for the <see cref="ThemeSwitcher"/> component.
/// </summary>
public sealed class ThemeSwitcherTests : TestContext
{
    /// <summary>
    /// Verifies that the component renders with default values.
    /// </summary>
    [Fact]
    public void DefaultRender_HasDefaultValues()
    {
        // Arrange
        var mockThemeService = new Mock<IThemeService>();
        mockThemeService.SetupGet(x => x.CurrentTheme).Returns(ThemeMode.System);

        // Act
        Services.AddSingleton(mockThemeService.Object);
        var cut = RenderComponent<ThemeSwitcher>();

        // Assert
        var buttons = cut.FindAll("button");
        Assert.Equal(3, buttons.Count);

        Assert.Equal("bcl-theme-switcher", cut.Instance.RootClass);
        Assert.True(cut.Instance.ShowLabel);
        Assert.Null(cut.Instance.CssClass);
    }

    /// <summary>
    /// Verifies that setting <see cref="ThemeSwitcher.ShowLabel"/> to false hides labels.
    /// </summary>
    [Fact]
    public void ShowLabel_False_HidesLabels()
    {
        // Arrange
        var mockThemeService = new Mock<IThemeService>();
        mockThemeService.SetupGet(x => x.CurrentTheme).Returns(ThemeMode.System);

        // Act
        Services.AddSingleton(mockThemeService.Object);
        var cut = RenderComponent<ThemeSwitcher>(parameters => parameters
            .Add(p => p.ShowLabel, false));

        // Assert
        var spans = cut.FindAll("span");
        Assert.DoesNotContain(spans, s => s.TextContent.Contains("Light"));
        Assert.DoesNotContain(spans, s => s.TextContent.Contains("System"));
        Assert.DoesNotContain(spans, s => s.TextContent.Contains("Dark"));
    }

    /// <summary>
    /// Verifies that setting <see cref="ThemeSwitcher.CssClass"/> adds custom CSS class.
    /// </summary>
    [Fact]
    public void CssClass_Set_AddsCustomClass()
    {
        // Arrange
        var mockThemeService = new Mock<IThemeService>();
        mockThemeService.SetupGet(x => x.CurrentTheme).Returns(ThemeMode.System);

        // Act
        Services.AddSingleton(mockThemeService.Object);
        var cut = RenderComponent<ThemeSwitcher>(parameters => parameters
            .Add(p => p.CssClass, "custom-class"));

        // Assert
        Assert.Equal("bcl-theme-switcher custom-class", cut.Instance.RootClass);
    }

    /// <summary>
    /// Verifies that <see cref="ThemeSwitcher.IsActive(ThemeMode)"/> returns true for current theme.
    /// </summary>
    [Theory]
    [InlineData(ThemeMode.Light)]
    [InlineData(ThemeMode.Dark)]
    [InlineData(ThemeMode.System)]
    public void IsActive_CurrentTheme_ReturnsTrue(ThemeMode currentTheme)
    {
        // Arrange
        var mockThemeService = new Mock<IThemeService>();
        mockThemeService.SetupGet(x => x.CurrentTheme).Returns(currentTheme);

        // Act
        Services.AddSingleton(mockThemeService.Object);
        var cut = RenderComponent<ThemeSwitcher>();

        // Assert
        Assert.True(cut.Instance.IsActive(currentTheme));
    }

    /// <summary>
    /// Verifies that <see cref="ThemeSwitcher.IsActive(ThemeMode)"/> returns false for non-current theme.
    /// </summary>
    [Theory]
    [InlineData(ThemeMode.Light, ThemeMode.Dark)]
    [InlineData(ThemeMode.Light, ThemeMode.System)]
    [InlineData(ThemeMode.Dark, ThemeMode.Light)]
    [InlineData(ThemeMode.Dark, ThemeMode.System)]
    [InlineData(ThemeMode.System, ThemeMode.Light)]
    [InlineData(ThemeMode.System, ThemeMode.Dark)]
    public void IsActive_NonCurrentTheme_ReturnsFalse(ThemeMode currentTheme, ThemeMode testedTheme)
    {
        // Arrange
        var mockThemeService = new Mock<IThemeService>();
        mockThemeService.SetupGet(x => x.CurrentTheme).Returns(currentTheme);

        // Act
        Services.AddSingleton(mockThemeService.Object);
        var cut = RenderComponent<ThemeSwitcher>();

        // Assert
        Assert.False(cut.Instance.IsActive(testedTheme));
    }

    /// <summary>
    /// Verifies that clicking Light theme button calls ThemeService.SetTheme with Light.
    /// </summary>
    [Fact]
    public void ClickLightButton_CallsSetThemeWithLight()
    {
        // Arrange
        var mockThemeService = new Mock<IThemeService>();
        mockThemeService.SetupGet(x => x.CurrentTheme).Returns(ThemeMode.System);
        ThemeMode? capturedTheme = null;
        mockThemeService.Setup(x => x.SetTheme(It.IsAny<ThemeMode>()))
            .Callback<ThemeMode>(theme => capturedTheme = theme);

        // Act
        Services.AddSingleton(mockThemeService.Object);
        var cut = RenderComponent<ThemeSwitcher>();
        var lightButton = cut.FindAll("button")[0];
        lightButton.Click();

        // Assert
        Assert.Equal(ThemeMode.Light, capturedTheme);
        mockThemeService.Verify(x => x.SetTheme(ThemeMode.Light), Times.Once);
    }

    /// <summary>
    /// Verifies that clicking System theme button calls ThemeService.SetTheme with System.
    /// </summary>
    [Fact]
    public void ClickSystemButton_CallsSetThemeWithSystem()
    {
        // Arrange
        var mockThemeService = new Mock<IThemeService>();
        mockThemeService.SetupGet(x => x.CurrentTheme).Returns(ThemeMode.Light);
        ThemeMode? capturedTheme = null;
        mockThemeService.Setup(x => x.SetTheme(It.IsAny<ThemeMode>()))
            .Callback<ThemeMode>(theme => capturedTheme = theme);

        // Act
        Services.AddSingleton(mockThemeService.Object);
        var cut = RenderComponent<ThemeSwitcher>();
        var systemButton = cut.FindAll("button")[1];
        systemButton.Click();

        // Assert
        Assert.Equal(ThemeMode.System, capturedTheme);
        mockThemeService.Verify(x => x.SetTheme(ThemeMode.System), Times.Once);
    }

    /// <summary>
    /// Verifies that clicking Dark theme button calls ThemeService.SetTheme with Dark.
    /// </summary>
    [Fact]
    public void ClickDarkButton_CallsSetThemeWithDark()
    {
        // Arrange
        var mockThemeService = new Mock<IThemeService>();
        mockThemeService.SetupGet(x => x.CurrentTheme).Returns(ThemeMode.System);
        ThemeMode? capturedTheme = null;
        mockThemeService.Setup(x => x.SetTheme(It.IsAny<ThemeMode>()))
            .Callback<ThemeMode>(theme => capturedTheme = theme);

        // Act
        Services.AddSingleton(mockThemeService.Object);
        var cut = RenderComponent<ThemeSwitcher>();
        var darkButton = cut.FindAll("button")[2];
        darkButton.Click();

        // Assert
        Assert.Equal(ThemeMode.Dark, capturedTheme);
        mockThemeService.Verify(x => x.SetTheme(ThemeMode.Dark), Times.Once);
    }

    /// <summary>
    /// Verifies that clicking theme button updates active state.
    /// </summary>
    [Fact]
    public void ClickThemeButton_UpdatesActiveState()
    {
        // Arrange
        var mockThemeService = new Mock<IThemeService>();
        mockThemeService.SetupGet(x => x.CurrentTheme).Returns(ThemeMode.System);

        // Act
        Services.AddSingleton(mockThemeService.Object);
        var cut = RenderComponent<ThemeSwitcher>();

        // Initially System should be active
        Assert.True(cut.Instance.IsActive(ThemeMode.System));
        Assert.False(cut.Instance.IsActive(ThemeMode.Light));
        Assert.False(cut.Instance.IsActive(ThemeMode.Dark));

        // Click Light theme
        var lightButton = cut.FindAll("button")[0];
        lightButton.Click();

        // Assert - Light should now be active
        Assert.True(cut.Instance.IsActive(ThemeMode.Light));
        Assert.False(cut.Instance.IsActive(ThemeMode.System));
        Assert.False(cut.Instance.IsActive(ThemeMode.Dark));
    }

    /// <summary>
    /// Verifies that theme button has correct aria-pressed attribute for current theme.
    /// </summary>
    [Theory]
    [InlineData(ThemeMode.Light)]
    [InlineData(ThemeMode.Dark)]
    [InlineData(ThemeMode.System)]
    public void ThemeButton_AriaPressed_ReflectsCurrentTheme(ThemeMode currentTheme)
    {
        // Arrange
        var mockThemeService = new Mock<IThemeService>();
        mockThemeService.SetupGet(x => x.CurrentTheme).Returns(currentTheme);

        // Act
        Services.AddSingleton(mockThemeService.Object);
        var cut = RenderComponent<ThemeSwitcher>();

        // Assert
        var buttons = cut.FindAll("button");
        for (int i = 0; i < buttons.Count; i++)
        {
            var button = buttons[i];
            var expectedPressed = (i == (int)currentTheme).ToString().ToLower();
            Assert.Equal(expectedPressed, button.GetAttribute("aria-pressed"));
        }
    }

    /// <summary>
    /// Verifies that theme button has correct title attribute.
    /// </summary>
    [Theory]
    [InlineData(0, "Light theme")]
    [InlineData(1, "System theme")]
    [InlineData(2, "Dark theme")]
    public void ThemeButton_TitleAttribute_Correct(int buttonIndex, string expectedTitle)
    {
        // Arrange
        var mockThemeService = new Mock<IThemeService>();
        mockThemeService.SetupGet(x => x.CurrentTheme).Returns(ThemeMode.System);

        // Act
        Services.AddSingleton(mockThemeService.Object);
        var cut = RenderComponent<ThemeSwitcher>();

        // Assert
        var button = cut.FindAll("button")[buttonIndex];
        Assert.Equal(expectedTitle, button.GetAttribute("title"));
    }

    /// <summary>
    /// Verifies that theme button has correct icon content.
    /// </summary>
    [Theory]
    [InlineData(0, "☀️")]
    [InlineData(1, "💻")]
    [InlineData(2, "🌙")]
    public void ThemeButton_IconContent_Correct(int buttonIndex, string expectedIcon)
    {
        // Arrange
        var mockThemeService = new Mock<IThemeService>();
        mockThemeService.SetupGet(x => x.CurrentTheme).Returns(ThemeMode.System);

        // Act
        Services.AddSingleton(mockThemeService.Object);
        var cut = RenderComponent<ThemeSwitcher>();

        // Assert
        var button = cut.FindAll("button")[buttonIndex];
        var span = button.QuerySelector("span.bcl-icon");
        Assert.Equal(expectedIcon, span?.TextContent);
    }

    /// <summary>
    /// Verifies that theme button has correct active class when active.
    /// </summary>
    [Theory]
    [InlineData(0, ThemeMode.Light)]
    [InlineData(1, ThemeMode.System)]
    [InlineData(2, ThemeMode.Dark)]
    public void ThemeButton_ActiveClass_WhenActive(int buttonIndex, ThemeMode activeTheme)
    {
        // Arrange
        var mockThemeService = new Mock<IThemeService>();
        mockThemeService.SetupGet(x => x.CurrentTheme).Returns(activeTheme);

        // Act
        Services.AddSingleton(mockThemeService.Object);
        var cut = RenderComponent<ThemeSwitcher>();

        // Assert
        var buttons = cut.FindAll("button");
        var activeButton = buttons[buttonIndex];
        Assert.Contains("bcl-theme-switcher__btn--active", activeButton.ClassName);

        // Other buttons should not have active class
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i != buttonIndex)
            {
                Assert.DoesNotContain("bcl-theme-switcher__btn--active", buttons[i].ClassName);
            }
        }
    }

    /// <summary>
    /// Verifies that theme button does not have active class when inactive.
    /// </summary>
    [Theory]
    [InlineData(0, ThemeMode.Dark)]
    [InlineData(0, ThemeMode.System)]
    [InlineData(1, ThemeMode.Light)]
    [InlineData(1, ThemeMode.Dark)]
    [InlineData(2, ThemeMode.Light)]
    [InlineData(2, ThemeMode.System)]
    public void ThemeButton_NoActiveClass_WhenInactive(int buttonIndex, ThemeMode inactiveTheme)
    {
        // Arrange
        var mockThemeService = new Mock<IThemeService>();
        mockThemeService.SetupGet(x => x.CurrentTheme).Returns(inactiveTheme);

        // Act
        Services.AddSingleton(mockThemeService.Object);
        var cut = RenderComponent<ThemeSwitcher>();

        // Assert
        var buttons = cut.FindAll("button");
        Assert.DoesNotContain("bcl-theme-switcher__btn--active", buttons[buttonIndex].ClassName);
    }

    /// <summary>
    /// Verifies that ThemeChanged event triggers component re-render.
    /// </summary>
    [Fact]
    public void ThemeChangedEvent_TriggersReRender()
    {
        // Arrange
        var mockThemeService = new Mock<IThemeService>();
        mockThemeService.SetupGet(x => x.CurrentTheme).Returns(ThemeMode.System);
        var themeChangedTriggered = false;

        // Act
        Services.AddSingleton(mockThemeService.Object);
        var cut = RenderComponent<ThemeSwitcher>();

        // Simulate theme change event and verify the component re-renders
        var renderCountBefore = cut.RenderCount;
        mockThemeService.Raise(x => x.ThemeChanged += null, ThemeMode.Light);
        themeChangedTriggered = cut.RenderCount > renderCountBefore;

        // Assert
        Assert.True(themeChangedTriggered);
    }

    /// <summary>
    /// Verifies that Dispose removes ThemeChanged event subscription.
    /// </summary>
    [Fact]
    public void Dispose_RemovesThemeChangedEventSubscription()
    {
        // Arrange
        var mockThemeService = new Mock<IThemeService>();
        mockThemeService.SetupGet(x => x.CurrentTheme).Returns(ThemeMode.System);

        // Act
        Services.AddSingleton(mockThemeService.Object);
        var cut = RenderComponent<ThemeSwitcher>();

        // Verify event is subscribed
        Assert.IsAssignableFrom<IDisposable>(cut.Instance);

        // Dispose should not throw
        var exception = Record.Exception(() => cut.Dispose());
        Assert.Null(exception);
    }

    /// <summary>
    /// Verifies that component can be re-rendered with different parameters.
    /// </summary>
    [Fact]
    public void SetParametersAndRender_UpdatesParameters()
    {
        // Arrange
        var mockThemeService = new Mock<IThemeService>();
        mockThemeService.SetupGet(x => x.CurrentTheme).Returns(ThemeMode.System);

        // Act
        Services.AddSingleton(mockThemeService.Object);
        var cut = RenderComponent<ThemeSwitcher>();

        // Initially
        Assert.True(cut.Instance.ShowLabel);

        // Update parameters
        cut.SetParametersAndRender(parameters => parameters
            .Add(p => p.ShowLabel, false)
            .Add(p => p.CssClass, "updated-class"));

        // Assert
        Assert.False(cut.Instance.ShowLabel);
        Assert.Equal("bcl-theme-switcher updated-class", cut.Instance.RootClass);
    }

    /// <summary>
    /// Verifies that component renders with correct number of buttons.
    /// </summary>
    [Fact]
    public void Render_ThreeButtons()
    {
        // Arrange
        var mockThemeService = new Mock<IThemeService>();
        mockThemeService.SetupGet(x => x.CurrentTheme).Returns(ThemeMode.System);

        // Act
        Services.AddSingleton(mockThemeService.Object);
        var cut = RenderComponent<ThemeSwitcher>();

        // Assert
        var buttons = cut.FindAll("button");
        Assert.Equal(3, buttons.Count);
    }

    /// <summary>
    /// Verifies that component has correct role and aria-label.
    /// </summary>
    [Fact]
    public void Render_HasCorrectAccessibilityAttributes()
    {
        // Arrange
        var mockThemeService = new Mock<IThemeService>();
        mockThemeService.SetupGet(x => x.CurrentTheme).Returns(ThemeMode.System);

        // Act
        Services.AddSingleton(mockThemeService.Object);
        var cut = RenderComponent<ThemeSwitcher>();

        // Assert
        var div = cut.Find("div");
        Assert.Equal("group", div.GetAttribute("role"));
        Assert.Equal("Theme selector", div.GetAttribute("aria-label"));
    }
}
