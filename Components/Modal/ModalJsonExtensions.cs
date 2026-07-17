namespace BlazorComponentLibrary.Components.Modal;

using System;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

/// <summary>
/// Provides System.Text.Json serialization and deserialization extensions for <see cref="Modal"/> components.
/// </summary>
public static class ModalJsonExtensions
{
	private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		WriteIndented = false,
	};

	/// <summary>
	/// Serializes the modal to a JSON string.
	/// </summary>
	/// <param name="value">The modal instance to serialize.</param>
	/// <param name="indented">Whether to format the JSON with indentation for readability.</param>
	/// <returns>A JSON string representation of the modal.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
	public static string ToJson(this Modal value, bool indented = false)
	{
		ArgumentNullException.ThrowIfNull(value);

		var options = indented
			? new JsonSerializerOptions(_jsonOptions) { WriteIndented = true }
			: _jsonOptions;

		return JsonSerializer.Serialize(value, options);
	}

	/// <summary>
	/// Deserializes a JSON string into a new <see cref="Modal"/> instance.
	/// </summary>
	/// <param name="json">The JSON string to deserialize.</param>
	/// <returns>A new modal instance populated from the JSON, or null if deserialization fails.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="json"/> is null.</exception>
	/// <exception cref="JsonException">Thrown when the JSON is invalid or cannot be deserialized.</exception>
	public static Modal? FromJson(string json)
	{
		ArgumentNullException.ThrowIfNull(json);

		return JsonSerializer.Deserialize<Modal>(json, _jsonOptions);
	}

	/// <summary>
	/// Attempts to deserialize a JSON string into a <see cref="Modal"/> instance.
	/// </summary>
	/// <param name="json">The JSON string to deserialize.</param>
	/// <param name="value">Receives the deserialized modal instance if successful; otherwise, null.</param>
	/// <returns>True if deserialization succeeded; false if the JSON is invalid or deserialization failed.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="json"/> is null.</exception>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
	public static bool TryFromJson(string json, out Modal? value)
	{
		ArgumentNullException.ThrowIfNull(json);

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