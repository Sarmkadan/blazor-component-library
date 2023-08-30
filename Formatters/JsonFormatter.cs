// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorComponentLibrary.Formatters;

/// <summary>
/// JSON formatter using System.Text.Json.
/// Provides serialization and deserialization with consistent settings.
/// Used for API responses and data persistence.
/// </summary>
public class JsonFormatter : IDataFormatter
{
    private readonly JsonSerializerOptions _options;

    public JsonFormatter()
    {
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
    }

    /// <summary>
    /// Serializes object to JSON string.
    /// Uses configured options for consistent formatting.
    /// </summary>
    public string Serialize<T>(T? obj) where T : class
    {
        try
        {
            return JsonSerializer.Serialize(obj, _options);
        }
        catch (JsonException ex)
        {
            throw new FormattingException("Failed to serialize object to JSON", ex);
        }
    }

    /// <summary>
    /// Deserializes JSON string to object.
    /// Validates JSON structure before deserialization.
    /// </summary>
    public T? Deserialize<T>(string json) where T : class
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            return JsonSerializer.Deserialize<T>(json, _options);
        }
        catch (JsonException ex)
        {
            throw new FormattingException("Failed to deserialize JSON to object", ex);
        }
    }

    /// <summary>
    /// Serializes object to JSON bytes.
    /// Useful for binary storage or streaming.
    /// </summary>
    public byte[] SerializeToBytes<T>(T? obj) where T : class
    {
        var json = Serialize(obj);
        return System.Text.Encoding.UTF8.GetBytes(json);
    }

    /// <summary>
    /// Deserializes JSON bytes to object.
    /// </summary>
    public T? DeserializeFromBytes<T>(byte[] data) where T : class
    {
        if (data == null || data.Length == 0)
            return null;

        var json = System.Text.Encoding.UTF8.GetString(data);
        return Deserialize<T>(json);
    }

    /// <summary>
    /// Validates JSON string format without full deserialization.
    /// Useful for quick validation before processing.
    /// </summary>
    public bool IsValidJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            using var _ = JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Merges two objects into a single JSON representation.
    /// Used for patching and partial updates. Array properties are unioned.
    /// </summary>
    public string Merge<T>(T obj1, T obj2) where T : class
    {
        var json1 = Serialize(obj1);
        var json2 = Serialize(obj2);

        using var doc1 = JsonDocument.Parse(json1);
        using var doc2 = JsonDocument.Parse(json2);

        using var stream = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });

        MergeElements(writer, doc1.RootElement, doc2.RootElement);
        writer.Flush();

        return System.Text.Encoding.UTF8.GetString(stream.ToArray());
    }

    private static void MergeElements(Utf8JsonWriter writer, JsonElement base_, JsonElement override_)
    {
        if (base_.ValueKind != JsonValueKind.Object || override_.ValueKind != JsonValueKind.Object)
        {
            override_.WriteTo(writer);
            return;
        }

        writer.WriteStartObject();

        foreach (var prop in base_.EnumerateObject())
        {
            writer.WritePropertyName(prop.Name);
            if (override_.TryGetProperty(prop.Name, out var overrideProp))
            {
                if (prop.Value.ValueKind == JsonValueKind.Array && overrideProp.ValueKind == JsonValueKind.Array)
                {
                    writer.WriteStartArray();
                    foreach (var item in prop.Value.EnumerateArray())
                        item.WriteTo(writer);
                    foreach (var item in overrideProp.EnumerateArray())
                        item.WriteTo(writer);
                    writer.WriteEndArray();
                }
                else
                {
                    MergeElements(writer, prop.Value, overrideProp);
                }
            }
            else
            {
                prop.Value.WriteTo(writer);
            }
        }

        foreach (var prop in override_.EnumerateObject())
        {
            if (!base_.TryGetProperty(prop.Name, out _))
            {
                writer.WritePropertyName(prop.Name);
                prop.Value.WriteTo(writer);
            }
        }

        writer.WriteEndObject();
    }

    /// <summary>
    /// Gets the format name identifier.
    /// </summary>
    public string Format => "json";
}

/// <summary>
/// Interface for data formatters.
/// Allows pluggable serialization strategies.
/// </summary>
public interface IDataFormatter
{
    string Serialize<T>(T? obj) where T : class;
    T? Deserialize<T>(string data) where T : class;
    string Format { get; }
}

/// <summary>
/// Exception thrown during formatting operations.
/// Wraps underlying serialization errors.
/// </summary>
public class FormattingException : Exception
{
    public FormattingException(string message) : base(message) { }
    public FormattingException(string message, Exception innerException) : base(message, innerException) { }
}
