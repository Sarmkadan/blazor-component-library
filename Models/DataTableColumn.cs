// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BlazorComponentLibrary.Models;

/// <summary>
/// Defines a column configuration for the DataTable component.
/// Includes sorting, filtering, and display behavior settings.
/// </summary>
public class DataTableColumn
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    [JsonProperty("key")]
    public string Key { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    [JsonProperty("header")]
    public string Header { get; set; } = string.Empty;

    [JsonProperty("dataType")]
    public DataType DataType { get; set; } = DataType.String;

    [JsonProperty("isVisible")]
    public bool IsVisible { get; set; } = true;

    [JsonProperty("isSortable")]
    public bool IsSortable { get; set; } = true;

    [JsonProperty("isFilterable")]
    public bool IsFilterable { get; set; } = true;

    [Range(10, 500)]
    [JsonProperty("width")]
    public int Width { get; set; } = 100;

    [JsonProperty("alignment")]
    public TextAlignment Alignment { get; set; } = TextAlignment.Left;

    [JsonProperty("sortOrder")]
    public int SortOrder { get; set; } = 0;

    [JsonProperty("format")]
    public string? Format { get; set; }

    /// <summary>
    /// Formats a value according to the column's data type and format string.
    /// </summary>
    public string FormatValue(object? value)
    {
        if (value == null) return string.Empty;

        return DataType switch
        {
            DataType.Date when value is DateTime dt => dt.ToString(Format ?? "yyyy-MM-dd"),
            DataType.Currency when value is decimal dec => dec.ToString(Format ?? "C2"),
            DataType.Percentage when value is double d => (d * 100).ToString(Format ?? "F2") + "%",
            DataType.Boolean when value is bool b => b ? "Yes" : "No",
            _ => value.ToString() ?? string.Empty
        };
    }

    /// <summary>
    /// Validates the column configuration.
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Key) &&
               !string.IsNullOrWhiteSpace(Header) &&
               Width >= 10 &&
               Width <= 500 &&
               Key.Length >= 1 &&
               Key.Length <= 100;
    }

    /// <summary>
    /// Creates a copy of this column with updated properties.
    /// </summary>
    public DataTableColumn Copy()
    {
        return new DataTableColumn
        {
            Id = Id,
            Key = Key,
            Header = Header,
            DataType = DataType,
            IsVisible = IsVisible,
            IsSortable = IsSortable,
            IsFilterable = IsFilterable,
            Width = Width,
            Alignment = Alignment,
            SortOrder = SortOrder,
            Format = Format
        };
    }
}

public enum DataType
{
    String = 0,
    Number = 1,
    Date = 2,
    Boolean = 3,
    Currency = 4,
    Percentage = 5,
    Email = 6,
    Url = 7
}

public enum TextAlignment
{
    Left = 0,
    Center = 1,
    Right = 2,
    Justify = 3
}
