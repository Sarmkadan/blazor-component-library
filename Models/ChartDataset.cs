// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BlazorComponentLibrary.Models;

/// <summary>
/// Represents a dataset for chart visualization.
/// Supports multiple chart types with series data and styling.
/// </summary>
public class ChartDataset
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    [JsonProperty("label")]
    public string Label { get; set; } = string.Empty;

    [Required]
    [JsonProperty("chartType")]
    public ChartType ChartType { get; set; } = ChartType.Line;

    [Required]
    [JsonProperty("data")]
    public List<decimal> Data { get; set; } = new();

    [JsonProperty("labels")]
    public List<string> Labels { get; set; } = new();

    [JsonProperty("backgroundColor")]
    public string? BackgroundColor { get; set; }

    [JsonProperty("borderColor")]
    public string? BorderColor { get; set; }

    [JsonProperty("borderWidth")]
    public int BorderWidth { get; set; } = 1;

    [JsonProperty("fill")]
    public bool Fill { get; set; } = false;

    [JsonProperty("tension")]
    public double Tension { get; set; } = 0.4;

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonProperty("modifiedAt")]
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// Adds a data point to the dataset and optionally a label.
    /// </summary>
    public void AddDataPoint(decimal value, string? label = null)
    {
        Data.Add(value);
        if (label != null && Labels.Count < Data.Count)
        {
            Labels.Add(label);
        }
    }

    /// <summary>
    /// Calculates the average value of the dataset.
    /// </summary>
    public decimal GetAverage()
    {
        return Data.Count == 0 ? 0 : Data.Average();
    }

    /// <summary>
    /// Gets the minimum value in the dataset.
    /// </summary>
    public decimal GetMinimum()
    {
        return Data.Count == 0 ? 0 : Data.Min();
    }

    /// <summary>
    /// Gets the maximum value in the dataset.
    /// </summary>
    public decimal GetMaximum()
    {
        return Data.Count == 0 ? 0 : Data.Max();
    }

    /// <summary>
    /// Calculates the sum of all values in the dataset.
    /// </summary>
    public decimal GetSum()
    {
        return Data.Sum();
    }

    /// <summary>
    /// Validates the dataset configuration.
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Label) &&
               Data.Count > 0 &&
               (Labels.Count == 0 || Labels.Count == Data.Count);
    }

    /// <summary>
    /// Clears all data points and labels from the dataset.
    /// </summary>
    public void Clear()
    {
        Data.Clear();
        Labels.Clear();
    }

    /// <summary>
    /// Creates a normalized copy of the dataset with values scaled to 0-100 range.
    /// </summary>
    public ChartDataset CreateNormalizedCopy()
    {
        if (Data.Count == 0)
        {
            return Copy();
        }

        var min = GetMinimum();
        var max = GetMaximum();
        var range = max - min;

        var normalized = Copy();
        normalized.Data = Data.Select(d =>
        {
            if (range == 0) return 50;
            return ((d - min) / range) * 100;
        }).ToList();

        return normalized;
    }

    /// <summary>
    /// Creates a copy of this dataset.
    /// </summary>
    public ChartDataset Copy()
    {
        return new ChartDataset
        {
            Id = Id,
            Label = Label,
            ChartType = ChartType,
            Data = new List<decimal>(Data),
            Labels = new List<string>(Labels),
            BackgroundColor = BackgroundColor,
            BorderColor = BorderColor,
            BorderWidth = BorderWidth,
            Fill = Fill,
            Tension = Tension,
            CreatedAt = CreatedAt,
            ModifiedAt = DateTime.UtcNow
        };
    }
}

public enum ChartType
{
    Line = 0,
    Bar = 1,
    Pie = 2,
    Doughnut = 3,
    Area = 4,
    Scatter = 5,
    Bubble = 6,
    Radar = 7
}
