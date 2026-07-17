namespace BlazorComponentLibrary.Components.ThemeSwitcher;

using System;
using System.Text.Json;

/// <summary>
/// Provides System.Text.Json serialization and deserialization extensions for <see cref="ThemeSwitcher"/> components.
/// </summary>
public static class ThemeSwitcherJsonExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    /// <summary>
    /// Serializes the theme switcher to a JSON string.
    /// </summary>
    /// <param name="value">The theme switcher instance to serialize.</param>
    /// <param name="indented">Whether to format the JSON with indentation for readability.</param>
    /// <returns>A JSON string representation of the theme switcher.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
    public static string ToJson(this ThemeSwitcher value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);

        var options = indented
            ? new JsonSerializerOptions(_jsonOptions) { WriteIndented = true }
            : _jsonOptions;

        return JsonSerializer.Serialize(value, options);
    }

    /// <summary>
    /// Deserializes a JSON string into a new <see cref="ThemeSwitcher"/> instance.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>A new theme switcher instance populated from the JSON, or null if deserialization fails.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="json"/> is null.</exception>
    public static ThemeSwitcher? FromJson(string json)
    {
        ArgumentNullException.ThrowIfNull(json);

        try
        {
            return JsonSerializer.Deserialize<ThemeSwitcher>(json, _jsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Attempts to deserialize a JSON string into a <see cref="ThemeSwitcher"/> instance.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="value">Receives the deserialized theme switcher instance if successful; otherwise, null.</param>
    /// <returns>True if deserialization succeeded; false if the JSON is invalid or deserialization failed.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="json"/> is null.</exception>
    public static bool TryFromJson(string json, out ThemeSwitcher? value)
    {
        ArgumentNullException.ThrowIfNull(json);

        try
        {
            value = FromJson(json);
            return value is not null;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }
}