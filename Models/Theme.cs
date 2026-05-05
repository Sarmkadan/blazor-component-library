// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorComponentLibrary.Models;

/// <summary>
/// Defines a theme with colors, typography, and spacing configuration.
/// Supports light and dark variants with CSS variable generation.
/// </summary>
public class Theme
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("mode")]
    public ThemeMode Mode { get; set; } = ThemeMode.Light;

    [JsonPropertyName("primaryColor")]
    public string PrimaryColor { get; set; } = "#007bff";

    [JsonPropertyName("secondaryColor")]
    public string SecondaryColor { get; set; } = "#6c757d";

    [JsonPropertyName("successColor")]
    public string SuccessColor { get; set; } = "#28a745";

    [JsonPropertyName("dangerColor")]
    public string DangerColor { get; set; } = "#dc3545";

    [JsonPropertyName("warningColor")]
    public string WarningColor { get; set; } = "#ffc107";

    [JsonPropertyName("infoColor")]
    public string InfoColor { get; set; } = "#17a2b8";

    [JsonPropertyName("backgroundColor")]
    public string BackgroundColor { get; set; } = "#ffffff";

    [JsonPropertyName("textColor")]
    public string TextColor { get; set; } = "#212529";

    [JsonPropertyName("borderColor")]
    public string BorderColor { get; set; } = "#dee2e6";

    [JsonPropertyName("fontFamily")]
    public string FontFamily { get; set; } = "-apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', sans-serif";

    [Range(8, 32)]
    [JsonPropertyName("baseFontSize")]
    public int BaseFontSize { get; set; } = 14;

    [Range(1, 4)]
    [JsonPropertyName("lineHeight")]
    public double LineHeight { get; set; } = 1.5;

    [JsonPropertyName("borderRadius")]
    public string BorderRadius { get; set; } = "4px";

    [JsonPropertyName("shadowColor")]
    public string ShadowColor { get; set; } = "rgba(0, 0, 0, 0.1)";

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Generates CSS variables string for use in stylesheets.
    /// </summary>
    public string GenerateCssVariables()
    {
        var css = @$":root {{
  --color-primary: {PrimaryColor};
  --color-secondary: {SecondaryColor};
  --color-success: {SuccessColor};
  --color-danger: {DangerColor};
  --color-warning: {WarningColor};
  --color-info: {InfoColor};
  --color-background: {BackgroundColor};
  --color-text: {TextColor};
  --color-border: {BorderColor};
  --color-shadow: {ShadowColor};
  --font-family: {FontFamily};
  --font-size-base: {BaseFontSize}px;
  --line-height: {LineHeight};
  --border-radius: {BorderRadius};
}}";
        return css;
    }

    /// <summary>
    /// Gets a color by semantic name.
    /// </summary>
    public string GetColor(ColorSemantic semantic)
    {
        return semantic switch
        {
            ColorSemantic.Primary => PrimaryColor,
            ColorSemantic.Secondary => SecondaryColor,
            ColorSemantic.Success => SuccessColor,
            ColorSemantic.Danger => DangerColor,
            ColorSemantic.Warning => WarningColor,
            ColorSemantic.Info => InfoColor,
            ColorSemantic.Background => BackgroundColor,
            ColorSemantic.Text => TextColor,
            ColorSemantic.Border => BorderColor,
            _ => PrimaryColor
        };
    }

    /// <summary>
    /// Sets all color values at once from another theme.
    /// </summary>
    public void SetColorsFrom(Theme other)
    {
        PrimaryColor = other.PrimaryColor;
        SecondaryColor = other.SecondaryColor;
        SuccessColor = other.SuccessColor;
        DangerColor = other.DangerColor;
        WarningColor = other.WarningColor;
        InfoColor = other.InfoColor;
        BackgroundColor = other.BackgroundColor;
        TextColor = other.TextColor;
        BorderColor = other.BorderColor;
        ShadowColor = other.ShadowColor;
    }

    /// <summary>
    /// Validates the theme configuration.
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Name) &&
               !string.IsNullOrWhiteSpace(PrimaryColor) &&
               Name.Length >= 1 &&
               Name.Length <= 50 &&
               BaseFontSize >= 8 &&
               BaseFontSize <= 32 &&
               LineHeight >= 1 &&
               LineHeight <= 4;
    }

    /// <summary>
    /// Creates a copy of this theme with a new name.
    /// </summary>
    public Theme Clone(string newName)
    {
        return new Theme
        {
            Name = newName,
            Mode = Mode,
            PrimaryColor = PrimaryColor,
            SecondaryColor = SecondaryColor,
            SuccessColor = SuccessColor,
            DangerColor = DangerColor,
            WarningColor = WarningColor,
            InfoColor = InfoColor,
            BackgroundColor = BackgroundColor,
            TextColor = TextColor,
            BorderColor = BorderColor,
            FontFamily = FontFamily,
            BaseFontSize = BaseFontSize,
            LineHeight = LineHeight,
            BorderRadius = BorderRadius,
            ShadowColor = ShadowColor,
            IsActive = IsActive,
            CreatedAt = DateTime.UtcNow
        };
    }
}

public enum ThemeMode
{
    Light = 0,
    Dark = 1,
    Auto = 2
}

public enum ColorSemantic
{
    Primary,
    Secondary,
    Success,
    Danger,
    Warning,
    Info,
    Background,
    Text,
    Border
}
