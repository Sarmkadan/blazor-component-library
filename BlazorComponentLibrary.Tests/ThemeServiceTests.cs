namespace BlazorComponentLibrary.Tests;

using Xunit;
using BlazorComponentLibrary.Services;
using Microsoft.JSInterop;
using Moq;

public sealed class ThemeServiceTests
{
    private static ThemeService CreateService()
    {
        var jsMock = new Mock<IJSRuntime>();
        // Return null for any InvokeAsync<string?> call (used by InitializeAsync)
        jsMock
            .Setup(js => js.InvokeAsync<string?>(It.IsAny<string>(), It.IsAny<object?[]?>()))
            .Returns(new ValueTask<string?>(default(string)));

        return new ThemeService(jsMock.Object);
    }

    [Fact]
    public void DefaultTheme_IsSystem()
    {
        var service = CreateService();
        Assert.Equal(ThemeMode.System, service.CurrentTheme);
    }

    [Fact]
    public void SetTheme_Dark_UpdatesCurrentTheme()
    {
        var service = CreateService();
        service.SetTheme(ThemeMode.Dark);
        Assert.Equal(ThemeMode.Dark, service.CurrentTheme);
    }

    [Fact]
    public void SetTheme_Light_UpdatesCurrentTheme()
    {
        var service = CreateService();
        service.SetTheme(ThemeMode.Light);
        Assert.Equal(ThemeMode.Light, service.CurrentTheme);
    }

    [Fact]
    public void SetTheme_RaisesThemeChangedEvent_WithCorrectMode()
    {
        var service = CreateService();
        ThemeMode? raised = null;
        service.ThemeChanged += mode => raised = mode;

        service.SetTheme(ThemeMode.Dark);

        Assert.Equal(ThemeMode.Dark, raised);
    }

    [Fact]
    public void SetTheme_CalledMultipleTimes_AlwaysReflectsLastValue()
    {
        var service = CreateService();
        service.SetTheme(ThemeMode.Dark);
        service.SetTheme(ThemeMode.Light);
        service.SetTheme(ThemeMode.System);

        Assert.Equal(ThemeMode.System, service.CurrentTheme);
    }

    [Fact]
    public void Constructor_NullJsRuntime_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ThemeService(null!));
    }
}
