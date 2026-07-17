namespace BlazorComponentLibrary.Components.Skeleton;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Provides JSON serialization and deserialization extension methods for <see cref="Skeleton"/> components.
/// </summary>
public static class SkeletonJsonExtensions
{
	private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web)
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		WriteIndented = true,
		NumberHandling = JsonNumberHandling.AllowReadingFromString
	};

	/// <summary>
	/// Serializes a <see cref="Skeleton"/> instance to a JSON string.
	/// </summary>
	/// <param name="value">The skeleton instance to serialize. Cannot be <see langword="null"/>.</param>
	/// <param name="indented">Whether to format the JSON with indentation for readability.</param>
	/// <returns>A JSON string representation of the skeleton.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	public static string ToJson(this Skeleton value, bool indented = false)
	{
		ArgumentNullException.ThrowIfNull(value);
		return indented
			? JsonSerializer.Serialize(value, _jsonSerializerOptions)
			: JsonSerializer.Serialize(value);
	}

	/// <summary>
	/// Deserializes a JSON string to a <see cref="Skeleton"/> instance.
	/// </summary>
	/// <param name="json">The JSON string to deserialize. Cannot be <see langword="null"/>.</param>
	/// <returns>A <see cref="Skeleton"/> instance if deserialization succeeds; otherwise, <see langword="null"/>.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="json"/> is <see langword="null"/>.</exception>
	public static Skeleton? FromJson(string json)
	{
		ArgumentNullException.ThrowIfNull(json);

		return JsonSerializer.Deserialize<Skeleton>(json, _jsonSerializerOptions);
	}

	/// <summary>
	/// Attempts to deserialize a JSON string to a <see cref="Skeleton"/> instance.
	/// </summary>
	/// <param name="json">The JSON string to deserialize. Cannot be <see langword="null"/>.</param>
	/// <param name="value">Receives the deserialized skeleton if successful; otherwise, <see langword="null"/>.</param>
	/// <returns><see langword="true"/> if deserialization succeeds; otherwise, <see langword="false"/>.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="json"/> is <see langword="null"/>.</exception>
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