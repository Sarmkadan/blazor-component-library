namespace BlazorComponentLibrary.Components.Toast;

/// <summary>Defines the screen corner where the toast stack is anchored.</summary>
public enum ToastPosition
{
    TopLeft,
    TopCenter,
    TopRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}

/// <summary>Contract for the toast container component that renders active notifications.</summary>
public interface IToastContainer
{
    /// <summary>
    /// Gets or sets the screen position where toasts are stacked.
    /// Defaults to <see cref="ToastPosition.BottomRight"/>.
    /// </summary>
    ToastPosition Position { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of toasts visible simultaneously.
    /// When the queue exceeds this limit the oldest toasts are hidden until
    /// newer ones dismiss. Defaults to <c>5</c>.
    /// </summary>
    int MaxVisible { get; set; }

    /// <summary>
    /// Gets or sets whether hovering over a toast pauses its auto-dismiss timer.
    /// Defaults to <c>true</c>.
    /// </summary>
    bool PauseOnHover { get; set; }

    /// <summary>
    /// Gets or sets whether to deduplicate toasts with the same message.
    /// When enabled, consecutive identical messages are combined into a single toast
    /// with a counter badge showing the count. Defaults to <c>false</c>.
    /// </summary>
    bool Dedup { get; set; }
}
