// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorComponentLibrary.Models;

/// <summary>
/// Represents a single row of data in a DataTable component.
/// Stores row data, state, and metadata.
/// </summary>
public class DataTableRow
{
    [Key]
    public int Id { get; set; }

    [Required]
    [JsonPropertyName("tableId")]
    public int TableId { get; set; }

    [Required]
    [JsonPropertyName("data")]
    public Dictionary<string, object?> Data { get; set; } = new();

    [JsonPropertyName("isSelected")]
    public bool IsSelected { get; set; } = false;

    [JsonPropertyName("isExpanded")]
    public bool IsExpanded { get; set; } = false;

    [JsonPropertyName("cssClass")]
    public string? CssClass { get; set; }

    [JsonPropertyName("rowNumber")]
    public int RowNumber { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the value for a specific column key with type safety.
    /// </summary>
    public T? GetValue<T>(string key)
    {
        if (!Data.TryGetValue(key, out var value)) return default;

        try
        {
            if (value == null) return default;
            if (typeof(T).IsAssignableFrom(value.GetType())) return (T)value;
            return (T?)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Sets a value for a specific column key.
    /// </summary>
    public void SetValue(string key, object? value)
    {
        if (Data.ContainsKey(key))
        {
            Data[key] = value;
        }
        else
        {
            Data.Add(key, value);
        }
    }

    /// <summary>
    /// Validates that all required data keys are present with non-null values.
    /// </summary>
    public bool HasAllKeys(IEnumerable<string> requiredKeys)
    {
        return requiredKeys.All(key => Data.ContainsKey(key) && Data[key] != null);
    }

    /// <summary>
    /// Creates a dictionary representation of the row for serialization.
    /// </summary>
    public Dictionary<string, object?> ToDictionary()
    {
        return new Dictionary<string, object?>(Data)
        {
            { "_id", Id },
            { "_selected", IsSelected },
            { "_expanded", IsExpanded }
        };
    }

    /// <summary>
    /// Toggles the selection state of this row.
    /// </summary>
    public void ToggleSelection()
    {
        IsSelected = !IsSelected;
    }

    /// <summary>
    /// Toggles the expansion state of this row.
    /// </summary>
    public void ToggleExpansion()
    {
        IsExpanded = !IsExpanded;
    }
}
