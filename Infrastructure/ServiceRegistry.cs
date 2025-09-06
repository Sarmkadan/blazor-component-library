// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

namespace BlazorComponentLibrary.Infrastructure;

/// <summary>
/// Central registry for all service configurations.
/// Centralizes dependency injection setup to prevent repetition.
/// Used by Program.cs to bootstrap all infrastructure services.
/// </summary>
public static class ServiceRegistry
{
    /// <summary>
    /// Registers all library services in dependency injection container.
    /// Call this in Program.cs: builder.Services.AddBlazorComponentLibrary().
    /// </summary>
    public static IServiceCollection AddBlazorComponentLibrary(this IServiceCollection services, Action<LibraryOptions>? configure = null)
    {
        var options = new LibraryOptions();
        configure?.Invoke(options);

        // Register core services
        RegisterCoreServices(services);

        // Register caching services
        RegisterCachingServices(services, options);

        // Register event system
        RegisterEventServices(services);

        // Register integration services
        RegisterIntegrationServices(services);

        // Register background tasks
        RegisterBackgroundServices(services);

        // Register logging
        RegisterLogging(services, options);

        return services;
    }

    /// <summary>
    /// Registers core business services.
    /// </summary>
    private static void RegisterCoreServices(IServiceCollection services)
    {
        // Services
        services.AddScoped<ComponentService>();
        services.AddScoped<DataService>();
        services.AddScoped<FormService>();
        services.AddScoped<ThemeService>();
        services.AddScoped<UserService>();

        // Repositories
        services.AddScoped<IComponentRepository, ComponentRepository>();
        services.AddScoped<IDataRepository, DataRepository>();
        services.AddScoped<IFormRepository, FormRepository>();
        services.AddScoped<IThemeRepository, ThemeRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }

    /// <summary>
    /// Registers caching services.
    /// </summary>
    private static void RegisterCachingServices(IServiceCollection services, LibraryOptions options)
    {
        services.AddCaching();

        if (options.EnableDistributedCache)
        {
            // For distributed caching, add Redis or other provider
            // services.AddStackExchangeRedisCache(...)
        }
    }

    /// <summary>
    /// Registers event system services.
    /// </summary>
    private static void RegisterEventServices(IServiceCollection services)
    {
        services.AddEventBus();
    }

    /// <summary>
    /// Registers integration services.
    /// </summary>
    private static void RegisterIntegrationServices(IServiceCollection services)
    {
        services.AddHttpClientFactory();
        services.AddWebhookHandler();
    }

    /// <summary>
    /// Registers background task services.
    /// </summary>
    private static void RegisterBackgroundServices(IServiceCollection services)
    {
        services.AddBackgroundTasks();
    }

    /// <summary>
    /// Registers logging configuration.
    /// </summary>
    private static void RegisterLogging(IServiceCollection services, LibraryOptions options)
    {
        // Logging is configured elsewhere, but we can add library-specific log providers here
        if (options.EnableDetailedLogging)
        {
            // Add verbose logging configuration
        }
    }
}

/// <summary>
/// Configuration options for Blazor Component Library.
/// </summary>
public class LibraryOptions
{
    /// <summary>
    /// Enable distributed caching (Redis, etc).
    /// Default: false (in-memory cache only).
    /// </summary>
    public bool EnableDistributedCache { get; set; }

    /// <summary>
    /// Enable detailed logging for debugging.
    /// Default: false (production logging).
    /// </summary>
    public bool EnableDetailedLogging { get; set; }

    /// <summary>
    /// Cache expiration default (for items without explicit TTL).
    /// Default: 1 hour.
    /// </summary>
    public TimeSpan DefaultCacheExpiration { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// Enable event sourcing for audit trail.
    /// Default: false.
    /// </summary>
    public bool EnableEventSourcing { get; set; }

    /// <summary>
    /// Enable webhook system.
    /// Default: true.
    /// </summary>
    public bool EnableWebhooks { get; set; } = true;

    /// <summary>
    /// API rate limit requests per minute.
    /// Default: 100.
    /// </summary>
    public int ApiRateLimit { get; set; } = 100;
}

/// <summary>
/// Extension method to configure middleware pipeline.
/// Call this in Program.cs after creating app: app.UseBlazorComponentLibrary().
/// </summary>
public static class MiddlewareRegistry
{
    public static IApplicationBuilder UseBlazorComponentLibrary(this IApplicationBuilder app, Action<PipelineOptions>? configure = null)
    {
        var options = new PipelineOptions();
        configure?.Invoke(options);

        if (options.UseExceptionHandling)
        {
            app.UseExceptionHandling();
        }

        if (options.UseRequestLogging)
        {
            app.UseRequestLogging();
        }

        if (options.UseRequestValidation)
        {
            app.UseRequestValidation();
        }

        if (options.UseRateLimiting)
        {
            var rateLimitOptions = new RateLimitingMiddleware.RateLimitOptions
            {
                RequestLimit = options.RateLimitRequestsPerMinute,
                WindowSizeSeconds = 60
            };
            app.UseRateLimiting(rateLimitOptions);
        }

        return app;
    }
}

/// <summary>
/// Pipeline middleware options.
/// </summary>
public class PipelineOptions
{
    public bool UseExceptionHandling { get; set; } = true;
    public bool UseRequestLogging { get; set; } = true;
    public bool UseRequestValidation { get; set; } = true;
    public bool UseRateLimiting { get; set; } = true;
    public int RateLimitRequestsPerMinute { get; set; } = 100;
}
