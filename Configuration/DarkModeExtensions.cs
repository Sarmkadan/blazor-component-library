// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorComponentLibrary.Configuration;

/// <summary>
/// Extension methods for registering dark mode support in the dependency injection container.
/// </summary>
public static class DarkModeExtensions
{
    /// <summary>
    /// Adds <see cref="DarkModeService"/> and its configuration to the service collection.
    /// <para>
    /// Typical usage in <c>Program.cs</c>:
    /// <code>
    /// builder.Services.AddDarkModeSupport(opt =>
    /// {
    ///     opt.DefaultToDarkMode   = false;
    ///     opt.TransitionDurationMs = 150;
    /// });
    /// </code>
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configure">Optional delegate to customise <see cref="DarkModeOptions"/>.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for call chaining.</returns>
    public static IServiceCollection AddDarkModeSupport(
        this IServiceCollection services,
        Action<DarkModeOptions>? configure = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        var options = new DarkModeOptions();
        configure?.Invoke(options);

        if (!options.IsValid())
            throw new InvalidOperationException("DarkModeOptions configuration is invalid.");

        services.AddSingleton(options);
        services.AddScoped<DarkModeService>();

        return services;
    }
}

/// <summary>
/// Configuration options for the dark mode subsystem.
/// Passed to <see cref="DarkModeExtensions.AddDarkModeSupport"/> via an optional delegate.
/// </summary>
public class DarkModeOptions
{
    /// <summary>
    /// Default dark background colour used for new users with no saved preference.
    /// Default: <c>#121212</c>.
    /// </summary>
    public string DefaultDarkBackground { get; set; } = "#121212";

    /// <summary>
    /// Default dark foreground (text) colour used for new users with no saved preference.
    /// Default: <c>#e0e0e0</c>.
    /// </summary>
    public string DefaultDarkText { get; set; } = "#e0e0e0";

    /// <summary>
    /// Duration in milliseconds for the colour transition animation when switching modes.
    /// Must be between 0 and 2000. Default: <c>200</c>.
    /// </summary>
    public int TransitionDurationMs { get; set; } = 200;

    /// <summary>
    /// When <c>true</c>, users without a saved preference default to dark mode
    /// instead of light mode. Default: <c>false</c>.
    /// </summary>
    public bool DefaultToDarkMode { get; set; }

    /// <summary>
    /// Validates that the options are within acceptable ranges.
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(DefaultDarkBackground) &&
               !string.IsNullOrWhiteSpace(DefaultDarkText) &&
               TransitionDurationMs >= 0 &&
               TransitionDurationMs <= 2000;
    }
}
