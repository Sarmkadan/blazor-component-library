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
            int effectiveTotalPages = Math.Max(1, TotalPages);
            int effectiveCurrentPage = Math.Clamp(CurrentPage, 1, effectiveTotalPages);

            if (effectiveTotalPages <= MaxVisiblePages)
            {
                // Show all pages when within max visible limit
                for (int i = 1; i <= effectiveTotalPages; i++)
                {
                    items.Add(new PageItem(i, PageItemType.Page));
                }
            }
            else
            {
                // Always show first page
                items.Add(new PageItem(1, PageItemType.Page));

                // Calculate range around current page
                var leftBound = Math.Max(2, effectiveCurrentPage - SiblingCount);
                var rightBound = Math.Min(effectiveTotalPages - 1, effectiveCurrentPage + SiblingCount);

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
                if (rightBound < effectiveTotalPages - 1)
                {
                    items.Add(new PageItem(0, PageItemType.Ellipsis));
                }

                // Always show last page if more than one page
                if (effectiveTotalPages > 1)
                {
                    items.Add(new PageItem(effectiveTotalPages, PageItemType.Page));
                }
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
        int effectiveTotalPages = Math.Max(1, TotalPages);
        int effectiveCurrentPage = Math.Clamp(CurrentPage, 1, effectiveTotalPages);

        if (pageNumber >= 1 && pageNumber <= effectiveTotalPages && pageNumber != effectiveCurrentPage)
        {
            CurrentPage = pageNumber;
            await PageChanged.InvokeAsync(CurrentPage);
        }
    }

    internal record PageItem(int Number, PageItemType Type);
    internal enum PageItemType { Page, Ellipsis }
}
