// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

namespace BlazorComponentLibrary.Events;

/// <summary>
/// In-memory event bus implementation.
/// Manages subscriptions and publishes events to subscribers.
/// Thread-safe implementation using concurrent collections.
/// </summary>
public class EventBus : IEventPublisher, IDisposable
{
    private readonly Dictionary<Type, List<Delegate>> _subscribers = new();
    private readonly ReaderWriterLockSlim _lock = new();
    private readonly ILogger<EventBus> _logger;

    public EventBus(ILogger<EventBus> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Subscribes a handler to an event type.
    /// Handler will be called whenever an event of that type is published.
    /// </summary>
    public void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        _lock.EnterWriteLock();
        try
        {
            var eventType = typeof(TEvent);
            if (!_subscribers.ContainsKey(eventType))
            {
                _subscribers[eventType] = new List<Delegate>();
            }

            _subscribers[eventType].Add(handler);
            _logger.LogInformation("Handler subscribed to {EventType}", eventType.Name);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Unsubscribes a handler from an event type.
    /// Handler will no longer be called for that event type.
    /// </summary>
    public void Unsubscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        _lock.EnterWriteLock();
        try
        {
            var eventType = typeof(TEvent);
            if (_subscribers.ContainsKey(eventType))
            {
                _subscribers[eventType].Remove(handler);
                _logger.LogInformation("Handler unsubscribed from {EventType}", eventType.Name);

                if (_subscribers[eventType].Count == 0)
                {
                    _subscribers.Remove(eventType);
                }
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Publishes an event asynchronously.
    /// Invokes all registered handlers in parallel.
    /// Does not wait for handlers to complete.
    /// </summary>
    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        _logger.LogInformation("Publishing event {EventType} with ID {EventId}", typeof(TEvent).Name, @event.EventId);

        var handlers = GetHandlers<TEvent>();

        if (handlers.Count == 0)
        {
            _logger.LogWarning("No subscribers for event {EventType}", typeof(TEvent).Name);
            return;
        }

        var tasks = handlers.Select(handler =>
        {
            try
            {
                return handler(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in event handler for {EventType}", typeof(TEvent).Name);
                return Task.CompletedTask;
            }
        });

        // Fire and forget - continue without waiting
        _ = Task.WhenAll(tasks).ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                _logger.LogError(t.Exception, "Error during event publication {EventType}", typeof(TEvent).Name);
            }
        });

        await Task.CompletedTask;
    }

    /// <summary>
    /// Publishes an event and waits for all handlers to complete.
    /// Ensures consistent state after all handlers have executed.
    /// </summary>
    public async Task PublishAndWaitAsync<TEvent>(TEvent @event) where TEvent : IEvent
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        _logger.LogInformation("Publishing event (with wait) {EventType} with ID {EventId}",
            typeof(TEvent).Name, @event.EventId);

        var handlers = GetHandlers<TEvent>();

        if (handlers.Count == 0)
        {
            _logger.LogWarning("No subscribers for event {EventType}", typeof(TEvent).Name);
            return;
        }

        var tasks = handlers.Select(handler =>
        {
            try
            {
                return handler(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in event handler for {EventType}", typeof(TEvent).Name);
                return Task.CompletedTask;
            }
        });

        try
        {
            await Task.WhenAll(tasks);
            _logger.LogInformation("Event {EventType} processed by all handlers", typeof(TEvent).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during event publication {EventType}", typeof(TEvent).Name);
            throw;
        }
    }

    /// <summary>
    /// Checks if there are any subscribers for an event type.
    /// </summary>
    public bool HasSubscribers<TEvent>() where TEvent : IEvent
    {
        _lock.EnterReadLock();
        try
        {
            return _subscribers.ContainsKey(typeof(TEvent)) && _subscribers[typeof(TEvent)].Count > 0;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <summary>
    /// Gets subscriber count for an event type.
    /// Useful for monitoring and debugging.
    /// </summary>
    public int GetSubscriberCount<TEvent>() where TEvent : IEvent
    {
        _lock.EnterReadLock();
        try
        {
            return _subscribers.ContainsKey(typeof(TEvent)) ? _subscribers[typeof(TEvent)].Count : 0;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <summary>
    /// Clears all subscribers for an event type.
    /// Use with caution - breaks all subscriptions.
    /// </summary>
    public void ClearSubscribers<TEvent>() where TEvent : IEvent
    {
        _lock.EnterWriteLock();
        try
        {
            _subscribers.Remove(typeof(TEvent));
            _logger.LogInformation("Cleared all subscribers for {EventType}", typeof(TEvent).Name);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Gets all handlers for an event type.
    /// Safely retrieves handlers under read lock.
    /// </summary>
    private List<Func<TEvent, Task>> GetHandlers<TEvent>() where TEvent : IEvent
    {
        _lock.EnterReadLock();
        try
        {
            var eventType = typeof(TEvent);
            if (_subscribers.ContainsKey(eventType))
            {
                return _subscribers[eventType].Cast<Func<TEvent, Task>>().ToList();
            }
            return new List<Func<TEvent, Task>>();
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <summary>
    /// Cleans up resources.
    /// Clears all subscriptions and disposes lock.
    /// </summary>
    public void Dispose()
    {
        _lock.Dispose();
        _subscribers.Clear();
        _logger.LogInformation("EventBus disposed");
    }
}

/// <summary>
/// Extension method to register EventBus in dependency injection.
/// </summary>
public static class EventBusExtensions
{
    public static IServiceCollection AddEventBus(this IServiceCollection services)
    {
        services.AddSingleton<IEventPublisher, EventBus>();
        return services;
    }
}
