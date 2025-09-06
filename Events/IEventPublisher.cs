// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

namespace BlazorComponentLibrary.Events;

/// <summary>
/// Interface for event publishing in a pub-sub pattern.
/// Allows components to broadcast events to interested subscribers.
/// Enables loose coupling between components through events.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes an event to all registered subscribers.
    /// Asynchronous operation with error handling.
    /// </summary>
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent;

    /// <summary>
    /// Publishes event and waits for all subscribers to complete.
    /// Ensures all handlers have processed the event.
    /// </summary>
    Task PublishAndWaitAsync<TEvent>(TEvent @event) where TEvent : IEvent;

    /// <summary>
    /// Checks if there are any subscribers for an event type.
    /// Useful for conditional event publishing.
    /// </summary>
    bool HasSubscribers<TEvent>() where TEvent : IEvent;
}

/// <summary>
/// Base interface for all events.
/// Provides common properties for tracking and routing.
/// </summary>
public interface IEvent
{
    /// <summary>
    /// Unique identifier for this event instance.
    /// Useful for tracking and correlation.
    /// </summary>
    string EventId { get; }

    /// <summary>
    /// Timestamp when event was created.
    /// Used for temporal ordering and analytics.
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Aggregate root ID associated with this event.
    /// Used for event sourcing and replay.
    /// </summary>
    string? AggregateId { get; }

    /// <summary>
    /// Event version for schema evolution.
    /// Allows handling multiple versions gracefully.
    /// </summary>
    int Version { get; }
}

/// <summary>
/// Base class for implementing events.
/// Provides standard event properties and initialization.
/// </summary>
public abstract class DomainEvent : IEvent
{
    public string EventId { get; protected set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public string? AggregateId { get; protected set; }
    public int Version { get; protected set; } = 1;

    protected DomainEvent() { }

    protected DomainEvent(string? aggregateId)
    {
        AggregateId = aggregateId;
    }
}

/// <summary>
/// Specific event raised when a component is created.
/// Useful for initialization and tracking lifecycle.
/// </summary>
public class ComponentCreatedEvent : DomainEvent
{
    public string ComponentName { get; set; } = string.Empty;
    public string ComponentType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public ComponentCreatedEvent() { }

    public ComponentCreatedEvent(string componentName, string componentType, string? aggregateId = null)
        : base(aggregateId)
    {
        ComponentName = componentName;
        ComponentType = componentType;
    }
}

/// <summary>
/// Specific event raised when a component is updated.
/// </summary>
public class ComponentUpdatedEvent : DomainEvent
{
    public string ComponentName { get; set; } = string.Empty;
    public Dictionary<string, object> Changes { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public ComponentUpdatedEvent() { }

    public ComponentUpdatedEvent(string componentName, Dictionary<string, object> changes, string? aggregateId = null)
        : base(aggregateId)
    {
        ComponentName = componentName;
        Changes = changes;
    }
}

/// <summary>
/// Specific event raised when a component is deleted.
/// </summary>
public class ComponentDeletedEvent : DomainEvent
{
    public string ComponentName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public ComponentDeletedEvent() { }

    public ComponentDeletedEvent(string componentName, string? aggregateId = null)
        : base(aggregateId)
    {
        ComponentName = componentName;
    }
}

/// <summary>
/// Specific event raised when form is submitted.
/// </summary>
public class FormSubmittedEvent : DomainEvent
{
    public int FormId { get; set; }
    public Dictionary<string, object> FormData { get; set; } = new();
    public string? UserId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public FormSubmittedEvent() { }

    public FormSubmittedEvent(int formId, Dictionary<string, object> formData, string? userId = null, string? aggregateId = null)
        : base(aggregateId)
    {
        FormId = formId;
        FormData = formData;
        UserId = userId;
    }
}

/// <summary>
/// Specific event raised when data is exported.
/// </summary>
public class DataExportedEvent : DomainEvent
{
    public string ExportType { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public int RecordCount { get; set; }
    public string? UserId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public DataExportedEvent() { }

    public DataExportedEvent(string exportType, string format, int recordCount, string? userId = null, string? aggregateId = null)
        : base(aggregateId)
    {
        ExportType = exportType;
        Format = format;
        RecordCount = recordCount;
        UserId = userId;
    }
}
