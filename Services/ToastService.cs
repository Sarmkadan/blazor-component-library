namespace BlazorComponentLibrary.Services;

using BlazorComponentLibrary.Exceptions;
using Microsoft.Extensions.Logging;
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
    private readonly List<Timer> _timers = new();
    private readonly object _lock = new();

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
    public void Show(string message, ToastType type = ToastType.Info, int durationMs = 4000)
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

        var toast = new ToastMessage(Guid.NewGuid(), message, type, durationMs);

        lock (_lock)
        {
            _toasts.Add(toast);
        }

        _logger.LogInformation("Toast added successfully with ID: {ToastId}", toast.Id);
        ToastsChanged?.Invoke();

        if (durationMs > 0)
            ScheduleDismiss(toast.Id, durationMs);
    }

    /// <inheritdoc/>
    public void Dismiss(Guid id)
    {
        _logger.LogDebug("Dismissing toast with ID: {ToastId}", id);

        lock (_lock)
        {
            _toasts.RemoveAll(t => t.Id == id);
        }

        _logger.LogInformation("Toast dismissed successfully with ID: {ToastId}", id);
        ToastsChanged?.Invoke();
    }

    /// <inheritdoc/>
    public void DismissAll()
    {
        _logger.LogDebug("Dismissing all toasts");

        lock (_lock)
        {
            _toasts.Clear();
            foreach (var timer in _timers)
                timer.Dispose();
            _timers.Clear();
        }

        _logger.LogInformation("All toasts dismissed successfully");
        ToastsChanged?.Invoke();
    }

    private void ScheduleDismiss(Guid id, int durationMs)
    {
        try
        {
            var timer = new Timer(durationMs) { AutoReset = false };
            timer.Elapsed += (_, _) =>
            {
                try
                {
                    Dismiss(id);
                    lock (_lock) { _timers.Remove(timer); }
                }
                catch (ObjectDisposedException)
                {
                    // Timer or service is being disposed, ignore
                }
                catch (Exception ex)
                {
                    throw new ToastServiceException("Failed to dismiss toast", ex);
                }
            };

            lock (_lock) { _timers.Add(timer); }
            timer.Start();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to schedule toast dismissal");
            throw new ToastServiceException("Failed to schedule toast dismissal", ex);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (_lock)
        {
            foreach (var timer in _timers)
                timer.Dispose();
            _timers.Clear();
        }
    }
}
