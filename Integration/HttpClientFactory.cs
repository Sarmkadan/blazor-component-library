// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

namespace BlazorComponentLibrary.Integration;

/// <summary>
/// Factory for creating and configuring HTTP clients.
/// Manages named clients with specific configurations (timeouts, headers, etc).
/// Centralizes HTTP client creation for consistent configuration across the app.
/// </summary>
public class HttpClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HttpClientFactory> _logger;
    private readonly Dictionary<string, HttpClientConfiguration> _configurations = new();

    public HttpClientFactory(IHttpClientFactory httpClientFactory, ILogger<HttpClientFactory> logger)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Registers a named HTTP client with custom configuration.
    /// Configuration is applied when client is created.
    /// </summary>
    public void RegisterClient(string clientName, HttpClientConfiguration config)
    {
        if (string.IsNullOrEmpty(clientName))
            throw new ArgumentException("Client name cannot be null or empty", nameof(clientName));

        if (config == null)
            throw new ArgumentNullException(nameof(config));

        _configurations[clientName] = config;
        _logger.LogInformation("Registered HTTP client: {ClientName}", clientName);
    }

    /// <summary>
    /// Creates or retrieves a named HTTP client with registered configuration.
    /// Returns default client if configuration not found.
    /// </summary>
    public HttpClient GetClient(string clientName)
    {
        var client = _httpClientFactory.CreateClient(clientName);

        if (_configurations.TryGetValue(clientName, out var config))
        {
            ApplyConfiguration(client, config);
        }

        return client;
    }

    /// <summary>
    /// Creates default HTTP client with base configuration.
    /// </summary>
    public HttpClient GetDefaultClient()
    {
        var client = _httpClientFactory.CreateClient();
        var defaultConfig = new HttpClientConfiguration { Timeout = TimeSpan.FromSeconds(30) };
        ApplyConfiguration(client, defaultConfig);
        return client;
    }

    /// <summary>
    /// Applies configuration to HTTP client instance.
    /// Sets timeout, headers, and other client-level settings.
    /// </summary>
    private void ApplyConfiguration(HttpClient client, HttpClientConfiguration config)
    {
        if (config.Timeout.HasValue)
        {
            client.Timeout = config.Timeout.Value;
        }

        if (!string.IsNullOrEmpty(config.BaseAddress))
        {
            client.BaseAddress = new Uri(config.BaseAddress);
        }

        if (config.DefaultHeaders != null)
        {
            foreach (var header in config.DefaultHeaders)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        if (config.AcceptedMediaTypes != null)
        {
            foreach (var mediaType in config.AcceptedMediaTypes)
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(mediaType));
            }
        }
    }

    /// <summary>
    /// Makes GET request and deserializes response to object.
    /// Includes error handling and logging.
    /// </summary>
    public async Task<T?> GetAsync<T>(string clientName, string endpoint) where T : class
    {
        var client = GetClient(clientName);

        try
        {
            _logger.LogInformation("GET request to {Endpoint}", endpoint);
            // Fix: Added using statement to properly dispose HttpResponseMessage and prevent connection leaks
            using var response = await client.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error on GET {Endpoint}", endpoint);
            throw;
        }
    }

    /// <summary>
    /// Makes POST request with JSON body.
    /// Returns deserialized response.
    /// </summary>
    public async Task<T?> PostAsync<T, TRequest>(string clientName, string endpoint, TRequest request)
        where T : class
        where TRequest : class
    {
        var client = GetClient(clientName);

        try
        {
            _logger.LogInformation("POST request to {Endpoint}", endpoint);
            var json = JsonConvert.SerializeObject(request);
            // Fix: Added using statement to properly dispose StringContent
            using var requestContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            // Fix: Added using statement to properly dispose HttpResponseMessage and prevent connection leaks
            using var response = await client.PostAsync(endpoint, requestContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error on POST {Endpoint}", endpoint);
            throw;
        }
    }

    /// <summary>
    /// Makes PUT request with JSON body.
    /// </summary>
    public async Task<T?> PutAsync<T, TRequest>(string clientName, string endpoint, TRequest request)
        where T : class
        where TRequest : class
    {
        var client = GetClient(clientName);

        try
        {
            _logger.LogInformation("PUT request to {Endpoint}", endpoint);
            var json = JsonConvert.SerializeObject(request);
            // Fix: Added using statement to properly dispose StringContent
            using var requestContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            // Fix: Added using statement to properly dispose HttpResponseMessage and prevent connection leaks
            using var response = await client.PutAsync(endpoint, requestContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error on PUT {Endpoint}", endpoint);
            throw;
        }
    }

    /// <summary>
    /// Makes DELETE request.
    /// </summary>
    public async Task<bool> DeleteAsync(string clientName, string endpoint)
    {
        var client = GetClient(clientName);

        try
        {
            _logger.LogInformation("DELETE request to {Endpoint}", endpoint);
            // Fix: Added using statement to properly dispose HttpResponseMessage and prevent connection leaks
            using var response = await client.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error on DELETE {Endpoint}", endpoint);
            return false;
        }
    }
}

/// <summary>
/// Configuration for HTTP client.
/// </summary>
public class HttpClientConfiguration
{
    /// <summary>
    /// Request timeout duration.
    /// </summary>
    public TimeSpan? Timeout { get; set; }

    /// <summary>
    /// Base address for relative URLs.
    /// </summary>
    public string? BaseAddress { get; set; }

    /// <summary>
    /// Default HTTP headers sent with every request.
    /// </summary>
    public Dictionary<string, string>? DefaultHeaders { get; set; }

    /// <summary>
    /// Accepted media types for responses.
    /// </summary>
    public List<string>? AcceptedMediaTypes { get; set; }
}

/// <summary>
/// Extension method to register HTTP client factory in dependency injection.
/// </summary>
public static class HttpClientFactoryExtensions
{
    public static IServiceCollection AddHttpClientFactory(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<HttpClientFactory>();
        return services;
    }
}
