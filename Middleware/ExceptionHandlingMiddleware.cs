// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Net;
using System.Text.Json;
using BlazorComponentLibrary.Models;

namespace BlazorComponentLibrary.Middleware;

/// <summary>
/// Middleware for centralized exception handling.
/// Catches unhandled exceptions, logs them, and returns appropriate HTTP responses.
/// Prevents sensitive error information from leaking to clients.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Invokes the middleware to handle exceptions in the request pipeline.
    /// Wraps downstream execution in try-catch to intercept all exceptions.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Determines exception type and returns appropriate response.
    /// Maps business exceptions to specific HTTP status codes.
    /// Logs exception details for debugging and monitoring.
    /// </summary>
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var requestId = context.TraceIdentifier;
        var response = context.Response;
        response.ContentType = "application/json";

        var exceptionResponse = new ExceptionResponse
        {
            RequestId = requestId,
            Timestamp = DateTime.UtcNow,
            Message = "An error occurred while processing your request"
        };

        // Determine status code and message based on exception type
        switch (exception)
        {
            case ArgumentNullException:
            case ArgumentException:
                response.StatusCode = StatusCodes.Status400BadRequest;
                exceptionResponse.Message = "Invalid argument provided";
                _logger.LogWarning(exception, "Argument validation error: {Message}", exception.Message);
                break;

            case KeyNotFoundException:
                response.StatusCode = StatusCodes.Status404NotFound;
                exceptionResponse.Message = "Resource not found";
                _logger.LogWarning(exception, "Resource not found: {Message}", exception.Message);
                break;

            case InvalidOperationException:
                response.StatusCode = StatusCodes.Status409Conflict;
                exceptionResponse.Message = "Operation cannot be performed in current state";
                _logger.LogWarning(exception, "Invalid operation: {Message}", exception.Message);
                break;

            case UnauthorizedAccessException:
                response.StatusCode = StatusCodes.Status401Unauthorized;
                exceptionResponse.Message = "Unauthorized access";
                _logger.LogWarning(exception, "Unauthorized access attempt: {Message}", exception.Message);
                break;

            case TimeoutException:
                response.StatusCode = StatusCodes.Status504GatewayTimeout;
                exceptionResponse.Message = "Request timeout";
                _logger.LogError(exception, "Request timeout: {Message}", exception.Message);
                break;

            case ComponentLibraryException componentEx:
                response.StatusCode = componentEx.StatusCode;
                exceptionResponse.Message = componentEx.Message;
                _logger.LogError(exception, "Component library error ({StatusCode}): {Message}",
                    componentEx.StatusCode, componentEx.Message);
                break;

            default:
                response.StatusCode = StatusCodes.Status500InternalServerError;
                exceptionResponse.Message = "Internal server error";
                _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

                #if DEBUG
                exceptionResponse.Details = exception.ToString();
                #endif
                break;
        }

        return response.WriteAsJsonAsync(exceptionResponse, JsonSerializerOptions.Default);
    }
}

/// <summary>
/// Standard exception response format returned to clients.
/// Includes request ID for correlation with server logs.
/// </summary>
public class ExceptionResponse
{
    public string RequestId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
}

/// <summary>
/// Extension method to register exception handling middleware in the pipeline.
/// </summary>
public static class ExceptionHandlingExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
