// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Collections.Concurrent;

namespace BlazorComponentLibrary.Middleware;

/// <summary>
/// Middleware for implementing rate limiting on API endpoints.
/// Uses sliding window approach to track requests per IP address.
/// Prevents API abuse and ensures fair resource usage.
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly RateLimitOptions _options;
    private static readonly ConcurrentDictionary<string, RateLimitBucket> _buckets = new();

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, RateLimitOptions options)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Enforces rate limiting based on IP address and request count.
    /// Returns HTTP 429 (Too Many Requests) when limit is exceeded.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        // Skip rate limiting for health check endpoints
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        var clientIp = GetClientIpAddress(context);
        var bucket = _buckets.GetOrAdd(clientIp, new RateLimitBucket(_options.RequestLimit, _options.WindowSizeSeconds));

        if (!bucket.TryConsume())
        {
            _logger.LogWarning("Rate limit exceeded for IP: {ClientIp}", clientIp);
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers.Add("Retry-After", _options.WindowSizeSeconds.ToString());
            await context.Response.WriteAsJsonAsync(new { error = "Rate limit exceeded. Please try again later." });
            return;
        }

        // Add rate limit headers to response
        context.Response.Headers.Add("X-RateLimit-Limit", _options.RequestLimit.ToString());
        context.Response.Headers.Add("X-RateLimit-Remaining", bucket.RemainingRequests.ToString());
        context.Response.Headers.Add("X-RateLimit-Reset", bucket.ResetTime.ToUnixTimeSeconds().ToString());

        await _next(context);
    }

    /// <summary>
    /// Extracts client IP address from request.
    /// Handles proxy scenarios where IP is forwarded in headers.
    /// </summary>
    private string GetClientIpAddress(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
        {
            var ips = forwarded.ToString().Split(',');
            return ips[0].Trim();
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}

/// <summary>
/// Token bucket implementation for rate limiting.
/// Tracks requests and resets quota on window expiration.
/// </summary>
public class RateLimitBucket
{
    private int _requestsRemaining;
    private DateTime _resetTime;
    private readonly int _requestLimit;
    private readonly int _windowSizeSeconds;
    private readonly object _lock = new();

    public RateLimitBucket(int requestLimit, int windowSizeSeconds)
    {
        _requestLimit = requestLimit;
        _windowSizeSeconds = windowSizeSeconds;
        _requestsRemaining = requestLimit;
        _resetTime = DateTime.UtcNow.AddSeconds(windowSizeSeconds);
    }

    /// <summary>
    /// Attempts to consume a request from the bucket.
    /// Returns true if limit not exceeded, false otherwise.
    /// Automatically resets on window expiration.
    /// </summary>
    public bool TryConsume()
    {
        lock (_lock)
        {
            if (DateTime.UtcNow >= _resetTime)
            {
                _requestsRemaining = _requestLimit;
                _resetTime = DateTime.UtcNow.AddSeconds(_windowSizeSeconds);
            }

            if (_requestsRemaining > 0)
            {
                _requestsRemaining--;
                return true;
            }

            return false;
        }
    }

    public int RemainingRequests
    {
        get { lock (_lock) { return _requestsRemaining; } }
    }

    public DateTime ResetTime
    {
        get { lock (_lock) { return _resetTime; } }
    }
}

/// <summary>
/// Configuration for rate limiting behavior.
/// </summary>
public class RateLimitOptions
{
    public int RequestLimit { get; set; } = 100;
    public int WindowSizeSeconds { get; set; } = 60;
}

/// <summary>
/// Extension method to register rate limiting middleware in the pipeline.
/// </summary>
public static class RateLimitingExtensions
{
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app, RateLimitOptions? options = null)
    {
        options ??= new RateLimitOptions();
        return app.UseMiddleware<RateLimitingMiddleware>(options);
    }
}
