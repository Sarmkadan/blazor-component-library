namespace BlazorComponentLibrary.Services;

using BlazorComponentLibrary.Exceptions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;
using System.Timers;

/// <summary>
/// Default implementation of <see cref="IToastService"/>.
/// Each toast with a positive <c>DurationMs</c> value is automatically dismissed via a
/// <see cref="System.Timers.Timer"/> so the calling component does not need to manage
/// any async lifecycle state.
/// </summary>
public sealed class ToastService : IToastService, IDisposable
{
    private readonly ILogger<ToastService> _logger;
    private readonly List<ToastMessage> _toasts = new();
    private readonly Dictionary<Guid, Timer> _timers = new();
    private readonly ConcurrentDictionary<string, Guid> _dedupCache = new();
    private readonly object _lock = new();
    private readonly object _eventLock = new();

    /// <summary>Initialises a new instance of <see cref="ToastService"/>.</summary>
    /// <param name="logger">
    /// Optional logger. When omitted (e.g. in unit tests) a no-op logger is used.
    /// </param>
    public ToastService(ILogger<ToastService>? logger = null)
    {
        _logger = logger ?? NullLogger<ToastService>.Instance;
    }

    /// <inheritdoc/>
    public IReadOnlyList<ToastMessage> ActiveToasts
    {
        get { lock (_lock) return _toasts.AsReadOnly(); }
    }

    /// <inheritdoc/>
    public event Action? ToastsChanged;

    /// <summary>
    /// Gets or sets whether to deduplicate toasts with the same message.
    /// When enabled, consecutive identical messages are combined into a single toast
    /// with a counter badge showing the count.
    /// </summary>
    public bool Dedup { get; set; } = false;

    /// <inheritdoc/>
    /// <exception cref="ToastServiceException">Thrown when the message is null or whitespace,
    /// or when <paramref name="durationMs"/> is negative.</exception>
    public void Show(string message, ToastType type = ToastType.Info, int durationMs = 4000, string? icon = null)
    {
        _logger.LogDebug("Showing toast with message: '{Message}', type: {ToastType}, duration: {DurationMs}ms", message, type, durationMs);

        if (string.IsNullOrWhiteSpace(message))
        {
            _logger.LogWarning("Toast message is null or whitespace");
            throw new ToastServiceException("Toast message must not be empty.");
        }

        if (durationMs < 0)
        {
            _logger.LogWarning("Toast duration cannot be negative: {DurationMs}", durationMs);
            throw new ToastServiceException("DurationMs cannot be negative.");
        }

        Guid toastId;
        ToastMessage? existingToast = null;

        lock (_lock)
        {
            // Handle deduplication if enabled
            if (Dedup)
            {
                var messageKey = message.Trim();
                if (_dedupCache.TryGetValue(messageKey, out var cachedId))
                {
                    // Find the existing toast with this ID
                    existingToast = _toasts.FirstOrDefault(t => t.Id == cachedId);
                    if (existingToast != null)
                    {
                        // Update the existing toast's count and reset its timer
                        var updatedToast = existingToast with { Count = existingToast.Count + 1 };
                        _toasts.Remove(existingToast);
                        _toasts.Add(updatedToast);
                        toastId = updatedToast.Id;

                        // Update the timer with the new duration
                        if (durationMs > 0 && _timers.TryGetValue(toastId, out var existingTimer))
                        {
                            existingTimer.Stop();
                            existingTimer.Interval = durationMs;
                            existingTimer.Start();
                        }
                        else if (durationMs > 0)
                        {
                            ScheduleDismiss(toastId, durationMs);
                        }

                        _logger.LogInformation("Deduplicated toast updated with ID: {ToastId}, new count: {Count}", toastId, updatedToast.Count);
                        InvokeToastsChanged();
                        return;
                    }
                }
                else
                {
                    // Add to cache for future deduplication
                    _dedupCache[messageKey] = Guid.Empty; // Placeholder, will be updated below
                }
            }

            // Create new toast
            toastId = Guid.NewGuid();
            var toast = new ToastMessage(toastId, message, type, durationMs, icon);
            _toasts.Add(toast);

            // Update cache with actual ID
            if (Dedup)
            {
                _dedupCache[message.Trim()] = toastId;
            }
        }

        _logger.LogInformation("Toast added successfully with ID: {ToastId}", toastId);
        InvokeToastsChanged();

        if (durationMs > 0)
            ScheduleDismiss(toastId, durationMs);
    }

    /// <inheritdoc/>
    public void Dismiss(Guid id)
    {
        _logger.LogDebug("Dismissing toast with ID: {ToastId}", id);

        bool removed;
        string? dismissedMessage = null;
        lock (_lock)
        {
            var toastToRemove = _toasts.FirstOrDefault(t => t.Id == id);
            if (toastToRemove != null)
            {
                dismissedMessage = toastToRemove.Message.Trim();
            }
            removed = _toasts.RemoveAll(t => t.Id == id) > 0;
            if (_timers.Remove(id, out var timer))
                timer.Dispose();
        }

        if (!removed)
            return;

        // Clean up deduplication cache if this was the last instance of a message
        if (Dedup && dismissedMessage != null)
        {
            // Only remove from cache if no other toasts with this message exist
            lock (_lock)
            {
                if (!_toasts.Any(t => t.Message.Trim() == dismissedMessage))
                {
                    _dedupCache.TryRemove(dismissedMessage, out _);
                }
            }
        }

        _logger.LogInformation("Toast dismissed successfully with ID: {ToastId}", id);
        InvokeToastsChanged();
    }

    /// <inheritdoc/>
    public void DismissAll()
    {
        _logger.LogDebug("Dismissing all toasts");

        lock (_lock)
        {
            _toasts.Clear();
            DisposeTimers();
            if (Dedup)
            {
                _dedupCache.Clear();
            }
        }

        _logger.LogInformation("All toasts dismissed successfully");
        InvokeToastsChanged();
    }

    private void ScheduleDismiss(Guid id, int durationMs)
    {
        var timer = new Timer(durationMs) { AutoReset = false };
        timer.Elapsed += (_, _) =>
        {
            // System.Timers.Timer silently swallows exceptions thrown from Elapsed,
            // so failures must be logged here rather than rethrown.
            try
            {
                Dismiss(id);
            }
            catch (ObjectDisposedException)
            {
                // Timer or service is being disposed, ignore
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to auto-dismiss toast with ID: {ToastId}", id);
            }
        };

        lock (_lock) { _timers[id] = timer; }
        timer.Start();
    }

    /// <summary>
    /// Pauses the auto-dismiss timer for a specific toast.
    /// </summary>
    /// <param name="id">The ID of the toast to pause.</param>
    public void PauseTimer(Guid id)
    {
        lock (_lock)
        {
            if (_timers.TryGetValue(id, out var timer))
            {
                timer.Stop();
                _logger.LogDebug("Paused toast timer for ID: {ToastId}", id);
            }
        }
    }

    /// <summary>
    /// Resumes the auto-dismiss timer for a specific toast.
    /// </summary>
    /// <param name="id">The ID of the toast to resume.</param>
    /// <param name="remainingMs">The remaining time in milliseconds.</param>
    public void ResumeTimer(Guid id, double remainingMs)
    {
        lock (_lock)
        {
            if (_timers.TryGetValue(id, out var timer))
            {
                timer.Interval = remainingMs;
                timer.Start();
                _logger.LogDebug("Resumed toast timer for ID: {ToastId} with {RemainingMs}ms remaining", id, remainingMs);
            }
        }
    }

    private void InvokeToastsChanged()
    {
        lock (_eventLock)
        {
            ToastsChanged?.Invoke();
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (_lock)
        {
            DisposeTimers();
        }
    }

    private void DisposeTimers()
    {
        foreach (var timer in _timers.Values)
            timer.Dispose();
        _timers.Clear();
    }
}
