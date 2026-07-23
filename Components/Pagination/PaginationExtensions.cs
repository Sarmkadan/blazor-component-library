namespace BlazorComponentLibrary.Components.Pagination;

/// <summary>
/// Provides extension methods for working with the <see cref="Pagination"/> component.
/// </summary>
public static class PaginationExtensions
{
    /// <summary>
    /// Gets the collection of page items that should be displayed in the pagination control.
    /// </summary>
    /// <param name="pagination">The pagination instance.</param>
    /// <returns>An enumerable of tuples representing the pages to display, where Item1 is the page number (0 for ellipsis) and Item2 indicates if it's an ellipsis.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="pagination"/> is null.</exception>
    public static IEnumerable<(int PageNumber, bool IsEllipsis)> GetPageItems(this Pagination pagination)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        return pagination.PageItems.Select(item => (item.Number, item.Type == Pagination.PageItemType.Ellipsis));
    }

    /// <summary>
    /// Determines whether the specified page number is the current page.
    /// </summary>
    /// <param name="pagination">The pagination instance.</param>
    /// <param name="pageNumber">The page number to check.</param>
    /// <returns>True if the page is the current page; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="pagination"/> is null.</exception>
    public static bool IsCurrentPage(this Pagination pagination, int pageNumber)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        return pagination.CurrentPage == pageNumber;
    }

    /// <summary>
    /// Determines whether the specified page number is valid within the pagination range.
    /// </summary>
    /// <param name="pagination">The pagination instance.</param>
    /// <param name="pageNumber">The page number to validate.</param>
    /// <returns>True if the page number is valid; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="pagination"/> is null.</exception>
    public static bool IsValidPage(this Pagination pagination, int pageNumber)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        return pageNumber >= 1 && pageNumber <= pagination.TotalPages;
    }

    /// <summary>
    /// Gets the next page number that can be navigated to.
    /// </summary>
    /// <param name="pagination">The pagination instance.</param>
    /// <returns>The next page number, or 0 if no next page exists.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="pagination"/> is null.</exception>
    public static int GetNextPage(this Pagination pagination)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        return pagination.CurrentPage < pagination.TotalPages ? pagination.CurrentPage + 1 : 0;
    }

    /// <summary>
    /// Gets the previous page number that can be navigated to.
    /// </summary>
    /// <param name="pagination">The pagination instance.</param>
    /// <returns>The previous page number, or 0 if no previous page exists.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="pagination"/> is null.</exception>
    public static int GetPreviousPage(this Pagination pagination)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        return pagination.CurrentPage > 1 ? pagination.CurrentPage - 1 : 0;
    }

    /// <summary>
    /// Gets the first page number.
    /// </summary>
    /// <param name="pagination">The pagination instance.</param>
    /// <returns>The first page number (always 1).</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="pagination"/> is null.</exception>
    public static int GetFirstPage(this Pagination pagination)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        return 1;
    }

    /// <summary>
    /// Gets the last page number.
    /// </summary>
    /// <param name="pagination">The pagination instance.</param>
    /// <returns>The last page number.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="pagination"/> is null.</exception>
    public static int GetLastPage(this Pagination pagination)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        return pagination.TotalPages;
    }

    /// <summary>
    /// Determines whether there is a next page available.
    /// </summary>
    /// <param name="pagination">The pagination instance.</param>
    /// <returns>True if there is a next page; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="pagination"/> is null.</exception>
    public static bool HasNextPage(this Pagination pagination)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        return pagination.CurrentPage < pagination.TotalPages;
    }

    /// <summary>
    /// Determines whether there is a previous page available.
    /// </summary>
    /// <param name="pagination">The pagination instance.</param>
    /// <returns>True if there is a previous page; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="pagination"/> is null.</exception>
    public static bool HasPreviousPage(this Pagination pagination)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        return pagination.CurrentPage > 1;
    }

    /// <summary>
    /// Determines whether the pagination has multiple pages.
    /// </summary>
    /// <param name="pagination">The pagination instance.</param>
    /// <returns>True if there are multiple pages; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="pagination"/> is null.</exception>
    public static bool HasMultiplePages(this Pagination pagination)
    {
        ArgumentNullException.ThrowIfNull(pagination);

        return pagination.TotalPages > 1;
    }
}