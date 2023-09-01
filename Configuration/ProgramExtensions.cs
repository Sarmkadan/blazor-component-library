// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Infrastructure;
using BlazorComponentLibrary.Middleware;

namespace BlazorComponentLibrary.Configuration;

/// <summary>
/// Extension methods for Program.cs configuration.
/// Provides fluent API for setting up Blazor Component Library.
/// Simplifies startup configuration with sensible defaults.
/// </summary>
public static class ProgramExtensions
{
    /// <summary>
    /// Adds all Blazor Component Library services to dependency injection.
    /// Configure options by passing action parameter.
    /// </summary>
    public static WebApplicationBuilder AddBlazorComponents(
        this WebApplicationBuilder builder,
        Action<LibraryOptions>? configureOptions = null)
    {
        // Add library services
        builder.Services.AddBlazorComponentLibrary(configureOptions);

        // Add logging
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        // Add API endpoints
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add CORS if needed
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return builder;
    }

    /// <summary>
    /// Configures middleware pipeline for Blazor Component Library.
    /// Adds exception handling, logging, rate limiting, etc.
    /// </summary>
    public static WebApplication UseBlazorComponents(
        this WebApplication app,
        Action<PipelineOptions>? configureOptions = null)
    {
        // Configure options
        var options = new PipelineOptions();
        configureOptions?.Invoke(options);

        // Add library middleware
        app.UseBlazorComponentLibrary(o =>
        {
            o.UseExceptionHandling = options.UseExceptionHandling;
            o.UseRequestLogging = options.UseRequestLogging;
            o.UseRequestValidation = options.UseRequestValidation;
            o.UseRateLimiting = options.UseRateLimiting;
            o.RateLimitRequestsPerMinute = options.RateLimitRequestsPerMinute;
        });

        // Swagger UI
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // HTTPS redirect
        app.UseHttpsRedirection();

        // CORS
        app.UseCors("AllowAll");

        // Routing
        app.UseRouting();

        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Map controllers
        app.MapControllers();

        // Health check endpoint
        app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
            .WithName("HealthCheck")
            .WithOpenApi();

        return app;
    }

    /// <summary>
    /// Configures development-specific settings.
    /// Enables detailed logging, Swagger, and debugging tools.
    /// </summary>
    public static WebApplicationBuilder ConfigureDevelopment(this WebApplicationBuilder builder)
    {
        builder.Services.AddBlazorComponentLibrary(options =>
        {
            options.EnableDetailedLogging = true;
            options.ApiRateLimit = 1000; // Higher limit for development
        });

        return builder;
    }

    /// <summary>
    /// Configures production-specific settings.
    /// Enables caching, rate limiting, and performance optimizations.
    /// </summary>
    public static WebApplicationBuilder ConfigureProduction(this WebApplicationBuilder builder)
    {
        builder.Services.AddBlazorComponentLibrary(options =>
        {
            options.EnableDetailedLogging = false;
            options.EnableDistributedCache = true; // Use Redis/distributed cache
            options.ApiRateLimit = 100; // Standard rate limit
        });

        return builder;
    }

    /// <summary>
    /// Gets version information for the library.
    /// </summary>
    public static LibraryInfo GetLibraryInfo()
    {
        return new LibraryInfo
        {
            Name = "Blazor Component Library",
            Version = "1.0.0",
            Author = "Vladyslav Zaiets",
            Website = "https://sarmkadan.com",
            Framework = ".NET 10.0",
            ReleaseDate = new DateTime(2026, 5, 4)
        };
    }
}

/// <summary>
/// Information about the library version.
/// </summary>
public class LibraryInfo
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Framework { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }

    public override string ToString()
    {
        return $"{Name} v{Version} by {Author} (.NET 10.0 compatible)";
    }
}

/// <summary>
/// Example Program.cs setup using the extension methods.
/// </summary>
public static class ProgramExample
{
    /*
    // Example usage in Program.cs:

    var builder = WebApplication.CreateBuilder(args);

    // Configure based on environment
    if (builder.Environment.IsDevelopment())
    {
        builder.ConfigureDevelopment();
    }
    else
    {
        builder.ConfigureProduction();
    }

    // OR manually configure
    builder.AddBlazorComponents(options =>
    {
        options.EnableDetailedLogging = builder.Environment.IsDevelopment();
        options.DefaultCacheExpiration = TimeSpan.FromHours(1);
        options.EnableEventSourcing = true;
        options.ApiRateLimit = 100;
    });

    var app = builder.Build();

    app.UseBlazorComponents(options =>
    {
        options.UseExceptionHandling = true;
        options.UseRequestLogging = true;
        options.UseRequestValidation = true;
        options.UseRateLimiting = true;
        options.RateLimitRequestsPerMinute = 100;
    });

    app.Run();
    */
}
