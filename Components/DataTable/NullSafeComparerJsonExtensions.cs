namespace BlazorComponentLibrary.Components.DataTable;

using System.Text.Json;
using System.Text.Json.Serialization;

public static class NullSafeComparerJsonExtensions
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    /// <summary>
    /// Serializes the NullSafeComparer instance to a JSON string.
    /// </summary>
    /// <param name="value">The NullSafeComparer instance to serialize.</param>
    /// <param name="indented">Whether to format the JSON with indentation.</param>
    /// <returns>The JSON string representation of the NullSafeComparer instance.</returns>
    public static string ToJson(this NullSafeComparer value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);
        return indented ? JsonSerializer.Serialize(value, _jsonSerializerOptions) : JsonSerializer.Serialize(value);
    }

    /// <summary>
    /// Deserializes a JSON string to a NullSafeComparer instance.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The deserialized NullSafeComparer instance, or null if the JSON is invalid.</returns>
    public static NullSafeComparer? FromJson(string json)
    {
        ArgumentNullException.ThrowIfNull(json);
        try
        {
            return JsonSerializer.Deserialize<NullSafeComparer>(json, _jsonSerializerOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Attempts to deserialize a JSON string to a NullSafeComparer instance.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="value">The deserialized NullSafeComparer instance, or null if the JSON is invalid.</param>
    /// <returns>True if the JSON was successfully deserialized, false otherwise.</returns>
    public static bool TryFromJson(string json, out NullSafeComparer? value)
    {
        ArgumentNullException.ThrowIfNull(json);
        try
        {
            value = JsonSerializer.Deserialize<NullSafeComparer>(json, _jsonSerializerOptions);
            return true;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }
}
