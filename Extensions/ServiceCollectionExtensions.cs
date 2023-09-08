namespace BlazorComponentLibrary.Extensions;

using BlazorComponentLibrary.Services;
using Microsoft.Extensions.DependencyInjection;

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
    public static IServiceCollection AddBlazorComponentLibrary(this IServiceCollection services)
    {
        services.AddScoped<IThemeService, ThemeService>();
        services.AddScoped<IToastService, ToastService>();
        return services;
    }
}
