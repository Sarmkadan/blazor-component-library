// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorComponentLibrary.Models;

/// <summary>
/// Comparison operators available when building column filters for the virtualized grid.
/// The engine evaluates each filter by comparing the cell's raw value against
/// <see cref="GridFilterDescriptor.Value"/> using the selected operator.
/// </summary>
public enum FilterOperator
{
    /// <summary>Cell value equals the filter value (case behaviour controlled by <see cref="GridFilterDescriptor.CaseSensitive"/>).</summary>
    Equals,

    /// <summary>Cell value does not equal the filter value.</summary>
    NotEquals,

    /// <summary>String cell value contains the filter value as a substring.</summary>
    Contains,

    /// <summary>String cell value begins with the filter value.</summary>
    StartsWith,

    /// <summary>String cell value ends with the filter value.</summary>
    EndsWith,

    /// <summary>Numeric or date cell value is strictly greater than the filter value.</summary>
    GreaterThan,

    /// <summary>Numeric or date cell value is strictly less than the filter value.</summary>
    LessThan,

    /// <summary>Numeric or date cell value is greater than or equal to the filter value.</summary>
    GreaterThanOrEqual,

    /// <summary>Numeric or date cell value is less than or equal to the filter value.</summary>
    LessThanOrEqual,

    /// <summary>Cell value is <c>null</c> or the field key is absent from the row's data dictionary.</summary>
    IsNull,

    /// <summary>Cell value is present and not <c>null</c>.</summary>
    IsNotNull,

    /// <summary>
    /// Cell value matches at least one entry in a comma-separated allowlist supplied as
    /// <see cref="GridFilterDescriptor.Value"/>.
    /// </summary>
    In
}

/// <summary>
/// Sort direction for a <see cref="GridSortDescriptor"/>.
/// </summary>
public enum SortDirection
{
    /// <summary>Ascending order: A → Z, 0 → 9, oldest → newest.</summary>
    Ascending,

    /// <summary>Descending order: Z → A, 9 → 0, newest → oldest.</summary>
    Descending
}

/// <summary>
/// Describes a single active filter applied to a named grid column.
/// Multiple filters in a <see cref="GridVirtualRequest"/> are combined as a logical AND.
/// </summary>
/// <param name="Field">The <see cref="DataTableColumn.Key"/> of the column to filter.</param>
/// <param name="Operator">Comparison operator used to evaluate each row's cell value.</param>
/// <param name="Value">
///   Value to compare against.  May be <c>null</c> when <paramref name="Operator"/> is
///   <see cref="FilterOperator.IsNull"/> or <see cref="FilterOperator.IsNotNull"/>.
///   For <see cref="FilterOperator.In"/>, supply a comma-separated string of allowed values.
/// </param>
/// <param name="CaseSensitive">
///   When <c>true</c>, string comparisons are case-sensitive.
///   Defaults to <c>false</c> (ordinal ignore-case).
/// </param>
public record GridFilterDescriptor(
    [property: Required] string Field,
    FilterOperator Operator,
    object? Value,
    bool CaseSensitive = false
);

/// <summary>
/// Describes one ordering clause within a multi-column sort.
/// The query engine applies all active sorts in ascending <see cref="Priority"/> order so
/// that the descriptor with priority 0 is the primary sort key.
/// </summary>
/// <param name="Field">The <see cref="DataTableColumn.Key"/> to order by.</param>
/// <param name="Direction">Sort direction. Defaults to <see cref="SortDirection.Ascending"/>.</param>
/// <param name="Priority">
///   Tie-breaking rank among simultaneous sorts.
///   Lower values take precedence (0 = primary, 1 = secondary, …).
/// </param>
public record GridSortDescriptor(
    [property: Required] string Field,
    SortDirection Direction = SortDirection.Ascending,
    int Priority = 0
);

/// <summary>
/// Fully describes the virtual window of rows a client is requesting, together with all
/// active filters, multi-column sorts, and an optional full-text search term.
/// Sent as the request body to the grid query endpoint.
/// </summary>
public sealed record GridVirtualRequest
{
    /// <summary>
    /// Zero-based index of the first row to include from the filtered, sorted result set.
    /// The server clamps this value to the available row range.
    /// </summary>
    [Range(0, int.MaxValue)]
    [JsonPropertyName("startIndex")]
    public int StartIndex { get; init; }

    /// <summary>
    /// Maximum number of rows to return.
    /// Should not exceed the <see cref="VirtualScrollConfig.PageSize"/> configured for the grid.
    /// </summary>
    [Range(1, 500)]
    [JsonPropertyName("count")]
    public int Count { get; init; } = 50;

    /// <summary>
    /// Active column filters. An empty list returns all rows.
    /// Multiple filters are evaluated as a logical AND — a row must satisfy every filter
    /// to appear in the result window.
    /// </summary>
    [JsonPropertyName("filters")]
    public IReadOnlyList<GridFilterDescriptor> Filters { get; init; } = [];

    /// <summary>
    /// Active column sort descriptors.  Multiple entries enable multi-column sorting;
    /// they are applied in ascending <see cref="GridSortDescriptor.Priority"/> order.
    /// </summary>
    [JsonPropertyName("sorts")]
    public IReadOnlyList<GridSortDescriptor> Sorts { get; init; } = [];

    /// <summary>
    /// Optional free-text search term.  When non-empty, each row must contain this term
    /// (case-insensitive substring) in at least one of the <see cref="SearchFields"/>.
    /// </summary>
    [JsonPropertyName("searchTerm")]
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Column keys scoped for full-text search.  When <c>null</c> or empty, all string-valued
    /// fields present in each row's data dictionary are searched.
    /// </summary>
    [JsonPropertyName("searchFields")]
    public IReadOnlyList<string>? SearchFields { get; init; }
}

/// <summary>
/// The paginated window returned by a virtual grid query, containing the requested rows
/// together with result-set metadata needed by the client to drive further scrolling.
/// </summary>
/// <typeparam name="T">The row element type — typically <see cref="DataTableRow"/>.</typeparam>
public sealed record GridVirtualResult<T>
{
    /// <summary>Rows for the requested virtual window in their final display order.</summary>
    [JsonPropertyName("items")]
    public IReadOnlyList<T> Items { get; init; } = [];

    /// <summary>Total row count in the data source <em>before</em> any filters are applied.</summary>
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; init; }

    /// <summary>Row count after all active column filters and full-text search are applied.</summary>
    [JsonPropertyName("filteredCount")]
    public int FilteredCount { get; init; }

    /// <summary>
    /// Zero-based index of <see cref="Items"/>[0] within the fully filtered and sorted result set.
    /// Equals the requested <see cref="GridVirtualRequest.StartIndex"/> unless clamped.
    /// </summary>
    [JsonPropertyName("startIndex")]
    public int StartIndex { get; init; }

    /// <summary><c>true</c> when rows beyond the current window still exist in the filtered set.</summary>
    [JsonPropertyName("hasMore")]
    public bool HasMore { get; init; }

    /// <summary>Wall-clock duration the server spent computing this result window.</summary>
    [JsonPropertyName("queryDuration")]
    public TimeSpan QueryDuration { get; init; }

    /// <summary><c>true</c> when the result was returned from a cache entry rather than recomputed.</summary>
    [JsonPropertyName("fromCache")]
    public bool FromCache { get; init; }
}

/// <summary>
/// Describes a single cell-level inline edit to be applied to a specific row and column.
/// </summary>
/// <param name="RowId">Identifier of the <see cref="DataTableRow"/> to edit.</param>
/// <param name="Field">Column key (<see cref="DataTableColumn.Key"/>) of the cell to update.</param>
/// <param name="NewValue">The new value to write into the cell after validation passes.</param>
/// <param name="OriginalValue">
///   Optional value the client observed before editing.
///   Used as an optimistic-concurrency hint: the service rejects the edit with a conflict
///   error if the currently stored value no longer matches this expectation.
/// </param>
public record GridEditRequest(
    [property: Required] int    RowId,
    [property: Required] string Field,
    object? NewValue,
    object? OriginalValue = null
);

/// <summary>
/// Outcome of applying a <see cref="GridEditRequest"/>, returned by both single-edit
/// and batch-edit endpoints.
/// </summary>
/// <param name="Success">Whether the edit was validated and committed successfully.</param>
/// <param name="RowId">The row that was targeted by the edit.</param>
/// <param name="Field">The column key that was targeted by the edit.</param>
/// <param name="AppliedValue">
///   The value actually written to the cell.
///   May differ from <see cref="GridEditRequest.NewValue"/> if the service coerced the input.
/// </param>
/// <param name="Error">
///   Human-readable description of the validation or persistence failure when
///   <paramref name="Success"/> is <c>false</c>. <c>null</c> on success.
/// </param>
public record GridEditResult(
    bool    Success,
    int     RowId,
    string  Field,
    object? AppliedValue,
    string? Error = null
);

/// <summary>
/// Immutable snapshot of a single committed edit, appended to the per-table undo stack
/// maintained by <see cref="Services.IVirtualizedGridService"/>.
/// </summary>
/// <param name="RowId">Row that was modified by the edit.</param>
/// <param name="Field">Column key that was modified by the edit.</param>
/// <param name="PreviousValue">Cell value <em>before</em> the edit was applied (used to restore on undo).</param>
/// <param name="NewValue">Cell value <em>after</em> the edit was applied.</param>
/// <param name="Timestamp">UTC instant at which the edit was committed to the repository.</param>
public record GridEditHistoryEntry(
    int      RowId,
    string   Field,
    object?  PreviousValue,
    object?  NewValue,
    DateTime Timestamp
);
