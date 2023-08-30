// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

namespace BlazorComponentLibrary.Middleware;

/// <summary>
/// Middleware for validating HTTP requests before they reach handlers.
/// Checks content-type, content-length, and other request properties.
/// Rejects malformed requests early to prevent downstream errors.
/// </summary>
public class RequestValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestValidationMiddleware> _logger;
    private readonly RequestValidationOptions _options;

    public RequestValidationMiddleware(RequestDelegate next, ILogger<RequestValidationMiddleware> logger, RequestValidationOptions? options = null)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? new RequestValidationOptions();
    }

    /// <summary>
    /// Validates request before passing to next middleware.
    /// Checks for common issues like missing content-type, oversized payloads, etc.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;

        // Skip validation for GET requests and health checks
        if (request.Method == "GET" || request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        // Validate content-type for POST/PUT/PATCH requests
        if (!IsValidContentType(request.ContentType))
        {
            _logger.LogWarning("Invalid content-type: {ContentType} for {Method} {Path}",
                request.ContentType, request.Method, request.Path);

            context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
            await context.Response.WriteAsJsonAsync(new { error = "Unsupported Media Type. Expected application/json" });
            return;
        }

        // Validate content-length
        if (request.ContentLength.HasValue && request.ContentLength > _options.MaxContentLengthBytes)
        {
            _logger.LogWarning("Request too large: {ContentLength} bytes for {Method} {Path}",
                request.ContentLength, request.Method, request.Path);

            context.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
            await context.Response.WriteAsJsonAsync(new { error = "Payload Too Large" });
            return;
        }

        await _next(context);
    }

    /// <summary>
    /// Validates that content-type is acceptable for the API.
    /// Allows for both application/json and form data.
    /// </summary>
    private bool IsValidContentType(string? contentType)
    {
        if (string.IsNullOrEmpty(contentType))
            return true; // GET requests don't have content-type

        var allowedTypes = new[] { "application/json", "application/x-www-form-urlencoded", "multipart/form-data" };
        return allowedTypes.Any(t => contentType.StartsWith(t, StringComparison.OrdinalIgnoreCase));
    }
}

/// <summary>
/// Configuration options for request validation.
/// </summary>
public class RequestValidationOptions
{
    /// <summary>
    /// Maximum allowed request content length in bytes (default 10MB).
    /// Protects against extremely large payloads.
    /// </summary>
    public long MaxContentLengthBytes { get; set; } = 10 * 1024 * 1024;
}

/// <summary>
/// Extension method to register request validation middleware in the pipeline.
/// </summary>
public static class RequestValidationExtensions
{
    public static IApplicationBuilder UseRequestValidation(this IApplicationBuilder app, RequestValidationOptions? options = null)
    {
        return app.UseMiddleware<RequestValidationMiddleware>(options);
    }
}
