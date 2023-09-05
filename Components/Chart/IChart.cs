namespace BlazorComponentLibrary.Components.Chart;

using Microsoft.AspNetCore.Components;
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
}
