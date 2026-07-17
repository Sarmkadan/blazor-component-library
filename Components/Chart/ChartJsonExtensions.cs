namespace BlazorComponentLibrary.Components.Chart;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Provides JSON serialization and deserialization extensions for <see cref="Chart{TData}"/> components.
/// </summary>
public static class ChartJsonExtensions
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Serializes a chart to a JSON string.
    /// </summary>
    /// <typeparam name="TData">The type of data in the chart.</typeparam>
    /// <param name="value">The chart to serialize.</param>
    /// <param name="indented">Whether to format the JSON with indentation for readability.</param>
    /// <returns>A JSON string representation of the chart.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    public static string ToJson<TData>(this Chart<TData> value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);

        var options = indented
            ? new JsonSerializerOptions(_jsonSerializerOptions) { WriteIndented = true }
            : _jsonSerializerOptions;

        return JsonSerializer.Serialize(value, options);
    }

    /// <summary>
    /// Deserializes a chart from a JSON string.
    /// </summary>
    /// <typeparam name="TData">The type of data in the chart.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The deserialized chart, or <see langword="null"/> if the JSON is invalid.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="json"/> is empty.</exception>
    public static Chart<TData>? FromJson<TData>(string json)
    {
        ArgumentNullException.ThrowIfNull(json);
        ArgumentException.ThrowIfNullOrEmpty(json);

        try
        {
            return JsonSerializer.Deserialize<Chart<TData>>(json, _jsonSerializerOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Attempts to deserialize a chart from a JSON string.
    /// </summary>
    /// <typeparam name="TData">The type of data in the chart.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="value">Receives the deserialized chart if successful.</param>
    /// <returns><see langword="true"/> if deserialization succeeded; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="json"/> is empty.</exception>
    public static bool TryFromJson<TData>(string json, out Chart<TData>? value)
    {
        ArgumentNullException.ThrowIfNull(json);
        ArgumentException.ThrowIfNullOrEmpty(json);

        try
        {
            value = JsonSerializer.Deserialize<Chart<TData>>(json, _jsonSerializerOptions);
            return true;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }
}