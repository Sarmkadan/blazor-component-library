namespace BlazorComponentLibrary.Components.DragDropList;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Provides System.Text.Json serialization and deserialization helpers for <see cref="DragDropList{TItem}"/>.
/// </summary>
public static class DragDropListJsonExtensions
{
	private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		WriteIndented = false,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
	};

	/// <summary>
	/// Serializes a <see cref="DragDropList{TItem}"/> to a JSON string.
	/// </summary>
	/// <typeparam name="TItem">The type of items in the list.</typeparam>
	/// <param name="value">The drag-and-drop list to serialize.</param>
	/// <param name="indented">Whether to format the JSON with indentation for readability.</param>
	/// <returns>A JSON string representation of the list.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
	public static string ToJson<TItem>(this DragDropList<TItem> value, bool indented = false)
	{
		ArgumentNullException.ThrowIfNull(value);

		var options = indented
			? new JsonSerializerOptions(_jsonOptions)
			{
				WriteIndented = true,
			}
			: _jsonOptions;

		return JsonSerializer.Serialize(value.Items, options);
	}

	/// <summary>
	/// Deserializes a JSON string into a <see cref="DragDropList{TItem}"/>.
	/// </summary>
	/// <typeparam name="TItem">The type of items in the list.</typeparam>
	/// <param name="json">The JSON string to deserialize.</param>
	/// <returns>A new <see cref="DragDropList{TItem}"/> instance with deserialized items, or null if the JSON is null or empty.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is null.</exception>
	/// <exception cref="JsonException">Thrown when the JSON is malformed or cannot be deserialized.</exception>
	public static DragDropList<TItem>? FromJson<TItem>(string json)
	{
		ArgumentNullException.ThrowIfNull(json);

		if (string.IsNullOrEmpty(json))
		{
			return null;
		}

		var items = JsonSerializer.Deserialize<List<TItem>>(json, _jsonOptions);
		return items is null
			? null
			: new DragDropList<TItem> { Items = items };
	}

	/// <summary>
	/// Attempts to deserialize a JSON string into a <see cref="DragDropList{TItem}"/>.
	/// </summary>
	/// <typeparam name="TItem">The type of items in the list.</typeparam>
	/// <param name="json">The JSON string to deserialize.</param>
	/// <param name="value">Receives the deserialized list, or null if deserialization fails.</param>
	/// <returns>True if deserialization succeeded; otherwise, false.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is null.</exception>
	public static bool TryFromJson<TItem>(string json, out DragDropList<TItem>? value)
	{
		ArgumentNullException.ThrowIfNull(json);

		value = null;

		if (string.IsNullOrEmpty(json))
		{
			return true;
		}

		try
		{
			var items = JsonSerializer.Deserialize<List<TItem>>(json, _jsonOptions);
			if (items is not null)
			{
				value = new DragDropList<TItem> { Items = items };
			}

			return true;
		}
		catch (JsonException)
		{
			return false;
		}
	}
}
