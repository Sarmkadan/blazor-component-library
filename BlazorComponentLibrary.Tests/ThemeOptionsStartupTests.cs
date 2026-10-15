using BlazorComponentLibrary.Extensions;
using BlazorComponentLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit;

namespace BlazorComponentLibrary.Tests;

/// <summary>
/// Tests that <c>AddBlazorComponentLibraryTheme</c> validates <see cref="ThemeOptions"/> when the host starts.
/// </summary>
public sealed class ThemeOptionsStartupTests
{
    /// <summary>
    /// Verifies that a valid theme configuration starts the host without throwing.
    /// </summary>
    [Fact]
    public async Task StartHost_ValidThemeOptions_Starts()
    {
        // Arrange
        using var host = CreateHost(options =>
        {
            options.DefaultTheme = "brand";
            options.Themes["brand"] = new ThemeDefinition
            {
                CssVariables = { ["--bcl-primary"] = "#1e90ff" }
            };
        });

        // Act
        var exception = await Record.ExceptionAsync(() => host.StartAsync());
        await host.StopAsync();

        // Assert
        Assert.Null(exception);
    }

    /// <summary>
    /// Verifies that an invalid theme configuration makes host startup throw an
    /// <see cref="OptionsValidationException"/>, rather than failing later at first render.
    /// </summary>
    [Fact]
    public async Task StartHost_InvalidThemeOptions_ThrowsOptionsValidationException()
    {
        // Arrange
        using var host = CreateHost(options => options.DefaultTheme = "missing");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OptionsValidationException>(() => host.StartAsync());
        Assert.Contains("DefaultTheme 'missing' is not one of the registered themes", exception.Message);
    }

    /// <summary>
    /// Verifies that calling the registration twice does not register the validator twice.
    /// </summary>
    [Fact]
    public void AddBlazorComponentLibraryTheme_CalledTwice_RegistersValidatorOnce()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddBlazorComponentLibraryTheme();
        services.AddBlazorComponentLibraryTheme();

        // Assert
        Assert.Single(services, d => d.ServiceType == typeof(IValidateOptions<ThemeOptions>));
    }

    private static IHost CreateHost(Action<ThemeOptions> configure) =>
        new HostBuilder()
            .ConfigureServices(services => services.AddBlazorComponentLibraryTheme(configure))
            .Build();
}
