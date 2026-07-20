namespace BlazorComponentLibrary.Components.DataTable;

using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

public interface IDataTable<TItem>
{
    /// <summary>
    /// Sets the data source for the data table.
    /// </summary>
    /// <param name="data">The enumerable collection of data items to populate the table with.</param>
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
    /// Gets or sets a value indicating whether row virtualization is enabled.
    /// When true, only visible rows are rendered in the DOM, allowing efficient
    /// display of large datasets.
    /// </summary>
    bool EnableVirtualization { get; set; }

    /// <summary>
    /// Gets or sets the names of columns that should be hidden.
    /// </summary>
    ISet<string> HiddenColumns { get; set; }

    /// <summary>
    /// Sorts the table by the specified key selector with null-safe comparison.
    /// </summary>
    /// <param name="keySelector">A function to extract the key to sort by from a data item.</param>
    /// <param name="direction">The sort direction. Defaults to <see cref="SortDirection.Ascending"/>.</param>
    void SortBy(Func<TItem, object?> keySelector, SortDirection direction = SortDirection.Ascending);

    /// <summary>
    /// Clears all sort keys and returns to the original data order.
    /// </summary>
    void ClearSort();

    /// <summary>
    /// Toggles the visibility of a column by name.
    /// </summary>
    /// <param name="columnName">The name of the column to toggle.</param>
    void ToggleColumn(string columnName);

    /// <summary>
    /// Event callback for when a row in the data table is clicked.
    /// </summary>
    EventCallback<TItem> OnRowClick { get; set; }

        /// <summary>
        /// Content to render when the data table has no items to display.
        /// </summary>
        RenderFragment? EmptyTemplate { get; set; }
}
