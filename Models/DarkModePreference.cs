// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorComponentLibrary.Models;

/// <summary>
/// Stores a user's dark mode preference together with optional color overrides.
/// Color overrides are applied via the <c>[data-theme="dark"]</c> CSS selector
/// so they layer on top of the active base theme without replacing it.
/// </summary>
public class DarkModePreference
{
    [Key]
    public int Id { get; set; }

    [Required]
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>Whether dark mode is currently enabled for this user.</summary>
    [JsonPropertyName("isDarkMode")]
    public bool IsDarkMode { get; set; }

    /// <summary>
    /// When <c>true</c> the client should honour the OS-level
    /// <c>prefers-color-scheme</c> media query instead of <see cref="IsDarkMode"/>.
    /// </summary>
    [JsonPropertyName("followSystemPreference")]
    public bool FollowSystemPreference { get; set; } = true;

    [JsonPropertyName("darkBackgroundColor")]
    public string DarkBackgroundColor { get; set; } = "#121212";

    [JsonPropertyName("darkSurfaceColor")]
    public string DarkSurfaceColor { get; set; } = "#1e1e1e";

    [JsonPropertyName("darkElevatedSurfaceColor")]
    public string DarkElevatedSurfaceColor { get; set; } = "#2d2d2d";

    [JsonPropertyName("darkTextColor")]
    public string DarkTextColor { get; set; } = "#e0e0e0";

    [JsonPropertyName("darkTextSecondaryColor")]
    public string DarkTextSecondaryColor { get; set; } = "#a0a0a0";

    [JsonPropertyName("darkBorderColor")]
    public string DarkBorderColor { get; set; } = "#3a3a3a";

    [JsonPropertyName("darkShadowColor")]
    public string DarkShadowColor { get; set; } = "rgba(0, 0, 0, 0.4)";

    /// <summary>Transition animation duration in milliseconds.</summary>
    [Range(0, 2000)]
    [JsonPropertyName("transitionDurationMs")]
    public int TransitionDurationMs { get; set; } = 200;

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Generates the CSS variable overrides for dark mode.
    /// Applied under the <c>[data-theme="dark"]</c> attribute selector so the
    /// base theme's accent colours (primary, success, etc.) are inherited unchanged.
    /// </summary>
    public string GenerateDarkCssVariables()
    {
        return @$"[data-theme=""dark""] {{
  --color-background: {DarkBackgroundColor};
  --color-surface: {DarkSurfaceColor};
  --color-surface-elevated: {DarkElevatedSurfaceColor};
  --color-text: {DarkTextColor};
  --color-text-secondary: {DarkTextSecondaryColor};
  --color-border: {DarkBorderColor};
  --color-shadow: {DarkShadowColor};
  --theme-transition: {TransitionDurationMs}ms;
}}";
    }

    /// <summary>
    /// Generates a CSS rule that smoothly animates background, text, and border
    /// colours whenever the theme attribute is toggled.
    /// </summary>
    public string GenerateTransitionCss()
    {
        return @$"*, *::before, *::after {{
  transition: background-color {TransitionDurationMs}ms ease,
              color {TransitionDurationMs}ms ease,
              border-color {TransitionDurationMs}ms ease;
}}";
    }

    /// <summary>
    /// Validates that the preference holds a non-empty user identifier and a
    /// transition duration within the allowed range.
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(UserId) &&
               TransitionDurationMs >= 0 &&
               TransitionDurationMs <= 2000;
    }
}
