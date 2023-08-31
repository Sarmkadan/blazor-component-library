// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

namespace BlazorComponentLibrary.Integration;

/// <summary>
/// Handles webhook dispatching and delivery to external services.
/// Manages retry logic, timeout, and failure tracking.
/// Supports multiple webhook events and subscribers.
/// </summary>
public class WebhookHandler
{
    private readonly HttpClientFactory _httpClientFactory;
    private readonly ILogger<WebhookHandler> _logger;
    private readonly Dictionary<string, List<WebhookSubscription>> _subscriptions = new();

    public WebhookHandler(HttpClientFactory httpClientFactory, ILogger<WebhookHandler> logger)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Registers a webhook subscription for an event type.
    /// Multiple subscribers can listen to the same event.
    /// </summary>
    public void Subscribe(string eventType, string callbackUrl, Dictionary<string, string>? customHeaders = null)
    {
        if (string.IsNullOrEmpty(eventType))
            throw new ArgumentException("Event type cannot be null or empty", nameof(eventType));

        if (string.IsNullOrEmpty(callbackUrl))
            throw new ArgumentException("Callback URL cannot be null or empty", nameof(callbackUrl));

        if (!_subscriptions.ContainsKey(eventType))
        {
            _subscriptions[eventType] = new List<WebhookSubscription>();
        }

        var subscription = new WebhookSubscription
        {
            Id = Guid.NewGuid().ToString(),
            EventType = eventType,
            CallbackUrl = callbackUrl,
            CustomHeaders = customHeaders ?? new Dictionary<string, string>(),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _subscriptions[eventType].Add(subscription);
        _logger.LogInformation("Webhook subscribed to event {EventType}: {CallbackUrl}", eventType, callbackUrl);
    }

    /// <summary>
    /// Unsubscribes a webhook from an event type.
    /// </summary>
    public bool Unsubscribe(string eventType, string subscriptionId)
    {
        if (string.IsNullOrEmpty(eventType) || string.IsNullOrEmpty(subscriptionId))
            return false;

        if (_subscriptions.TryGetValue(eventType, out var subs))
        {
            var sub = subs.FirstOrDefault(s => s.Id == subscriptionId);
            if (sub != null)
            {
                subs.Remove(sub);
                _logger.LogInformation("Webhook unsubscribed from event {EventType}", eventType);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Dispatches webhook event to all registered subscribers.
    /// Handles retries and logs delivery status.
    /// </summary>
    public async Task DispatchAsync(string eventType, WebhookPayload payload)
    {
        if (string.IsNullOrEmpty(eventType))
            throw new ArgumentException("Event type cannot be null or empty", nameof(eventType));

        if (payload == null)
            throw new ArgumentNullException(nameof(payload));

        _logger.LogInformation("Dispatching webhook for event {EventType}", eventType);

        if (!_subscriptions.TryGetValue(eventType, out var subs))
        {
            _logger.LogWarning("No subscribers for event type {EventType}", eventType);
            return;
        }

        var tasks = subs
            .Where(s => s.IsActive)
            .Select(sub => DeliverWebhookAsync(sub, payload));

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Delivers webhook to a single subscriber with retry logic.
    /// Implements exponential backoff for failed attempts.
    /// </summary>
    private async Task DeliverWebhookAsync(WebhookSubscription subscription, WebhookPayload payload, int attempt = 1)
    {
        const int maxAttempts = 3;
        var backoffMs = (int)Math.Pow(2, attempt - 1) * 1000;

        try
        {
            _logger.LogDebug("Delivering webhook to {CallbackUrl} (attempt {Attempt})",
                subscription.CallbackUrl, attempt);

            var client = _httpClientFactory.GetDefaultClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            // Add custom headers
            foreach (var header in subscription.CustomHeaders)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            // Add signature for verification
            var signature = GenerateSignature(payload);
            client.DefaultRequestHeaders.Add("X-Webhook-Signature", signature);
            client.DefaultRequestHeaders.Add("X-Webhook-Event", subscription.EventType);
            client.DefaultRequestHeaders.Add("X-Webhook-Id", subscription.Id);

            var json = JsonConvert.SerializeObject(payload);
            // Fix: Added using statement to properly dispose StringContent
            using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            // Fix: Added using statement to properly dispose HttpResponseMessage and prevent connection leaks
            using var response = await client.PostAsync(subscription.CallbackUrl, content);

            if (response.IsSuccessStatusCode)
            {
                subscription.LastDeliveredAt = DateTime.UtcNow;
                subscription.DeliveryCount++;
                _logger.LogInformation("Webhook delivered successfully to {CallbackUrl}", subscription.CallbackUrl);
            }
            else if (attempt < maxAttempts && IsRetryableStatusCode(response.StatusCode))
            {
                _logger.LogWarning("Webhook delivery failed with status {StatusCode}, retrying in {DelayMs}ms",
                    response.StatusCode, backoffMs);

                await Task.Delay(backoffMs);
                await DeliverWebhookAsync(subscription, payload, attempt + 1);
            }
            else
            {
                subscription.LastError = $"HTTP {response.StatusCode}";
                subscription.FailureCount++;
                _logger.LogError("Webhook delivery failed permanently to {CallbackUrl}", subscription.CallbackUrl);
            }
        }
        catch (HttpRequestException ex) when (attempt < maxAttempts)
        {
            _logger.LogWarning(ex, "Webhook request failed, retrying in {DelayMs}ms", backoffMs);
            await Task.Delay(backoffMs);
            await DeliverWebhookAsync(subscription, payload, attempt + 1);
        }
        catch (Exception ex)
        {
            subscription.LastError = ex.Message;
            subscription.FailureCount++;
            _logger.LogError(ex, "Webhook delivery error for {CallbackUrl}", subscription.CallbackUrl);
        }
    }

    /// <summary>
    /// Determines if HTTP status code should trigger retry.
    /// Retries on server errors and timeout, not on client errors.
    /// </summary>
    private bool IsRetryableStatusCode(System.Net.HttpStatusCode statusCode)
    {
        return (int)statusCode >= 500 || statusCode == System.Net.HttpStatusCode.RequestTimeout;
    }

    /// <summary>
    /// Generates HMAC signature for webhook payload.
    /// Allows subscriber to verify request authenticity.
    /// </summary>
    private string GenerateSignature(WebhookPayload payload)
    {
        var json = JsonConvert.SerializeObject(payload);
        var hmac = Utilities.CryptographyHelper.GenerateHMACSHA256(json, "webhook-secret");
        return hmac;
    }

    /// <summary>
    /// Gets subscriptions for an event type.
    /// </summary>
    public List<WebhookSubscription> GetSubscriptions(string eventType)
    {
        if (string.IsNullOrEmpty(eventType))
            return new List<WebhookSubscription>();

        return _subscriptions.TryGetValue(eventType, out var subs) ? new List<WebhookSubscription>(subs) : new List<WebhookSubscription>();
    }
}

/// <summary>
/// Represents a webhook subscription.
/// </summary>
public class WebhookSubscription
{
    public string Id { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;
    public Dictionary<string, string> CustomHeaders { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? LastDeliveredAt { get; set; }
    public bool IsActive { get; set; } = true;
    public int DeliveryCount { get; set; }
    public int FailureCount { get; set; }
    public string? LastError { get; set; }
}

/// <summary>
/// Standard webhook payload format.
/// </summary>
public class WebhookPayload
{
    public string EventType { get; set; } = string.Empty;
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Data { get; set; } = new();
}

/// <summary>
/// Extension method to register WebhookHandler in dependency injection.
/// </summary>
public static class WebhookExtensions
{
    public static IServiceCollection AddWebhookHandler(this IServiceCollection services)
    {
        services.AddSingleton<WebhookHandler>();
        return services;
    }
}
