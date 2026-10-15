using BlazorComponentLibrary.Components.Toast;

namespace BlazorComponentLibrary.Services;

/// <summary>
/// Configuration for the toast system, supplied through <c>AddBlazorComponentLibrary</c>.
/// The options are validated when the host starts by <see cref="ToastOptionsValidator"/>, so an invalid
/// configuration fails fast with an <see cref="Microsoft.Extensions.Options.OptionsValidationException"/>.
/// </summary>
public sealed class ToastOptions
{
    /// <summary>
    /// Gets or sets the auto-dismiss delay used when a toast is shown without an explicit duration.
    /// Must be between 500 ms and 60 s inclusive. The default is 4 seconds.
    /// </summary>
    public TimeSpan DefaultDuration { get; set; } = TimeSpan.FromSeconds(4);

    /// <summary>
    /// Gets or sets the maximum number of toasts rendered at once by <c>ToastContainer</c>.
    /// Must be between 1 and 20 inclusive. The default is <c>5</c>.
    /// </summary>
    public int MaxVisibleToasts { get; set; } = 5;

    /// <summary>
    /// Gets or sets the screen corner where <c>ToastContainer</c> stacks toasts when no
    /// <c>Position</c> parameter is supplied. Must be a defined <see cref="ToastPosition"/> value.
    /// The default is <see cref="ToastPosition.BottomRight"/>.
    /// </summary>
    public ToastPosition Position { get; set; } = ToastPosition.BottomRight;

    /// <summary>
    /// Gets or sets whether hovering over a toast pauses its auto-dismiss timer when no
    /// <c>PauseOnHover</c> parameter is supplied. The default is <c>true</c>.
    /// </summary>
    public bool PauseOnHover { get; set; } = true;
}
