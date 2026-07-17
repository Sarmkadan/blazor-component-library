namespace BlazorComponentLibrary.Components.Toast;

using BlazorComponentLibrary.Services;

/// <summary>
/// Provides extension methods for <see cref="ToastContainer"/> to simplify common toast operations.
/// </summary>
public static class ToastContainerExtensions
{
    /// <summary>
    /// Shows a success toast notification that automatically dismisses after the default duration.
    /// </summary>
    /// <param name="container">The toast container instance.</param>
    /// <param name="message">The success message to display. Must not be empty.</param>
    /// <param name="durationMs">
    /// Auto-dismiss delay in milliseconds. Set to <c>0</c> to require manual dismissal.
    /// Defaults to 4000 ms.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="container"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="message"/> is <see langword="null"/> or empty.</exception>
    public static void ShowSuccess(this ToastContainer container, string message, int durationMs = 4000)
        => container.ToastService.Show(message, ToastType.Success, durationMs);

    /// <summary>
    /// Shows a warning toast notification that automatically dismisses after the default duration.
    /// </summary>
    /// <param name="container">The toast container instance.</param>
    /// <param name="message">The warning message to display. Must not be empty.</param>
    /// <param name="durationMs">
    /// Auto-dismiss delay in milliseconds. Set to <c>0</c> to require manual dismissal.
    /// Defaults to 4000 ms.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="container"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="message"/> is <see langword="null"/> or empty.</exception>
    public static void ShowWarning(this ToastContainer container, string message, int durationMs = 4000)
        => container.ToastService.Show(message, ToastType.Warning, durationMs);

    /// <summary>
    /// Shows an error toast notification that automatically dismisses after the default duration.
    /// </summary>
    /// <param name="container">The toast container instance.</param>
    /// <param name="message">The error message to display. Must not be empty.</param>
    /// <param name="durationMs">
    /// Auto-dismiss delay in milliseconds. Set to <c>0</c> to require manual dismissal.
    /// Defaults to 4000 ms.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="container"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="message"/> is <see langword="null"/> or empty.</exception>
    public static void ShowError(this ToastContainer container, string message, int durationMs = 4000)
        => container.ToastService.Show(message, ToastType.Error, durationMs);

    /// <summary>
    /// Shows an informational toast notification that requires manual dismissal.
    /// </summary>
    /// <param name="container">The toast container instance.</param>
    /// <param name="message">The informational message to display. Must not be empty.</param>
    /// <exception cref="ArgumentNullException"><paramref name="container"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="message"/> is <see langword="null"/> or empty.</exception>
    public static void ShowInfo(this ToastContainer container, string message)
        => container.ToastService.Show(message, ToastType.Info, 0);

    /// <summary>
    /// Dismisses all active toasts in the container.
    /// </summary>
    /// <param name="container">The toast container instance.</param>
    /// <exception cref="ArgumentNullException"><paramref name="container"/> is <see langword="null"/>.</exception>
    public static void DismissAll(this ToastContainer container)
        => container.ToastService.DismissAll();
}