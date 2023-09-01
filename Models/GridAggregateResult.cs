// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorComponentLibrary.Models;

/// <summary>
/// Statistical function to compute over the non-null cell values of a column within the
/// currently filtered and searched result set.
/// Used in <see cref="GridAggregateRequest"/> to describe what the server should compute
/// for the virtualized grid's optional summary footer row.
/// </summary>
public enum AggregateFunction
{
    /// <summary>
    /// Total number of rows in the filtered result set, including rows where the target
    /// field is absent or <c>null</c>.
    /// </summary>
    Count,

    /// <summary>
    /// Arithmetic sum of all non-null numeric cell values.
    /// Returns <c>null</c> when no values can be converted to a number.
    /// </summary>
    Sum,

    /// <summary>
    /// Arithmetic mean of all non-null numeric cell values.
    /// Returns <c>null</c> when no values can be converted to a number.
    /// </summary>
    Average,

    /// <summary>
    /// Lowest value among all non-null comparable cell values.
    /// Uses native <see cref="IComparable"/> ordering when available; string ordering otherwise.
    /// </summary>
    Min,

    /// <summary>
    /// Highest value among all non-null comparable cell values.
    /// Uses native <see cref="IComparable"/> ordering when available; string ordering otherwise.
    /// </summary>
    Max,

    /// <summary>
    /// Count of unique non-null cell values using ordinal case-insensitive string comparison
    /// on the values' string representations.
    /// </summary>
    DistinctCount
}

/// <summary>
/// Pairs a column key with the statistical function to apply to that column's values
/// when computing aggregates for a virtualized grid footer row.
/// </summary>
/// <param name="Field">
///   The <see cref="DataTableColumn.Key"/> of the column whose values should be aggregated.
/// </param>
/// <param name="Function">The aggregate function to compute over the column's cell values.</param>
public sealed record GridColumnAggregateDescriptor(
    [property: Required] string Field,
    AggregateFunction Function
);

/// <summary>
/// Describes a server-side aggregate computation over the filtered and searched rows of a
/// virtualized grid table.
/// <para>
/// The <see cref="Filters"/>, <see cref="SearchTerm"/>, and <see cref="SearchFields"/> should
/// mirror those in the corresponding <see cref="GridVirtualRequest"/> so that the footer totals
/// always reflect the same logical row set visible in the current scroll view.
/// </para>
/// </summary>
public sealed record GridAggregateRequest
{
    /// <summary>Identifier of the source data table to aggregate.</summary>
    [JsonPropertyName("tableId")]
    public int TableId { get; init; }

    /// <summary>
    /// Column filters applied to the rows before computing aggregates.
    /// Mirrors the <see cref="GridVirtualRequest.Filters"/> of the current query.
    /// An empty list includes all rows.
    /// </summary>
    [JsonPropertyName("filters")]
    public IReadOnlyList<GridFilterDescriptor> Filters { get; init; } = [];

    /// <summary>
    /// Optional free-text search term applied before aggregation.
    /// Mirrors <see cref="GridVirtualRequest.SearchTerm"/> of the current query.
    /// </summary>
    [JsonPropertyName("searchTerm")]
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Column keys scoped for the full-text search. <c>null</c> searches every field.
    /// Mirrors <see cref="GridVirtualRequest.SearchFields"/> of the current query.
    /// </summary>
    [JsonPropertyName("searchFields")]
    public IReadOnlyList<string>? SearchFields { get; init; }

    /// <summary>
    /// Specifies which columns to aggregate and which function to apply to each.
    /// Multiple descriptors targeting the same column with different functions are all
    /// evaluated independently, each producing a separate entry in the result dictionary.
    /// </summary>
    [JsonPropertyName("aggregates")]
    public IReadOnlyList<GridColumnAggregateDescriptor> Aggregates { get; init; } = [];
}

/// <summary>
/// Result returned by <see cref="Services.IGridAggregationService.ComputeAggregatesAsync"/>.
/// Contains the computed aggregate value for every requested column–function pair plus
/// diagnostic metadata about the computation.
/// </summary>
public sealed record GridAggregateResult
{
    /// <summary>
    /// Dictionary of computed aggregate values keyed by <c>"{field}:{function}"</c>
    /// (e.g., <c>"price:Sum"</c>, <c>"status:DistinctCount"</c>, <c>"createdAt:Max"</c>).
    /// <para>
    /// A key maps to <c>null</c> when the function could not be computed — for example, when
    /// <see cref="AggregateFunction.Sum"/> is requested for a non-numeric column.
    /// <see cref="AggregateFunction.Count"/> is always non-null.
    /// </para>
    /// </summary>
    [JsonPropertyName("values")]
    public IReadOnlyDictionary<string, object?> Values { get; init; } =
        new Dictionary<string, object?>();

    /// <summary>
    /// Number of rows included in the aggregate computation (after all filters and search
    /// conditions have been applied).
    /// </summary>
    [JsonPropertyName("rowCount")]
    public int RowCount { get; init; }

    /// <summary>Wall-clock duration the server spent fetching rows and computing aggregates.</summary>
    [JsonPropertyName("queryDuration")]
    public TimeSpan QueryDuration { get; init; }
}
