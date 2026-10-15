namespace BlazorComponentLibrary.Services;

/// <summary>
/// Configuration for the theme system, supplied through
/// <c>AddBlazorComponentLibraryTheme</c>. The options are validated at host start by
/// <see cref="ThemeOptionsValidator"/>, so an invalid configuration fails fast with an
/// <see cref="Microsoft.Extensions.Options.OptionsValidationException"/>.
/// </summary>
public sealed class ThemeOptions
{
    /// <summary>
    /// Gets or sets the name of the theme applied by default. It must match one of the keys in
    /// <see cref="Themes"/> (case-insensitive). The default is <c>"light"</c>.
    /// </summary>
    public string DefaultTheme { get; set; } = "light";

    /// <summary>
    /// Gets or sets the registered themes, keyed by theme name (case-insensitive).
    /// Each theme maps CSS custom property names (which must start with <c>--</c>) to
    /// colour values written as CSS hex (<c>#rgb</c>, <c>#rgba</c>, <c>#rrggbb</c>, <c>#rrggbbaa</c>)
    /// or <c>rgb()</c>/<c>rgba()</c> strings.
    /// By default the <c>light</c> and <c>dark</c> themes are registered without custom variables.
    /// </summary>
    public Dictionary<string, ThemeDefinition> Themes { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ["light"] = new ThemeDefinition(),
        ["dark"] = new ThemeDefinition(),
    };

    /// <summary>
    /// Gets or sets a value indicating whether the selected theme is persisted to browser storage.
    /// The default is <c>true</c>.
    /// </summary>
    public bool EnablePersistence { get; set; } = true;

    /// <summary>
    /// Gets or sets the browser storage key under which the selected theme is persisted.
    /// Must be non-empty when <see cref="EnablePersistence"/> is <c>true</c>. The default is <c>"bcl-theme"</c>.
    /// </summary>
    public string StorageKey { get; set; } = "bcl-theme";
}

/// <summary>
/// A named theme and the CSS custom properties it defines.
/// </summary>
public sealed class ThemeDefinition
{
    /// <summary>
    /// Gets or sets the CSS custom properties for this theme, keyed by property name
    /// (for example <c>--bcl-primary</c>) with colour values such as <c>#1e90ff</c> or <c>rgb(30, 144, 255)</c>.
    /// </summary>
    public Dictionary<string, string> CssVariables { get; set; } = new(StringComparer.Ordinal);
}
