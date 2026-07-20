namespace BlazorComponentLibrary.Services;

/// <summary>Represents the available UI theme modes.</summary>
public enum ThemeMode
{
    /// <summary>Light colour scheme.</summary>
    Light,

    /// <summary>Dark colour scheme.</summary>
    Dark,

    /// <summary>Follow the OS / browser preference.</summary>
    System
}

/// <summary>
/// Service that manages the active UI theme and persists the user's preference
/// across page reloads via the browser's local storage.
/// </summary>
public interface IThemeService
{
    /// <summary>Gets the currently active theme mode.</summary>
    ThemeMode CurrentTheme { get; }

    /// <summary>Raised whenever the active theme changes.</summary>
    event Action<ThemeMode>? ThemeChanged;

    /// <summary>
    /// Raised when the theme changes, providing the string representation
    /// that is written to the <c>data-bcl-theme</c> attribute (e.g., "light", "dark", "auto").
    /// </summary>
    event Action<string>? OnThemeChanged;

    /// <summary>
    /// Changes the active theme and persists the selection to local storage.
    /// Sets a <c>data-bcl-theme</c> attribute on <c>document.documentElement</c>
    /// so CSS custom properties can respond to the change without a page reload.
    /// </summary>
    /// <param name="theme">The new theme mode to apply.</param>
    void SetTheme(ThemeMode theme);

    /// <summary>
    /// Reads the persisted theme preference from local storage and applies it.
    /// Call this once during application startup, e.g. inside
    /// <c>OnAfterRenderAsync(firstRender: true)</c> of the root layout.
    /// </summary>
    Task InitializeAsync();
}
