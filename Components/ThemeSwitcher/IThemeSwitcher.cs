namespace BlazorComponentLibrary.Components.ThemeSwitcher;

/// <summary>Contract for the theme-switcher toggle component.</summary>
public interface IThemeSwitcher
{
    /// <summary>
    /// Gets or sets whether a text label is rendered beside each theme icon.
    /// Defaults to <c>true</c>.
    /// </summary>
    bool ShowLabel { get; set; }

    /// <summary>
    /// Gets or sets optional CSS class(es) to append to the root element,
    /// allowing consumers to customise layout without overriding library styles.
    /// </summary>
    string? CssClass { get; set; }
}
