using System.Text;
using System.Text.Json;

namespace BlazorComponentLibrary.Exceptions;

/// <summary>
/// Provides System.Text.Json helpers for serializing <see cref="ModalException"/> instances.
/// Only the exception message is persisted; stack traces, inner exceptions and <c>Data</c> are not included.
/// </summary>
public static class ModalExceptionJsonExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    /// <summary>Serializes a <see cref="ModalException"/> to a JSON string.</summary>
    /// <param name="value">The exception to serialize.</param>
    /// <param name="indented">When <c>true</c>, the JSON output is indented for readability.</param>
    /// <returns>A JSON object containing the exception message.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
    public static string ToJson(this ModalException value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);

        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = indented }))
        {
            JsonSerializer.Serialize(writer, new ModalExceptionPayload(value.Message), _jsonOptions);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>Deserializes a <see cref="ModalException"/> from a JSON string produced by <see cref="ToJson(ModalException, bool)"/>.</summary>
    /// <param name="json">The JSON text to parse.</param>
    /// <returns>The deserialized exception, or <c>null</c> when the JSON literal is <c>null</c>.</returns>
    /// <exception cref="ArgumentException"><paramref name="json"/> is <c>null</c> or empty.</exception>
    /// <exception cref="JsonException"><paramref name="json"/> is not valid JSON for a <see cref="ModalException"/>.</exception>
    public static ModalException? FromJson(string json)
    {
        ArgumentException.ThrowIfNullOrEmpty(json);

        var payload = JsonSerializer.Deserialize<ModalExceptionPayload>(json, _jsonOptions);

        return payload is null
            ? null
            : payload.Message is null
                ? new ModalException()
                : new ModalException(payload.Message);
    }

    /// <summary>Attempts to deserialize a <see cref="ModalException"/> from a JSON string without throwing on malformed input.</summary>
    /// <param name="json">The JSON text to parse.</param>
    /// <param name="value">When this method returns <c>true</c>, contains the deserialized exception (or <c>null</c> for a JSON <c>null</c> literal); otherwise <c>null</c>.</param>
    /// <returns><c>true</c> if the JSON was parsed successfully; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentException"><paramref name="json"/> is <c>null</c> or empty.</exception>
    public static bool TryFromJson(string json, out ModalException? value)
    {
        ArgumentException.ThrowIfNullOrEmpty(json);

        try
        {
            value = FromJson(json);
            return true;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }

    /// <summary>Wire format for <see cref="ModalException"/>.</summary>
    /// <param name="Message">The exception message.</param>
    private sealed record ModalExceptionPayload(string? Message);
}
