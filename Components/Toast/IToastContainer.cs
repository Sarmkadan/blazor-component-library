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
}
