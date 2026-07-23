namespace BlazorComponentLibrary.Components.Pagination;

using Microsoft.AspNetCore.Components;

/// <summary>
/// A pagination component that displays page numbers with ellipsis logic and supports keyboard navigation.
/// </summary>
public sealed partial class Pagination : ComponentBase
{
    /// <summary>
    /// Gets or sets the current page number (1-based).
    /// </summary>
    [Parameter]
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    /// Gets or sets the total number of pages.
    /// </summary>
    [Parameter]
    public int TotalPages { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of sibling pages to show around the current page.
    /// Defaults to 1 (shows 1 page on each side).
    /// </summary>
    [Parameter]
    public int SiblingCount { get; set; } = 1;

    /// <summary>
    /// Gets or sets the maximum number of pages to display.
    /// Defaults to 7 (shows first, last, and up to 5 middle pages).
    /// </summary>
    [Parameter]
    public int MaxVisiblePages { get; set; } = 7;

    /// <summary>
    /// Gets or sets the CSS class for the pagination container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the inline style for the pagination container.
    /// </summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>
    /// Event callback that fires when the page changes.
    /// </summary>
    [Parameter]
    public EventCallback<int> PageChanged { get; set; }

    internal IEnumerable<PageItem> PageItems
    {
        get
        {
            var items = new List<PageItem>();

            // Always show first page
            items.Add(new PageItem(1, PageItemType.Page));

            // Calculate range around current page
            var leftBound = Math.Max(2, CurrentPage - SiblingCount);
            var rightBound = Math.Min(TotalPages - 1, CurrentPage + SiblingCount);

            // Add left ellipsis if needed
            if (leftBound > 2)
            {
                items.Add(new PageItem(0, PageItemType.Ellipsis));
            }

            // Add pages in range
            for (int i = leftBound; i <= rightBound; i++)
            {
                items.Add(new PageItem(i, PageItemType.Page));
            }

            // Add right ellipsis if needed
            if (rightBound < TotalPages - 1)
            {
                items.Add(new PageItem(0, PageItemType.Ellipsis));
            }

            // Always show last page if more than one page
            if (TotalPages > 1)
            {
                items.Add(new PageItem(TotalPages, PageItemType.Page));
            }

            return items;
        }
    }

    private bool ShowFirstPage => TotalPages > MaxVisiblePages;
    private bool ShowLastPage => TotalPages > MaxVisiblePages;
    private bool ShowPreviousPage => CurrentPage > 1;
    private bool ShowNextPage => CurrentPage < TotalPages;

    private async Task NavigateToPage(int pageNumber)
    {
        if (pageNumber >= 1 && pageNumber <= TotalPages && pageNumber != CurrentPage)
        {
            CurrentPage = pageNumber;
            await PageChanged.InvokeAsync(CurrentPage);
        }
    }

    internal record PageItem(int Number, PageItemType Type);
    internal enum PageItemType { Page, Ellipsis }
}
