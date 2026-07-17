namespace BlazorComponentLibrary.Components.Modal;

using Microsoft.AspNetCore.Components;

/// <summary>
/// Provides extension methods for <see cref="Modal"/> components to enhance their functionality
/// with common modal operations and convenience methods.
/// </summary>
public static class ModalExtensions
{
    /// <summary>
    /// Shows the modal and returns a <see cref="Task"/> that can be awaited.
    /// </summary>
    /// <param name="modal">The modal instance to show.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="modal"/> is <see langword="null"/>.</exception>
    /// <exception cref="ModalException">Thrown when there is an error showing the modal.</exception>
    public static async Task ShowAsync(this Modal modal)
    {
        ArgumentNullException.ThrowIfNull(modal);
        await modal.Show();
    }

    /// <summary>
    /// Hides the modal and returns a <see cref="Task"/> that can be awaited.
    /// </summary>
    /// <param name="modal">The modal instance to hide.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="modal"/> is <see langword="null"/>.</exception>
    /// <exception cref="ModalException">Thrown when there is an error hiding the modal.</exception>
    public static async Task HideAsync(this Modal modal)
    {
        ArgumentNullException.ThrowIfNull(modal);
        await modal.Hide();
    }

    /// <summary>
    /// Toggles the visibility of the modal. Shows the modal if it's currently hidden,
    /// and hides it if it's currently visible.
    /// </summary>
    /// <param name="modal">The modal instance to toggle.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="modal"/> is <see langword="null"/>.</exception>
    /// <exception cref="ModalException">Thrown when there is an error toggling the modal.</exception>
    public static async Task ToggleAsync(this Modal modal)
    {
        ArgumentNullException.ThrowIfNull(modal);

        if (modal.IsVisible)
        {
            await modal.HideAsync();
        }
        else
        {
            await modal.ShowAsync();
        }
    }

    /// <summary>
    /// Shows the modal with a custom title, then restores the original title after hiding.
    /// Useful for temporary title changes like success/error messages.
    /// </summary>
    /// <param name="modal">The modal instance.</param>
    /// <param name="title">The temporary title to display.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="modal"/> or <paramref name="title"/> is <see langword="null"/>.</exception>
    /// <exception cref="ModalException">Thrown when there is an error showing or hiding the modal.</exception>
    public static async Task ShowWithTemporaryTitleAsync(this Modal modal, string title)
    {
        ArgumentNullException.ThrowIfNull(modal);
        ArgumentNullException.ThrowIfNull(title);

        var originalTitle = modal.Title;
        try
        {
            modal.Title = title;
            await modal.ShowAsync();
        }
        finally
        {
            modal.Title = originalTitle;
        }
    }

    /// <summary>
    /// Shows the modal and automatically hides it after the specified delay.
    /// Useful for auto-dismissing notifications or temporary dialogs.
    /// </summary>
    /// <param name="modal">The modal instance to show.</param>
    /// <param name="delayMilliseconds">The delay in milliseconds before automatically hiding the modal.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="modal"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="delayMilliseconds"/> is negative.</exception>
    /// <exception cref="ModalException">Thrown when there is an error showing or hiding the modal.</exception>
    public static async Task ShowWithAutoHideAsync(this Modal modal, int delayMilliseconds)
    {
        ArgumentNullException.ThrowIfNull(modal);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(delayMilliseconds);

        await modal.ShowAsync();
        await Task.Delay(delayMilliseconds);
        await modal.HideAsync();
    }

    /// <summary>
    /// Shows the modal and automatically hides it after the specified delay, then invokes the callback.
    /// Useful for showing temporary dialogs with completion callbacks.
    /// </summary>
    /// <param name="modal">The modal instance to show.</param>
    /// <param name="delayMilliseconds">The delay in milliseconds before automatically hiding the modal.</param>
    /// <param name="callback">The callback to invoke after the modal is hidden.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="modal"/> or <paramref name="callback"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="delayMilliseconds"/> is negative.</exception>
    /// <exception cref="ModalException">Thrown when there is an error showing or hiding the modal.</exception>
    public static async Task ShowWithAutoHideAndCallbackAsync(
        this Modal modal,
        int delayMilliseconds,
        Func<Task> callback)
    {
        ArgumentNullException.ThrowIfNull(modal);
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(delayMilliseconds);

        await modal.ShowAsync();
        await Task.Delay(delayMilliseconds);
        await modal.HideAsync();
        await callback();
    }

    /// <summary>
    /// Determines whether the modal is currently visible.
    /// </summary>
    /// <param name="modal">The modal instance to check.</param>
    /// <returns><see langword="true"/> if the modal is visible; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="modal"/> is <see langword="null"/>.</exception>
    public static bool IsVisible(this Modal modal)
    {
        ArgumentNullException.ThrowIfNull(modal);
        return modal.IsVisible;
    }

    /// <summary>
    /// Sets the footer content of the modal.
    /// </summary>
    /// <param name="modal">The modal instance.</param>
    /// <param name="footerContent">The footer content to set.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="modal"/> is <see langword="null"/>.</exception>
    public static void SetFooterContent(this Modal modal, RenderFragment footerContent)
    {
        ArgumentNullException.ThrowIfNull(modal);
        modal.FooterContent = footerContent;
    }

    /// <summary>
    /// Sets the child content of the modal.
    /// </summary>
    /// <param name="modal">The modal instance.</param>
    /// <param name="childContent">The child content to set.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="modal"/> is <see langword="null"/>.</exception>
    public static void SetChildContent(this Modal modal, RenderFragment childContent)
    {
        ArgumentNullException.ThrowIfNull(modal);
        modal.ChildContent = childContent;
    }

    /// <summary>
    /// Sets whether clicking the overlay should close the modal.
    /// </summary>
    /// <param name="modal">The modal instance.</param>
    /// <param name="closeOnOverlayClick">Whether to close on overlay click.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="modal"/> is <see langword="null"/>.</exception>
    public static void SetCloseOnOverlayClick(this Modal modal, bool closeOnOverlayClick)
    {
        ArgumentNullException.ThrowIfNull(modal);
        modal.CloseOnOverlayClick = closeOnOverlayClick;
    }

}