namespace BlazorComponentLibrary.Components.DataTable;

using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

public partial class DataTable<TItem> : ComponentBase, IDataTable<TItem>
{
    private IEnumerable<TItem> _data = Enumerable.Empty<TItem>();
    private IEnumerable<TItem> _currentViewData = Enumerable.Empty<TItem>();

    [Parameter]
    public RenderFragment TableHeader { get; set; }

    [Parameter]
    public RenderFragment<TItem> RowTemplate { get; set; }

    [Parameter]
    public EventCallback<TItem> OnRowClick { get; set; }

    [Parameter]
    public bool IsSortable { get; set; } = false;

    [Parameter]
    public bool IsFilterable { get; set; } = false;

    [Parameter]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Sets the data source for the data table.
    /// </summary>
    /// <param name="data">The enumerable collection of data items.</param>
    public void SetData(IEnumerable<TItem> data)
    {
        _data = data ?? Enumerable.Empty<TItem>();
        ApplyPagination();
        StateHasChanged(); // Notify Blazor that the component state has changed
    }

    /// <summary>
    /// Refreshes the data table, re-rendering its content.
    /// </summary>
    public void Refresh()
    {
        ApplyPagination();
        StateHasChanged();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        ApplyPagination();
    }

    private void ApplyPagination()
    {
        // Simple pagination for now, no sorting/filtering implemented yet
        _currentViewData = _data.Take(PageSize);
    }

    protected async Task OnRowClickHandler(TItem item)
    {
        if (OnRowClick.HasDelegate)
        {
            await OnRowClick.InvokeAsync(item);
        }
    }
}
