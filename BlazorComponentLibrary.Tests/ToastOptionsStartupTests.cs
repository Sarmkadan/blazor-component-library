using BlazorComponentLibrary.Components.Toast;
using BlazorComponentLibrary.Extensions;
using BlazorComponentLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit;

namespace BlazorComponentLibrary.Tests;

/// <summary>
/// Tests that <c>AddBlazorComponentLibrary</c> validates <see cref="ToastOptions"/> when the host starts.
/// </summary>
public sealed class ToastOptionsStartupTests
{
    /// <summary>
    /// Verifies that a <see cref="ToastOptions.DefaultDuration"/> at either boundary (500 ms or 60 s) starts the host.
    /// </summary>
    [Theory]
    [InlineData(500)]
    [InlineData(60_000)]
    public async Task StartHost_DefaultDurationAtBoundary_Starts(int durationMs)
    {
        // Arrange
        using var host = CreateHost(options => options.DefaultDuration = TimeSpan.FromMilliseconds(durationMs));

        // Act
        var exception = await Record.ExceptionAsync(() => host.StartAsync());
        await host.StopAsync();

        // Assert
        Assert.Null(exception);
    }

    /// <summary>
    /// Verifies that a <see cref="ToastOptions.MaxVisibleToasts"/> at either boundary (1 or 20) starts the host.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(20)]
    public async Task StartHost_MaxVisibleToastsAtBoundary_Starts(int maxVisibleToasts)
    {
        // Arrange
        using var host = CreateHost(options => options.MaxVisibleToasts = maxVisibleToasts);

        // Act
        var exception = await Record.ExceptionAsync(() => host.StartAsync());
        await host.StopAsync();

        // Assert
        Assert.Null(exception);
    }

    /// <summary>
    /// Verifies that a <see cref="ToastOptions.DefaultDuration"/> below 500 ms or above 60 s makes host startup
    /// throw an <see cref="OptionsValidationException"/>.
    /// </summary>
    [Theory]
    [InlineData(499)]
    [InlineData(0)]
    [InlineData(60_001)]
    public async Task StartHost_DefaultDurationOutOfRange_ThrowsOptionsValidationException(int durationMs)
    {
        // Arrange
        using var host = CreateHost(options => options.DefaultDuration = TimeSpan.FromMilliseconds(durationMs));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OptionsValidationException>(() => host.StartAsync());
        Assert.Contains("DefaultDuration must be between 500 ms and 60 s", exception.Message);
    }

    /// <summary>
    /// Verifies that a <see cref="ToastOptions.MaxVisibleToasts"/> below 1 or above 20 makes host startup
    /// throw an <see cref="OptionsValidationException"/>.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(21)]
    public async Task StartHost_MaxVisibleToastsOutOfRange_ThrowsOptionsValidationException(int maxVisibleToasts)
    {
        // Arrange
        using var host = CreateHost(options => options.MaxVisibleToasts = maxVisibleToasts);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OptionsValidationException>(() => host.StartAsync());
        Assert.Contains("MaxVisibleToasts must be between 1 and 20", exception.Message);
    }

    /// <summary>
    /// Verifies that a <see cref="ToastOptions.Position"/> that is not a defined <see cref="ToastPosition"/>
    /// makes host startup throw an <see cref="OptionsValidationException"/>.
    /// </summary>
    [Fact]
    public async Task StartHost_UndefinedPosition_ThrowsOptionsValidationException()
    {
        // Arrange
        using var host = CreateHost(options => options.Position = (ToastPosition)99);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OptionsValidationException>(() => host.StartAsync());
        Assert.Contains("is not a defined ToastPosition value", exception.Message);
    }

    /// <summary>
    /// Verifies that when several toast options are invalid, the single <see cref="OptionsValidationException"/>
    /// reports every failure rather than only the first.
    /// </summary>
    [Fact]
    public async Task StartHost_MultipleInvalidValues_ReportsEveryFailure()
    {
        // Arrange
        using var host = CreateHost(options =>
        {
            options.DefaultDuration = TimeSpan.FromMilliseconds(100);
            options.MaxVisibleToasts = 50;
        });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OptionsValidationException>(() => host.StartAsync());
        Assert.Contains("DefaultDuration must be between 500 ms and 60 s", exception.Message);
        Assert.Contains("MaxVisibleToasts must be between 1 and 20", exception.Message);
    }

    /// <summary>
    /// Verifies that calling the registration twice does not register the toast options validator twice.
    /// </summary>
    [Fact]
    public void AddBlazorComponentLibrary_CalledTwice_RegistersToastValidatorOnce()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddBlazorComponentLibrary(null);
        services.AddBlazorComponentLibrary(null);

        // Assert
        Assert.Single(services, d => d.ServiceType == typeof(IValidateOptions<ToastOptions>));
    }

    private static IHost CreateHost(Action<ToastOptions> configureToast) =>
        new HostBuilder()
            .ConfigureServices(services => services.AddBlazorComponentLibrary(null, configureToast))
            .Build();
}
