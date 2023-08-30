// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Diagnostics;

namespace BlazorComponentLibrary.Middleware;

/// <summary>
/// Middleware for HTTP request/response logging.
/// Captures request details, execution time, and response status.
/// Useful for monitoring, debugging, and performance analysis.
/// </summary>
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Logs incoming request and outgoing response with metrics.
    /// Records elapsed time to track performance issues.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // Store original response stream
        var originalBody = context.Response.Body;

        try
        {
            using (var memoryStream = new MemoryStream())
            {
                context.Response.Body = memoryStream;

                // Log incoming request
                LogRequest(context);

                // Execute next middleware
                await _next(context);

                stopwatch.Stop();

                // Log outgoing response
                LogResponse(context, stopwatch);

                // Copy response back to original stream
                memoryStream.Seek(0);
                await memoryStream.CopyToAsync(originalBody);
            }
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }

    /// <summary>
    /// Logs HTTP request details including method, path, and headers.
    /// Excludes sensitive headers to maintain security.
    /// </summary>
    private void LogRequest(HttpContext context)
    {
        var request = context.Request;
        var headersToLog = GetSafeHeaders(request.Headers);

        _logger.LogInformation(
            "Request: {Method} {Path} | Headers: {Headers}",
            request.Method,
            request.Path,
            string.Join(", ", headersToLog)
        );
    }

    /// <summary>
    /// Logs HTTP response details including status code and execution time.
    /// Execution time helps identify slow endpoints for optimization.
    /// </summary>
    private void LogResponse(HttpContext context, Stopwatch stopwatch)
    {
        var response = context.Response;
        var statusCode = response.StatusCode;
        var elapsedMs = stopwatch.ElapsedMilliseconds;

        var logLevel = statusCode >= 500 ? LogLevel.Error :
                       statusCode >= 400 ? LogLevel.Warning :
                       LogLevel.Information;

        _logger.Log(
            logLevel,
            "Response: {StatusCode} from {Method} {Path} | Duration: {ElapsedMs}ms",
            statusCode,
            context.Request.Method,
            context.Request.Path,
            elapsedMs
        );

        // Flag slow requests
        if (elapsedMs > 5000)
        {
            _logger.LogWarning(
                "Slow request detected: {Path} took {ElapsedMs}ms",
                context.Request.Path,
                elapsedMs
            );
        }
    }

    /// <summary>
    /// Filters headers to exclude sensitive information.
    /// Prevents authorization tokens and secrets from appearing in logs.
    /// </summary>
    private List<string> GetSafeHeaders(IHeaderDictionary headers)
    {
        var sensitiveHeaders = new[] { "Authorization", "X-API-Key", "Cookie", "Set-Cookie" };
        var safeHeaders = new List<string>();

        foreach (var header in headers)
        {
            if (sensitiveHeaders.Contains(header.Key, StringComparer.OrdinalIgnoreCase))
            {
                safeHeaders.Add($"{header.Key}: [REDACTED]");
            }
            else
            {
                safeHeaders.Add($"{header.Key}: {header.Value}");
            }
        }

        return safeHeaders;
    }
}

/// <summary>
/// Extension method to register logging middleware in the pipeline.
/// </summary>
public static class LoggingExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LoggingMiddleware>();
    }
}
