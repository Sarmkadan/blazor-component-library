using BlazorComponentLibrary.Components.Pagination;
using FluentAssertions;
using Xunit;

/// <summary>
/// Tests for the Pagination component's page item generation logic.
/// </summary>
public class PaginationLogicTests
{
    /// <summary>
    /// Verifies that a single-page result contains exactly that page.
    /// </summary>
    [Fact]
    public void PageItems_WithSinglePage_ShouldContainOnlyFirstPage()
    {
        var pagination = new Pagination { CurrentPage = 1, TotalPages = 1 };

        pagination.PageItems.Should().Equal(
            new Pagination.PageItem(1, Pagination.PageItemType.Page));
    }

    /// <summary>
    /// Verifies that small page counts are displayed sequentially without ellipses.
    /// </summary>
    [Fact]
    public void PageItems_WithSmallPageCount_ShouldContainSequentialPagesWithoutEllipsis()
    {
        var pagination = new Pagination { CurrentPage = 3, TotalPages = 5 };

        pagination.PageItems.Should().Equal(
            Page(1),
            Page(2),
            Page(3),
            Page(4),
            Page(5));
    }

    /// <summary>
    /// Verifies that a middle current page has ellipses on both sides.
    /// </summary>
    [Fact]
    public void PageItems_WithCurrentPageInMiddle_ShouldContainBothEllipses()
    {
        var pagination = new Pagination { CurrentPage = 5, TotalPages = 10 };

        pagination.PageItems.Should().Equal(
            Page(1),
            Ellipsis(),
            Page(4),
            Page(5),
            Page(6),
            Ellipsis(),
            Page(10));
    }

    /// <summary>
    /// Verifies that a current page near the start has only a right ellipsis.
    /// </summary>
    [Fact]
    public void PageItems_WithCurrentPageNearStart_ShouldContainOnlyRightEllipsis()
    {
        var pagination = new Pagination { CurrentPage = 2, TotalPages = 10 };

        pagination.PageItems.Should().Equal(
            Page(1),
            Page(2),
            Page(3),
            Ellipsis(),
            Page(10));
    }

    /// <summary>
    /// Verifies that the final current page has only a left ellipsis.
    /// </summary>
    [Fact]
    public void PageItems_WithCurrentPageAtEnd_ShouldContainOnlyLeftEllipsis()
    {
        var pagination = new Pagination { CurrentPage = 10, TotalPages = 10 };

        pagination.PageItems.Should().Equal(
            Page(1),
            Ellipsis(),
            Page(9),
            Page(10));
    }

    /// <summary>
    /// Verifies that the first and last page numbers are retained for every current page.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(15)]
    public void PageItems_WithManyPages_ShouldAlwaysContainFirstAndLastPages(int currentPage)
    {
        var pagination = new Pagination { CurrentPage = currentPage, TotalPages = 15 };

        var pageNumbers = pagination.PageItems
            .Where(item => item.Type == Pagination.PageItemType.Page)
            .Select(item => item.Number);

        pageNumbers.Should().Contain(1).And.Contain(15);
    }

    /// <summary>
    /// Verifies that increasing the sibling count widens the range around the current page.
    /// </summary>
    [Fact]
    public void PageItems_WithLargerSiblingCount_ShouldWidenMiddleRange()
    {
        var pagination = new Pagination
        {
            CurrentPage = 10,
            TotalPages = 20,
            SiblingCount = 3
        };

        pagination.PageItems.Should().Equal(
            Page(1),
            Ellipsis(),
            Page(7),
            Page(8),
            Page(9),
            Page(10),
            Page(11),
            Page(12),
            Page(13),
            Ellipsis(),
            Page(20));
    }

    private static Pagination.PageItem Page(int number) =>
        new(number, Pagination.PageItemType.Page);

    private static Pagination.PageItem Ellipsis() =>
        new(0, Pagination.PageItemType.Ellipsis);
}
