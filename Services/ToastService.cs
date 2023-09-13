namespace BlazorComponentLibrary.Services;

using BlazorComponentLibrary.Exceptions;
using System.Timers;

/// <summary>
/// Default implementation of <see cref="IToastService"/>.
/// Each toast with a positive <c>DurationMs</c> value is automatically dismissed via a
/// <see cref="System.Timers.Timer"/> so the calling component does not need to manage
/// any async lifecycle state.
/// </summary>
public sealed class ToastService : IToastService, IDisposable
{
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
    /// <exception cref="ToastServiceException">Thrown when the message is null or whitespace.</exception>
    public void Show(string message, ToastType type = ToastType.Info, int durationMs = 4000)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ToastServiceException("Toast message must not be empty.");

        var toast = new ToastMessage(Guid.NewGuid(), message, type, durationMs);

        lock (_lock)
        {
            _toasts.Add(toast);
        }

        ToastsChanged?.Invoke();

        if (durationMs > 0)
            ScheduleDismiss(toast.Id, durationMs);
    }

    /// <inheritdoc/>
    public void Dismiss(Guid id)
    {
        lock (_lock)
        {
            _toasts.RemoveAll(t => t.Id == id);
        }

        ToastsChanged?.Invoke();
    }

    /// <inheritdoc/>
    public void DismissAll()
    {
        lock (_lock)
        {
            _toasts.Clear();
            foreach (var timer in _timers)
                timer.Dispose();
            _timers.Clear();
        }

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
