namespace BlazorComponentLibrary.Components.Chart;

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

public enum ChartType
{
    Bar,
    Line,
    Pie,
    Doughnut,
    Radar,
    PolarArea,
    Bubble,
    Scatter
}

public enum ChartAnnotationType
{
    /// <summary>A horizontal line marking a threshold or target value.</summary>
    ThresholdLine,
    /// <summary>A vertical line marking a significant event on the x-axis.</summary>
    EventMarker,
    /// <summary>A shaded band between two values (e.g., an acceptable range).</summary>
    ReferenceBand
}

/// <summary>
/// Represents a contextual annotation overlaid on a chart, such as a threshold
/// line, event marker, or shaded reference band.
/// </summary>
public sealed class ChartAnnotation
{
    /// <summary>Gets or sets the annotation type.</summary>
    public ChartAnnotationType Type { get; set; }

    /// <summary>
    /// Gets or sets the primary value for the annotation.
    /// For ThresholdLine/EventMarker this is the line position.
    /// For ReferenceBand this is the band's start value.
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Gets or sets the end value for a ReferenceBand annotation.
    /// Ignored for other annotation types.
    /// </summary>
    public double? EndValue { get; set; }

    /// <summary>Gets or sets the display label for the annotation.</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Gets or sets the CSS color for the annotation line or band.</summary>
    public string Color { get; set; } = "#666666";

    /// <summary>Gets or sets the tooltip text shown on hover.</summary>
    public string Tooltip { get; set; } = string.Empty;
}

public interface IChart<TData>
{
    /// <summary>
    /// Sets the data source for the chart.
    /// </summary>
    /// <param name="data">The enumerable collection of data items.</param>
    void SetData(IEnumerable<TData> data);

    /// <summary>
    /// Refreshes the chart, re-rendering its content.
    /// </summary>
    void Refresh();

    /// <summary>
    /// Sets the visibility of a series in the chart.
    /// </summary>
    /// <param name="seriesIndex">The index of the series to toggle.</param>
    /// <param name="visible">Whether the series should be visible.</param>
    void SetSeriesVisibility(int seriesIndex, bool visible);

    /// <summary>
    /// Gets or sets the type of chart to display.
    /// </summary>
    ChartType ChartType { get; set; }

    /// <summary>
    /// Gets or sets the title of the chart.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// Gets or sets the labels for the chart's data points or series.
    /// </summary>
    IEnumerable<string> Labels { get; set; }

    /// <summary>
    /// Gets or sets the colors for the chart's data points or series.
    /// </summary>
    IEnumerable<string> Colors { get; set; }

    /// <summary>
    /// Gets or sets additional options for configuring the chart (e.g., library-specific settings).
    /// </summary>
    object Options { get; set; }

    /// <summary>
    /// Gets or sets the annotations overlaid on the chart, such as threshold lines,
    /// event markers, and reference bands.
    /// </summary>
    /// <summary>
/// Gets or sets a function to format numeric values for axis labels and tooltips.
/// Defaults to invariant culture formatting with "0.##" pattern.
/// </summary>
Func<double, string> ValueFormatter { get; set; }

		IEnumerable<ChartAnnotation> Annotations { get; set; }
}
