// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Repositories;
using BlazorComponentLibrary.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorComponentLibrary.Configuration;

/// <summary>
/// Extension methods for configuring the Blazor Component Library services.
/// Used to register all services and repositories in the dependency injection container.
/// </summary>
public static class ServiceConfiguration
{
    /// <summary>
    /// Adds all component library services to the dependency injection container.
    /// </summary>
    public static IServiceCollection AddBlazorComponentLibrary(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        // Register repositories
        RegisterRepositories(services);

        // Register services
        RegisterServices(services);

        return services;
    }

    /// <summary>
    /// Registers all repository implementations.
    /// </summary>
    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddSingleton<IComponentRepository, ComponentRepository>();
        services.AddSingleton<IDataRepository, DataRepository>();
        services.AddSingleton<IFormRepository, FormRepository>();
        services.AddSingleton<IThemeRepository, ThemeRepository>();
        services.AddSingleton<IUserRepository, UserRepository>();
    }

    /// <summary>
    /// Registers all service implementations.
    /// </summary>
    private static void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<ComponentService>();
        services.AddScoped<DataService>();
        services.AddScoped<FormService>();
        services.AddScoped<ThemeService>();
        services.AddScoped<UserService>();
    }
}

/// <summary>
/// Configuration settings for the component library.
/// </summary>
public class LibrarySettings
{
    public string LibraryName { get; set; } = "Blazor Component Library";
    public string Version { get; set; } = "1.0.0";
    public bool EnableCaching { get; set; } = true;
    public int CacheExpirationMinutes { get; set; } = 30;
    public int MaxRowsPerPage { get; set; } = 1000;
    public int DefaultPageSize { get; set; } = 25;
    public bool EnableLogging { get; set; } = true;
    public string? LogLevel { get; set; } = "Information";

    /// <summary>
    /// Validates the settings configuration.
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(LibraryName) &&
               !string.IsNullOrWhiteSpace(Version) &&
               CacheExpirationMinutes > 0 &&
               MaxRowsPerPage > 0 &&
               DefaultPageSize > 0 &&
               DefaultPageSize <= MaxRowsPerPage;
    }
}
