namespace BlazorComponentLibrary.Components.Form;

using System;
using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

/// <summary>
/// Provides System.Text.Json serialization and deserialization extensions for <see cref="Form{TModel}"/> components.
/// </summary>
public static class FormJsonExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    /// <summary>
    /// Serializes the form's model to a JSON string.
    /// </summary>
    /// <typeparam name="TModel">The model type bound to the form.</typeparam>
    /// <param name="value">The form instance to serialize.</param>
    /// <param name="indented">Whether to format the JSON with indentation for readability.</param>
    /// <returns>A JSON string representation of the form's model.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
    public static string ToJson<TModel>(this Form<TModel> value, bool indented = false) where TModel : new()
    {
        ArgumentNullException.ThrowIfNull(value);

        var options = indented
            ? new JsonSerializerOptions(_jsonOptions) { WriteIndented = true }
            : _jsonOptions;

        return JsonSerializer.Serialize(value.Model, options);
    }

    /// <summary>
    /// Deserializes a JSON string into a new <see cref="Form{TModel}"/> model instance.
    /// </summary>
    /// <typeparam name="TModel">The model type to deserialize.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>A new model instance populated from the JSON, or null if deserialization fails.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="json"/> is null.</exception>
    public static Form<TModel>? FromJson<TModel>(string json) where TModel : new()
    {
        ArgumentNullException.ThrowIfNull(json);

        try
        {
            var model = JsonSerializer.Deserialize<TModel>(json, _jsonOptions);
            if (model is null)
            {
                return null;
            }

            var form = new Form<TModel>();
            form.SetModel(model);
            return form;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Attempts to deserialize a JSON string into a <see cref="Form{TModel}"/> model instance.
    /// </summary>
    /// <typeparam name="TModel">The model type to deserialize.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="value">Receives the deserialized form instance if successful; otherwise, null.</param>
    /// <returns>True if deserialization succeeded; false if the JSON is invalid or deserialization failed.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="json"/> is null.</exception>
    public static bool TryFromJson<TModel>(string json, out Form<TModel>? value) where TModel : new()
    {
        ArgumentNullException.ThrowIfNull(json);

        try
        {
            value = FromJson<TModel>(json);
            return value is not null;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }
}