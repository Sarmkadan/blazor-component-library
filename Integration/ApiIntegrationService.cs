// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

namespace BlazorComponentLibrary.Integration;

/// <summary>
/// Service for integrating with external APIs.
/// Provides high-level abstraction over HTTP communication.
/// Includes error handling, retry logic, and response mapping.
/// </summary>
public class ApiIntegrationService
{
    private readonly HttpClientFactory _httpClientFactory;
    private readonly ILogger<ApiIntegrationService> _logger;
    private readonly ICacheService _cacheService;

    public ApiIntegrationService(
        HttpClientFactory httpClientFactory,
        ILogger<ApiIntegrationService> logger,
        ICacheService cacheService)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    }

    /// <summary>
    /// Makes GET request with optional caching.
    /// Returns cached result if available and not expired.
    /// </summary>
    public async Task<ApiResponse<T>> GetAsync<T>(
        string clientName,
        string endpoint,
        bool useCache = true,
        TimeSpan? cacheDuration = null) where T : class
    {
        if (useCache)
        {
            var cached = await _cacheService.GetAsync<T>($"api_{endpoint}");
            if (cached != null)
            {
                _logger.LogDebug("Returning cached response for {Endpoint}", endpoint);
                return ApiResponse<T>.Success(cached, "From cache");
            }
        }

        try
        {
            var result = await _httpClientFactory.GetAsync<T>(clientName, endpoint);

            if (useCache && result != null)
            {
                await _cacheService.SetAsync($"api_{endpoint}", result, cacheDuration ?? TimeSpan.FromHours(1));
            }

            return ApiResponse<T>.Success(result, "API call successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling GET {Endpoint}", endpoint);
            return ApiResponse<T>.Failure($"API error: {ex.Message}");
        }
    }

    /// <summary>
    /// Makes POST request with retry logic.
    /// Automatically retries on transient failures.
    /// </summary>
    public async Task<ApiResponse<TResponse>> PostAsync<TResponse, TRequest>(
        string clientName,
        string endpoint,
        TRequest request,
        int maxRetries = 3) where TResponse : class where TRequest : class
    {
        try
        {
            var result = await RetryAsync(async () =>
            {
                return await _httpClientFactory.PostAsync<TResponse, TRequest>(clientName, endpoint, request);
            }, maxRetries);

            return ApiResponse<TResponse>.Success(result, "POST successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling POST {Endpoint}", endpoint);
            return ApiResponse<TResponse>.Failure($"API error: {ex.Message}");
        }
    }

    /// <summary>
    /// Makes PUT request for updates.
    /// </summary>
    public async Task<ApiResponse<TResponse>> PutAsync<TResponse, TRequest>(
        string clientName,
        string endpoint,
        TRequest request) where TResponse : class where TRequest : class
    {
        try
        {
            var result = await _httpClientFactory.PutAsync<TResponse, TRequest>(clientName, endpoint, request);

            // Invalidate cache for updated resource
            await _cacheService.RemoveAsync($"api_{endpoint}");

            return ApiResponse<TResponse>.Success(result, "PUT successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling PUT {Endpoint}", endpoint);
            return ApiResponse<TResponse>.Failure($"API error: {ex.Message}");
        }
    }

    /// <summary>
    /// Makes DELETE request.
    /// </summary>
    public async Task<ApiResponse<bool>> DeleteAsync(string clientName, string endpoint)
    {
        try
        {
            var result = await _httpClientFactory.DeleteAsync(clientName, endpoint);

            // Invalidate cache for deleted resource
            await _cacheService.RemoveAsync($"api_{endpoint}");

            return ApiResponse<bool>.Success(result, "DELETE successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling DELETE {Endpoint}", endpoint);
            return ApiResponse<bool>.Failure($"API error: {ex.Message}");
        }
    }

    /// <summary>
    /// Batch retrieves multiple resources.
    /// Executes requests in parallel for performance.
    /// </summary>
    public async Task<ApiResponse<List<T>>> GetMultipleAsync<T>(
        string clientName,
        params string[] endpoints) where T : class
    {
        if (endpoints == null || endpoints.Length == 0)
            return ApiResponse<List<T>>.Success(new List<T>(), "No endpoints");

        try
        {
            var tasks = endpoints.Select(ep => GetAsync<T>(clientName, ep));
            var results = await Task.WhenAll(tasks);

            var data = results
                .Where(r => r.IsSuccess && r.Data != null)
                .Select(r => r.Data!)
                .ToList();

            return ApiResponse<List<T>>.Success(data, $"Retrieved {data.Count} items");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error batch retrieving from {ClientName}", clientName);
            return ApiResponse<List<T>>.Failure($"Batch operation failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Checks API health by making a lightweight request.
    /// Useful for health checks and monitoring.
    /// </summary>
    public async Task<bool> IsHealthyAsync(string clientName, string healthEndpoint = "health")
    {
        try
        {
            var response = await GetAsync<object>(clientName, healthEndpoint, useCache: false);
            return response.IsSuccess;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Retries an operation with exponential backoff.
    /// Handles transient failures gracefully.
    /// </summary>
    private async Task<T> RetryAsync<T>(Func<Task<T>> operation, int maxRetries)
    {
        Exception? lastException = null;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                return await operation();
            }
            catch (HttpRequestException ex)
            {
                lastException = ex;

                if (attempt < maxRetries)
                {
                    var delay = (int)Math.Pow(2, attempt - 1) * 1000;
                    _logger.LogWarning("Retry attempt {Attempt} after {DelayMs}ms", attempt, delay);
                    await Task.Delay(delay);
                }
            }
        }

        throw lastException ?? new InvalidOperationException("Operation failed after retries");
    }
}

/// <summary>
/// Standard API response format.
/// Wraps data with metadata about the request outcome.
/// </summary>
public class ApiResponse<T> where T : class
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? ErrorCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> Success(T? data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> Failure(string message, int? errorCode = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Data = null,
            Message = message,
            ErrorCode = errorCode
        };
    }
}

/// <summary>
/// Extension method to register API integration service in dependency injection.
/// </summary>
public static class ApiIntegrationExtensions
{
    public static IServiceCollection AddApiIntegration(this IServiceCollection services)
    {
        services.AddScoped<ApiIntegrationService>();
        return services;
    }
}
