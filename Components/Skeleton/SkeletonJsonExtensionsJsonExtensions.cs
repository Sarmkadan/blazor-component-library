namespace BlazorComponentLibrary.Components.Skeleton;

using System.Text.Json;

/// <summary>
/// Provides JSON serialization and deserialization extension methods for <see cref="SkeletonJsonExtensions"/>.
/// </summary>
public static class SkeletonJsonExtensionsJsonExtensions
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Serializes a <see cref="SkeletonJsonExtensions"/> instance to a JSON string.
    /// </summary>
    /// <param name="value">The skeleton JSON extensions instance to serialize. Cannot be <see langword="null"/>.</param>
    /// <param name="indented">Whether to format the JSON with indentation for readability.</param>
    /// <returns>A JSON string representation of the skeleton JSON extensions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public static string ToJson(this SkeletonJsonExtensions value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);
        return indented
            ? JsonSerializer.Serialize(value, _jsonSerializerOptions)
            : JsonSerializer.Serialize(value);
    }

    /// <summary>
    /// Deserializes a JSON string to a <see cref="SkeletonJsonExtensions"/> instance.
    /// </summary>
    /// <param name="json">The JSON string to deserialize. Cannot be <see langword="null"/>.</param>
    /// <returns>A <see cref="SkeletonJsonExtensions"/> instance if deserialization succeeds; otherwise, <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="json"/> is <see langword="null"/>.</exception>
    public static SkeletonJsonExtensions? FromJson(string json)
    {
        ArgumentNullException.ThrowIfNull(json);
        return JsonSerializer.Deserialize<SkeletonJsonExtensions>(json, _jsonSerializerOptions);
    }

    /// <summary>
    /// Attempts to deserialize a JSON string to a <see cref="SkeletonJsonExtensions"/> instance.
    /// </summary>
    /// <param name="json">The JSON string to deserialize. Cannot be <see langword="null"/>.</param>
    /// <param name="value">Receives the deserialized skeleton JSON extensions if successful; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if deserialization succeeds; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="json"/> is <see langword="null"/>.</exception>
    public static bool TryFromJson(string json, out SkeletonJsonExtensions? value)
    {
        ArgumentNullException.ThrowIfNull(json);

        try
        {
            value = JsonSerializer.Deserialize<SkeletonJsonExtensions>(json, _jsonSerializerOptions);
            return true;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }
}