namespace BlazorComponentLibrary.Components.DragDropList;

using System.Text.Json;

/// <summary>
/// Provides JSON serialization extension methods for <see cref="DragDropList{TItem}"/> components.
/// </summary>
public static class DragDropListExtensionsJsonExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Serializes the items of the <see cref="DragDropList{TItem}"/> to a JSON string.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the list.</typeparam>
    /// <param name="list">The <see cref="DragDropList{TItem}"/> instance.</param>
    /// <param name="indented">If set to <c>true</c> the JSON is indented for readability.</param>
    /// <returns>A JSON string representing the items.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="list"/> is <see langword="null"/>.</exception>
    public static string ToJson<TItem>(this DragDropList<TItem> list, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(list);

        var options = indented ? new JsonSerializerOptions(_jsonOptions) { WriteIndented = true } : _jsonOptions;
        return JsonSerializer.Serialize(list.Items, options);
    }

    /// <summary>
    /// Deserializes a JSON string into a new <see cref="DragDropList{TItem}"/> instance.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the list.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>A new <see cref="DragDropList{TItem}"/> instance containing the deserialized items, or <see langword="null"/> if deserialization fails or <paramref name="json"/> is <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is <see langword="null"/>.</exception>
    public static DragDropList<TItem>? FromJson<TItem>(string json)
    {
        ArgumentNullException.ThrowIfNull(json);

        var items = JsonSerializer.Deserialize<IList<TItem>>(json, _jsonOptions);
        if (items == null)
            return null;

        var list = new DragDropList<TItem>();
        list.Items = items;
        return list;
    }

    /// <summary>
    /// Attempts to deserialize a JSON string into a <see cref="DragDropList{TItem}"/> instance.
    /// </summary>
    /// <typeparam name="TItem">The type of items in the list.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="value">When this method returns, contains the deserialized <see cref="DragDropList{TItem}"/> instance if the deserialization succeeded, or <see langword="null"/> if it failed. This parameter is passed uninitialized.</param>
    /// <returns><see langword="true"/> if <paramref name="json"/> was successfully deserialized; otherwise, <see langword="false"/>.</returns>
    public static bool TryFromJson<TItem>(string json, out DragDropList<TItem>? value)
    {
        try
        {
            value = FromJson<TItem>(json);
            return value != null;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
        catch (ArgumentNullException)
        {
            value = null;
            return false;
        }
    }
}