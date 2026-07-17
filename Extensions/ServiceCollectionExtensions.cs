namespace BlazorComponentLibrary.Extensions;

/// <summary>
/// Contains extension methods for <see cref="IServiceCollection"/> that register BlazorComponentLibrary
/// services with the dependency injection container.
/// </summary>
using BlazorComponentLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

/// <summary>Extension methods for registering library services with the DI container.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all BlazorComponentLibrary services with the <see cref="IServiceCollection"/>.
    /// Call this once in <c>Program.cs</c>:
    /// <code>builder.Services.AddBlazorComponentLibrary();</code>
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    public static IServiceCollection AddBlazorComponentLibrary(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // TryAdd so an application can register its own IThemeService/IToastService
        // implementation before calling this and the library will pick it up.
        services.TryAddScoped<IThemeService, ThemeService>();
        services.TryAddScoped<IToastService, ToastService>();
        services.AddLogging();

        return services;
    }
}