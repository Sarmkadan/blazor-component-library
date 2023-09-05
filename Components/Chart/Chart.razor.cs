namespace BlazorComponentLibrary.Components.Chart;

using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

public partial class Chart<TData> : ComponentBase, IChart<TData>
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
    /// Sets the data source for the chart.
    /// </summary>
    /// <param name="data">The enumerable collection of data items.</param>
    public void SetData(IEnumerable<TData> data)
    {
        _data = data ?? Enumerable.Empty<TData>();
        StateHasChanged(); // Notify Blazor that the component state has changed
    }

    /// <summary>
    /// Refreshes the chart, re-rendering its content.
    /// </summary>
    public void Refresh()
    {
        StateHasChanged();
    }

    // A placeholder for rendering logic, will be implemented in Chart.razor
    protected RenderFragment ChildContent { get; set; }
}
