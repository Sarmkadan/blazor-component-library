namespace BlazorComponentLibrary.Tests;

using System;
using System.IO;
using System.Text;
using System.Text.Json;

/// <summary>
/// System.Text.Json helpers for <see cref="ThemeSwitcherTests"/>.
/// </summary>
public static class ThemeSwitcherTestsJsonExtensions
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Serializes the test fixture to a JSON string.
    /// </summary>
    /// <param name="value">The fixture to serialize.</param>
    /// <param name="indented">When <c>true</c>, the output is pretty-printed.</param>
    /// <returns>The JSON representation of <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
    public static string ToJson(this ThemeSwitcherTests value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);

        var json = JsonSerializer.Serialize(value, Options);
        if (!indented)
        {
            return json;
        }

        // The cached options are compact, so indentation is applied by re-writing the parsed document.
        using var document = JsonDocument.Parse(json);
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
        {
            document.WriteTo(writer);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>
    /// Deserializes a <see cref="ThemeSwitcherTests"/> instance from a JSON string.
    /// </summary>
    /// <remarks>
    /// <see cref="ThemeSwitcherTests"/> derives from bUnit's <c>TestContext</c>, which is disposable.
    /// The caller owns the returned instance and should dispose it when finished.
    /// </remarks>
    /// <param name="json">The JSON text to deserialize.</param>
    /// <returns>The deserialized fixture, or <c>null</c> when the JSON literal is <c>null</c>.</returns>
    /// <exception cref="ArgumentException"><paramref name="json"/> is <c>null</c> or empty.</exception>
    /// <exception cref="JsonException"><paramref name="json"/> is not valid JSON for <see cref="ThemeSwitcherTests"/>.</exception>
    public static ThemeSwitcherTests? FromJson(string json)
    {
        ArgumentException.ThrowIfNullOrEmpty(json);

        return JsonSerializer.Deserialize<ThemeSwitcherTests>(json, Options);
    }

    /// <summary>
    /// Attempts to deserialize a <see cref="ThemeSwitcherTests"/> instance from a JSON string without throwing on malformed input.
    /// </summary>
    /// <remarks>
    /// On success the caller owns the returned instance and should dispose it, as with <see cref="FromJson(string)"/>.
    /// </remarks>
    /// <param name="json">The JSON text to deserialize.</param>
    /// <param name="value">The deserialized fixture when the method returns <c>true</c>; otherwise <c>null</c>.</param>
    /// <returns><c>true</c> if the JSON was parsed into a non-null instance; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentException"><paramref name="json"/> is <c>null</c> or empty.</exception>
    public static bool TryFromJson(string json, out ThemeSwitcherTests? value)
    {
        ArgumentException.ThrowIfNullOrEmpty(json);

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
