namespace BlazorComponentLibrary.Components.DataTable;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Provides System.Text.Json serialization extensions for <see cref="NullSafeComparer"/>.
/// </summary>
public static class NullSafeComparerJsonExtensions
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    /// <summary>
    /// Serializes the <see cref="NullSafeComparer"/> type information to a JSON string.
    /// Since <see cref="NullSafeComparer"/> is a stateless singleton, this method returns a JSON representation
    /// containing the type information that can be used to recreate the comparer type.
    /// </summary>
    /// <param name="value">The NullSafeComparer instance to serialize. Must be <see cref="NullSafeComparer.Instance"/>.</param>
    /// <param name="indented">Whether to format the JSON with indentation for readability.</param>
    /// <returns>A JSON string representation containing the comparer type information.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is not <see cref="NullSafeComparer.Instance"/>.</exception>
    public static string ToJson(this NullSafeComparer value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!ReferenceEquals(value, NullSafeComparer.Instance))
        {
            throw new ArgumentException(
                "Only NullSafeComparer.Instance can be serialized. NullSafeComparer is a stateless singleton.",
                nameof(value));
        }

        var options = new JsonSerializerOptions(_jsonSerializerOptions)
        {
            WriteIndented = indented
        };

        return JsonSerializer.Serialize(new { Type = "NullSafeComparer", Version = "1.0" }, options);
    }

    /// <summary>
    /// Deserializes a JSON string to a <see cref="NullSafeComparer"/> instance.
    /// Since <see cref="NullSafeComparer"/> is a stateless singleton, this method always returns <see cref="NullSafeComparer.Instance"/>.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The <see cref="NullSafeComparer.Instance"/> singleton, or null if the JSON is null, empty, or whitespace.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is null.</exception>
    public static NullSafeComparer? FromJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        ArgumentNullException.ThrowIfNull(json);

        try
        {
            // Deserialize and discard - we always return the singleton instance
            JsonSerializer.Deserialize<object>(json, _jsonSerializerOptions);
            return NullSafeComparer.Instance;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Attempts to deserialize a JSON string to a <see cref="NullSafeComparer"/> instance.
    /// Since <see cref="NullSafeComparer"/> is a stateless singleton, this method always returns <see cref="NullSafeComparer.Instance"/> if deserialization succeeds.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="value">Receives the <see cref="NullSafeComparer.Instance"/> singleton if deserialization succeeds.</param>
    /// <returns><see langword="true"/> if the JSON is valid; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is null.</exception>
    public static bool TryFromJson(string json, out NullSafeComparer? value)
    {
        ArgumentNullException.ThrowIfNull(json);

        try
        {
            // Attempt to validate JSON by deserializing to a dummy object
            JsonSerializer.Deserialize<object>(json, _jsonSerializerOptions);
            value = NullSafeComparer.Instance;
            return true;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }
}