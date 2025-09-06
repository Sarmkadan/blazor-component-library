// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace BlazorComponentLibrary.Formatters;

/// <summary>
/// JSON formatter using Newtonsoft.Json.
/// Provides serialization and deserialization with consistent settings.
/// Used for API responses and data persistence.
/// </summary>
public class JsonFormatter : IDataFormatter
{
    private readonly JsonSerializerSettings _settings;

    public JsonFormatter()
    {
        _settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ",
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            Converters = new JsonConverter[]
            {
                new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() }
            }
        };
    }

    /// <summary>
    /// Serializes object to JSON string.
    /// Uses configured settings for consistent formatting.
    /// </summary>
    public string Serialize<T>(T? obj) where T : class
    {
        try
        {
            return JsonConvert.SerializeObject(obj, _settings);
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
            return JsonConvert.DeserializeObject<T>(json, _settings);
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
            JsonConvert.DeserializeObject(json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Merges two objects into a single JSON representation.
    /// Used for patching and partial updates.
    /// </summary>
    public string Merge<T>(T obj1, T obj2) where T : class
    {
        var json1 = Serialize(obj1);
        var json2 = Serialize(obj2);

        var jObject1 = Newtonsoft.Json.Linq.JObject.Parse(json1);
        var jObject2 = Newtonsoft.Json.Linq.JObject.Parse(json2);

        jObject1.Merge(jObject2, new Newtonsoft.Json.Linq.JsonMergeSettings
        {
            MergeArrayHandling = Newtonsoft.Json.Linq.MergeArrayHandling.Union
        });

        return jObject1.ToString();
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
