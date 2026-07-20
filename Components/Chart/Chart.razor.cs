namespace BlazorComponentLibrary.Components.Chart;

using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

public sealed partial class Chart<TData> : ComponentBase, IChart<TData>
{
    private IEnumerable<TData> _data = Enumerable.Empty<TData>();

    [Parameter]
    public ChartType ChartType { get; set; }

    [Parameter]
    public string Title { get; set; } = string.Empty;

    [Parameter]
    public IEnumerable<string> Labels { get; set; } = Enumerable.Empty<string>();

    [Parameter]
    public IEnumerable<string> Colors { get; set; } = Enumerable.Empty<string>();

    [Parameter]
    public object Options { get; set; } = new object();

    /// <summary>
    /// Gets or sets annotations overlaid on the chart, such as threshold lines,
    /// event markers, and reference bands.
    /// </summary>
    [Parameter]
    public IEnumerable<ChartAnnotation> Annotations { get; set; } = Enumerable.Empty<ChartAnnotation>();

    /// <summary>
    /// Sets the data source for the chart.
    /// </summary>
    /// <param name="data">The enumerable collection of data items.</param>
    public void SetData(IEnumerable<TData> data)
    {
        _data = data ?? Enumerable.Empty<TData>();
        NotifyStateChanged();
    }

    /// <summary>
    /// Refreshes the chart, re-rendering its content.
    /// </summary>
    public void Refresh() => NotifyStateChanged();

    /// <summary>
    /// Sets the visibility of a series in the chart.
    /// </summary>
    /// <param name="seriesIndex">The index of the series to toggle.</param>
    /// <param name="visible">Whether the series should be visible.</param>
    public void SetSeriesVisibility(int seriesIndex, bool visible)
    {
        // This method is called from JavaScript interop when a legend item is clicked
        // This method signature satisfies the IChart interface
  // The actual visibility toggle is handled in JavaScript
  // This maintains the interface contract defined in IChart<TData>
  StateHasChanged();
    }

    private void NotifyStateChanged()
    {
        try
        {
            StateHasChanged();
        }
        catch (InvalidOperationException)
        {
            // Ignore if the component is not attached to a renderer (e.g. unit tests).
        }
    }

    /// <summary>
    /// Gets or sets custom content rendered inside the chart body, after the
    /// built-in summary and before any annotations.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
