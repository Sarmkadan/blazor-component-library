namespace BlazorComponentLibrary.Components.DataTable;

using Microsoft.AspNetCore.Components;

public interface IDataTable<TItem>
{
    /// <summary>
    /// Sets the data source for the data table.
    /// </summary>
    /// <param name="data">The enumerable collection of data items.</param>
    void SetData(IEnumerable<TItem> data);

    /// <summary>
    /// Refreshes the data table, re-rendering its content.
    /// </summary>
    void Refresh();

    /// <summary>
    /// Gets or sets a value indicating whether the data table columns are sortable.
    /// </summary>
    bool IsSortable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the data table columns are filterable.
    /// </summary>
    bool IsFilterable { get; set; }

    /// <summary>
    /// Gets or sets the number of items to display per page.
    /// </summary>
    int PageSize { get; set; }

    /// <summary>
    /// Event callback for when a row in the data table is clicked.
    /// </summary>
    EventCallback<TItem> OnRowClick { get; set; }
}
