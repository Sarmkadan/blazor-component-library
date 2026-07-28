using System;
using BlazorComponentLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace BlazorComponentLibrary.Extensions;

/// <summary>
/// Extension methods for registering library services with the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all BlazorComponentLibrary services with the <see cref="IServiceCollection"/>.
    /// Calls the overload with no configuration delegate.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is <c>null</c>.</exception>
    public static IServiceCollection AddBlazorComponentLibrary(this IServiceCollection services) =>
        services.AddBlazorComponentLibrary(null);

    /// <summary>
    /// Registers all BlazorComponentLibrary services with the <see cref="IServiceCollection"/>,
    /// allowing optional configuration of library‑specific options.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configure">
    /// An optional delegate to configure <see cref="BlazorComponentLibraryOptions"/>.
    /// If <c>null</c>, default options are used.
    /// </param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is <c>null</c>.</exception>
    public static IServiceCollection AddBlazorComponentLibrary(
        this IServiceCollection services,
        Action<BlazorComponentLibraryOptions>? configure)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new BlazorComponentLibraryOptions();
        configure?.Invoke(options);

        // TryAdd so an application can register its own IThemeService/IToastService
        // implementation before calling this and the library will pick it up.
        services.TryAddScoped<IThemeService, ThemeService>();
        services.TryAddScoped<IToastService, ToastService>();

        if (options.RegisterLogging)
        {
            services.AddLogging();
        }

        return services;
    }
}
