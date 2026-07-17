namespace BlazorComponentLibrary.Components.Skeleton;

using System.Text.Json;
using System.Text.Json.Serialization;

public static class SkeletonJsonExtensions
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    public static string ToJson(this Skeleton value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);
        return indented ? JsonSerializer.Serialize(value, _jsonSerializerOptions) : JsonSerializer.Serialize(value);
    }

    public static Skeleton? FromJson(string json)
    {
        ArgumentNullException.ThrowIfNull(json);
        try
        {
            return JsonSerializer.Deserialize<Skeleton>(json, _jsonSerializerOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public static bool TryFromJson(string json, out Skeleton? value)
    {
        ArgumentNullException.ThrowIfNull(json);
        try
        {
            value = JsonSerializer.Deserialize<Skeleton>(json, _jsonSerializerOptions);
            return true;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }
}
