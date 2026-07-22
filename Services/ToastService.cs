namespace BlazorComponentLibrary.Services;

using BlazorComponentLibrary.Exceptions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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

        var toast = new ToastMessage(Guid.NewGuid(), message, type, durationMs, icon);

        lock (_lock)
        {
            _toasts.Add(toast);
        }

        _logger.LogInformation("Toast added successfully with ID: {ToastId}", toast.Id);
        InvokeToastsChanged();

        if (durationMs > 0)
            ScheduleDismiss(toast.Id, durationMs);
    }

    /// <inheritdoc/>
    public void Dismiss(Guid id)
    {
        _logger.LogDebug("Dismissing toast with ID: {ToastId}", id);

        bool removed;
        lock (_lock)
        {
            removed = _toasts.RemoveAll(t => t.Id == id) > 0;
            if (_timers.Remove(id, out var timer))
                timer.Dispose();
        }

        if (!removed)
            return;

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
