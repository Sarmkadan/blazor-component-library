namespace BlazorComponentLibrary.Services;

/// <summary>Classifies the visual style and semantic meaning of a toast notification.</summary>
public enum ToastType
{
    /// <summary>Informational message with no urgency.</summary>
    Info,

    /// <summary>Operation completed successfully.</summary>
    Success,

    /// <summary>Non-critical warning the user should be aware of.</summary>
    Warning,

    /// <summary>An error has occurred and requires attention.</summary>
    Error
}

/// <summary>Represents a single toast notification message.</summary>
/// <param name="Id">Unique identifier used to target the toast for dismissal.</param>
/// <param name="Message">The text content displayed in the toast.</param>
/// <param name="Type">The visual type that controls the toast's icon and accent colour.</param>
/// <param name="DurationMs">
/// Auto-dismiss delay in milliseconds. Set to <c>0</c> to require manual dismissal.
/// </param>
/// <param name="Icon">
/// The icon to display for this toast. If null, the default icon for the toast type will be used.
/// </param>
public sealed record ToastMessage(Guid Id, string Message, ToastType Type, int DurationMs, string? Icon = null);

/// <summary>Service for queuing and managing toast notification messages.</summary>
public interface IToastService
{
    /// <summary>Gets the currently active toast messages in display order (oldest first).</summary>
    IReadOnlyList<ToastMessage> ActiveToasts { get; }

    /// <summary>Raised whenever a toast is added or removed.</summary>
    event Action? ToastsChanged;

    /// <summary>
    /// Adds a new toast notification. If <paramref name="durationMs"/> is greater than
    /// zero the toast is automatically dismissed after the specified delay.
    /// </summary>
    /// <param name="message">The message text to display. Must not be empty.</param>
    /// <param name="type">The toast type. Defaults to <see cref="ToastType.Info"/>.</param>
    /// <param name="durationMs">
    /// Auto-dismiss delay in milliseconds. Defaults to 4 000 ms. Use <c>0</c> to
    /// require the user to manually dismiss the notification.
    /// </param>
    void Show(string message, ToastType type = ToastType.Info, int durationMs = 4000, string? icon = null);

    /// <summary>Removes the toast with the specified identifier.</summary>
    /// <param name="id">The unique identifier of the toast to dismiss.</param>
    void Dismiss(Guid id);

    /// <summary>Removes all active toasts immediately.</summary>
    void DismissAll();

    /// <summary>
    /// Pauses the auto-dismiss timer for a specific toast.
    /// </summary>
    /// <param name="id">The ID of the toast to pause.</param>
    void PauseTimer(Guid id);

    /// <summary>
    /// Resumes the auto-dismiss timer for a specific toast.
    /// </summary>
    /// <param name="id">The ID of the toast to resume.</param>
    /// <param name="remainingMs">The remaining time in milliseconds.</param>
    void ResumeTimer(Guid id, double remainingMs);
}
