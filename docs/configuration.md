# Configuration Guide

Complete reference for configuring the Blazor Component Library.

## Quick Start

The simplest configuration requires only one line:

```csharp
builder.Services.AddBlazorComponentLibrary();
```

This registers all services with default options.

## Advanced Configuration

### Using LibraryOptions

```csharp
builder.Services.AddBlazorComponentLibrary(options =>
{
    // Caching configuration
    options.EnableCaching = true;
    options.CacheDurationMinutes = 30;

    // Pagination configuration
    options.DefaultPageSize = 25;
    options.MaxPageSize = 1000;

    // Feature flags
    options.EnableEventBus = true;
    options.EnableLogging = true;
    options.EnableRateLimiting = false;
    options.StrictValidation = true;
    options.EnableCustomValidators = true;
});
```

## Configuration Sources

### 1. Code-Based Configuration

Configure directly in `Program.cs`:

```csharp
builder.Services.AddBlazorComponentLibrary(options =>
{
    options.EnableCaching = true;
});
```

**Pros:** Type-safe, IntelliSense support  
**Cons:** Requires recompilation to change

### 2. appsettings.json Configuration

Add configuration to `appsettings.json`:

```json
{
  "BlazorComponentLibrary": {
    "EnableCaching": true,
    "CacheDurationMinutes": 30,
    "DefaultPageSize": 25,
    "MaxPageSize": 1000,
    "EnableEventBus": true,
    "EnableLogging": true,
    "EnableRateLimiting": false,
    "StrictValidation": true,
    "EnableCustomValidators": true
  }
}
```

Then load in `Program.cs`:

```csharp
var options = builder.Configuration.GetSection("BlazorComponentLibrary")
    .Get<LibraryOptions>();

builder.Services.AddBlazorComponentLibrary(cfg =>
{
    cfg.EnableCaching = options.EnableCaching;
    cfg.CacheDurationMinutes = options.CacheDurationMinutes;
    // ... etc
});
```

**Pros:** No recompilation needed, environment-specific configs  
**Cons:** Less type safety

### 3. Environment-Specific Configuration

Create environment-specific files:

```
appsettings.json                    # Defaults
appsettings.Development.json        # Development overrides
appsettings.Staging.json           # Staging overrides
appsettings.Production.json        # Production overrides
```

**Development (appsettings.Development.json):**
```json
{
  "BlazorComponentLibrary": {
    "EnableLogging": true,
    "CacheDurationMinutes": 5,
    "EnableRateLimiting": false
  }
}
```

**Production (appsettings.Production.json):**
```json
{
  "BlazorComponentLibrary": {
    "EnableLogging": true,
    "CacheDurationMinutes": 60,
    "EnableRateLimiting": true,
    "EnableCaching": true
  }
}
```

### 4. Environment Variables

Set via environment variables (useful in containers):

```bash
export BlazorComponentLibrary__EnableCaching=true
export BlazorComponentLibrary__CacheDurationMinutes=60
export BlazorComponentLibrary__EnableRateLimiting=true
```

Docker example:

```yaml
environment:
  - BlazorComponentLibrary__EnableCaching=true
  - BlazorComponentLibrary__CacheDurationMinutes=60
```

## Configuration Options

### Caching

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `EnableCaching` | bool | true | Enable/disable caching layer |
| `CacheDurationMinutes` | int | 30 | How long items stay in cache |

**Example:**
```json
{
  "BlazorComponentLibrary": {
    "EnableCaching": true,
    "CacheDurationMinutes": 60
  }
}
```

### Pagination

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `DefaultPageSize` | int | 25 | Items per page by default |
| `MaxPageSize` | int | 1000 | Maximum items per page allowed |

**Example:**
```json
{
  "BlazorComponentLibrary": {
    "DefaultPageSize": 50,
    "MaxPageSize": 500
  }
}
```

### Features

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `EnableEventBus` | bool | true | Enable event publishing/subscribing |
| `EnableLogging` | bool | true | Enable detailed logging |
| `EnableRateLimiting` | bool | false | Enable rate limiting middleware |
| `StrictValidation` | bool | true | Require strict validation rules |
| `EnableCustomValidators` | bool | true | Allow custom validators |

**Example:**
```json
{
  "BlazorComponentLibrary": {
    "EnableEventBus": true,
    "EnableLogging": true,
    "EnableRateLimiting": true,
    "StrictValidation": false
  }
}
```

### Rate Limiting

When `EnableRateLimiting` is true:

```json
{
  "BlazorComponentLibrary": {
    "EnableRateLimiting": true,
    "RequestsPerMinute": 100,
    "RequestsPerHour": 1000,
    "EndpointExclusions": ["/health", "/status"]
  }
}
```

## Advanced Scenarios

### Using Different Configurations Per Environment

```csharp
var env = builder.Environment;

builder.Services.AddBlazorComponentLibrary(options =>
{
    if (env.IsDevelopment())
    {
        options.EnableLogging = true;
        options.CacheDurationMinutes = 5;
        options.EnableRateLimiting = false;
    }
    else if (env.IsProduction())
    {
        options.EnableLogging = true;
        options.CacheDurationMinutes = 60;
        options.EnableRateLimiting = true;
    }
});
```

### Dynamic Configuration

Load configuration dynamically:

```csharp
var config = builder.Configuration.GetSection("BlazorComponentLibrary");

builder.Services.Configure<LibraryOptions>(config);
builder.Services.AddBlazorComponentLibrary();
```

### Configuration Validation

Validate configuration on startup:

```csharp
builder.Services.AddOptions<LibraryOptions>()
    .BindConfiguration("BlazorComponentLibrary")
    .ValidateDataAnnotations()
    .ValidateOnStart();
```

### Custom Configuration Classes

Create your own configuration class:

```csharp
public class BlazorLibraryConfig
{
    [Required]
    public CachingConfig Caching { get; set; } = new();
    
    [Required]
    public PaginationConfig Pagination { get; set; } = new();
}

public class CachingConfig
{
    public bool Enabled { get; set; } = true;
    public int DurationMinutes { get; set; } = 30;
}

public class PaginationConfig
{
    public int DefaultPageSize { get; set; } = 25;
    public int MaxPageSize { get; set; } = 1000;
}
```

## Logging Configuration

### Enable Debug Logging

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "BlazorComponentLibrary": "Debug",
      "BlazorComponentLibrary.Services": "Debug"
    }
  }
}
```

### Log Levels

- **Trace**: Very detailed information
- **Debug**: Detailed diagnostic information
- **Information**: General information messages
- **Warning**: Warning messages for potentially harmful situations
- **Error**: Error messages for error events
- **Critical**: Critical messages for critical failures
- **None**: Disable logging

### Structured Logging with Serilog

```bash
dotnet add package Serilog
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
```

```csharp
builder.Host.UseSerilog((context, configuration) =>
    configuration
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .WriteTo.File(
            "logs/.txt",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}"));
```

## Database Configuration

### Using Entity Framework Core

```csharp
// Configure DbContext
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register custom repositories
services.AddScoped<IComponentRepository, EfComponentRepository>();
services.AddScoped<IDataRepository, EfDataRepository>();
```

**appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=BlazorComponentLibrary;Trusted_Connection=true;"
  }
}
```

### Using MongoDB

```csharp
var mongoConnection = builder.Configuration.GetConnectionString("MongoDb");
services.AddSingleton<IMongoClient>(new MongoClient(mongoConnection));

services.AddScoped<IComponentRepository, MongoComponentRepository>();
```

**appsettings.json:**
```json
{
  "ConnectionStrings": {
    "MongoDb": "mongodb://localhost:27017"
  }
}
```

## Middleware Configuration

### Exception Handling

```csharp
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

### Request Logging

```csharp
app.UseMiddleware<LoggingMiddleware>();
```

### Rate Limiting

```csharp
app.UseMiddleware<RateLimitingMiddleware>(
    new RateLimitOptions
    {
        RequestsPerMinute = 100,
        EndpointExclusions = new[] { "/health" }
    });
```

### Request Validation

```csharp
app.UseMiddleware<RequestValidationMiddleware>();
```

## Health Checks

Configure health checks:

```csharp
services.AddHealthChecks()
    .AddCheck("Database", async () =>
    {
        // Check database connectivity
        return HealthCheckResult.Healthy();
    })
    .AddCheck("Cache", async () =>
    {
        // Check cache connectivity
        return HealthCheckResult.Healthy();
    });

app.MapHealthChecks("/health");
```

## Troubleshooting Configuration Issues

### "Configuration not found" error

Ensure:
1. `appsettings.json` exists in project root
2. File is copied to output directory (`CopyToOutputDirectory=PreserveNewest`)
3. Section name matches exactly (case-sensitive)

### Changes not taking effect

1. Check if running debug or release
2. Stop and restart application
3. Clear any cached configuration
4. Verify environment variables are set correctly

### Configuration binding errors

Ensure types match:
```json
// Correct - number type
{ "CacheDurationMinutes": 30 }

// Incorrect - string type
{ "CacheDurationMinutes": "30" }
```

---

See also: [Getting Started](getting-started.md) | [Deployment](deployment.md) | [FAQ](faq.md)
