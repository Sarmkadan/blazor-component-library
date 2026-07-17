namespace BlazorComponentLibrary.Tests;

using Xunit;
using BlazorComponentLibrary.Services;
using Microsoft.JSInterop;
using Moq;

/// <summary>
/// Tests for the ThemeService class.
/// </summary>
public sealed class ThemeServiceTests
{
    /// <summary>
    /// Creates a new instance of the ThemeService class with a mock IJSRuntime.
    /// </summary>
    /// <returns>A new instance of the ThemeService class.</returns>
    private static ThemeService CreateService()
    {
        var jsMock = new Mock<IJSRuntime>();
        // Return null for any InvokeAsync<string?> call (used by InitializeAsync)
        jsMock
            .Setup(js => js.InvokeAsync<string?>(It.IsAny<string>(), It.IsAny<object?[]?>()))
            .Returns(new ValueTask<string?>(default(string)));

        return new ThemeService(jsMock.Object);
    }

    /// <summary>
    /// Verifies that the default theme is System.
    /// </summary>
    [Fact]
    public void DefaultTheme_IsSystem()
    {
        var service = CreateService();
        Assert.Equal(ThemeMode.System, service.CurrentTheme);
    }

    /// <summary>
    /// Verifies that setting the theme to Dark updates the current theme.
    /// </summary>
    [Fact]
    public void SetTheme_Dark_UpdatesCurrentTheme()
    {
        var service = CreateService();
        service.SetTheme(ThemeMode.Dark);
        Assert.Equal(ThemeMode.Dark, service.CurrentTheme);
    }

    /// <summary>
    /// Verifies that setting the theme to Light updates the current theme.
    /// </summary>
    [Fact]
    public void SetTheme_Light_UpdatesCurrentTheme()
    {
        var service = CreateService();
        service.SetTheme(ThemeMode.Light);
        Assert.Equal(ThemeMode.Light, service.CurrentTheme);
    }

    /// <summary>
    /// Verifies that setting the theme raises the ThemeChanged event with the correct mode.
    /// </summary>
    [Fact]
    public void SetTheme_RaisesThemeChangedEvent_WithCorrectMode()
    {
        var service = CreateService();
        ThemeMode? raised = null;
        service.ThemeChanged += mode => raised = mode;

        service.SetTheme(ThemeMode.Dark);

        Assert.Equal(ThemeMode.Dark, raised);
    }

    /// <summary>
    /// Verifies that setting the theme multiple times always reflects the last value.
    /// </summary>
    [Fact]
    public void SetTheme_CalledMultipleTimes_AlwaysReflectsLastValue()
    {
        var service = CreateService();
        service.SetTheme(ThemeMode.Dark);
        service.SetTheme(ThemeMode.Light);
        service.SetTheme(ThemeMode.System);

        Assert.Equal(ThemeMode.System, service.CurrentTheme);
    }

    /// <summary>
    /// Verifies that creating a new instance of the ThemeService class with a null IJSRuntime throws an ArgumentNullException.
    /// </summary>
    [Fact]
    public void Constructor_NullJsRuntime_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ThemeService(null!));
    }
}
