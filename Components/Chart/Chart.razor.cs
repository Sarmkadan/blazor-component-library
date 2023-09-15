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
