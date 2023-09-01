// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Diagnostics;
using BlazorComponentLibrary.Infrastructure;
using BlazorComponentLibrary.Models;
using BlazorComponentLibrary.Repositories;
using Microsoft.Extensions.Logging;

namespace BlazorComponentLibrary.Services;

/// <summary>
/// Contract for computing column-level statistical aggregates over the filtered row set
/// of a virtualized grid table.
/// <para>
/// Implementations share the same filter and search pipeline used by
/// <see cref="IVirtualizedGridService"/> so that footer totals always reflect the logical
/// row set visible in the current scroll view without duplicating predicate logic.
/// </para>
/// </summary>
public interface IGridAggregationService
{
    /// <summary>
    /// Applies the filter and search conditions from <paramref name="request"/> to the table's
    /// rows and then computes the nominated aggregate function for each requested column.
    /// </summary>
    /// <param name="request">
    ///   Aggregation parameters: table identifier, active filters, optional search term,
    ///   and a list of <see cref="GridColumnAggregateDescriptor"/> pairs describing which
    ///   columns to aggregate and with which <see cref="AggregateFunction"/>.
    /// </param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>
    ///   A <see cref="GridAggregateResult"/> with one entry per column–function pair in
    ///   <see cref="GridAggregateResult.Values"/>, keyed as <c>"{field}:{function}"</c>.
    ///   Values that cannot be computed (e.g., Sum over a non-numeric column) are <c>null</c>.
    /// </returns>
    Task<GridAggregateResult> ComputeAggregatesAsync(
        GridAggregateRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Default <see cref="IGridAggregationService"/> implementation.
/// <para>
/// After applying the same filter and search pipeline used by <see cref="VirtualizedGridService"/>,
/// this service iterates over each requested column–function pair and computes the aggregate by
/// extracting raw cell values from <see cref="DataTableRow.Data"/>.  Numeric functions
/// (<see cref="AggregateFunction.Sum"/>, <see cref="AggregateFunction.Average"/>) attempt
/// <see cref="Convert.ToDouble"/> on each value and return <c>null</c> gracefully when the
/// conversion fails.  Comparable functions (<see cref="AggregateFunction.Min"/>,
/// <see cref="AggregateFunction.Max"/>) use native <see cref="IComparable"/> ordering with a
/// string fallback.
/// </para>
/// </summary>
public sealed class GridAggregationService : IGridAggregationService
{
    private readonly IDataRepository _dataRepository;
    private readonly ILogger<GridAggregationService> _logger;

    /// <summary>
    /// Initialises a new <see cref="GridAggregationService"/>.
    /// </summary>
    /// <param name="dataRepository">Repository supplying the raw rows for each computation.</param>
    /// <param name="logger">Structured logger for query duration telemetry.</param>
    public GridAggregationService(
        IDataRepository dataRepository,
        ILogger<GridAggregationService> logger)
    {
        _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
        _logger         = logger         ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<GridAggregateResult> ComputeAggregatesAsync(
        GridAggregateRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var sw = Stopwatch.StartNew();

        var allRows = (await _dataRepository.GetRowsByTableIdAsync(request.TableId)).ToList();

        var filtered = allRows
            .ApplyGridFilters(request.Filters)
            .ApplyGridSearch(request.SearchTerm, request.SearchFields)
            .ToList();

        var values = new Dictionary<string, object?>(request.Aggregates.Count);

        foreach (var descriptor in request.Aggregates)
        {
            var key    = $"{descriptor.Field}:{descriptor.Function}";
            values[key] = ComputeAggregate(filtered, descriptor.Field, descriptor.Function);
        }

        sw.Stop();

        _logger.LogDebug(
            "Aggregates computed — table={TableId} rows={Rows} functions={Count} in {Ms} ms.",
            request.TableId, filtered.Count, request.Aggregates.Count, sw.ElapsedMilliseconds);

        return new GridAggregateResult
        {
            Values        = values,
            RowCount      = filtered.Count,
            QueryDuration = sw.Elapsed
        };
    }

    // ── Private computation helpers ───────────────────────────────────────────

    private static object? ComputeAggregate(
        IReadOnlyList<DataTableRow> rows,
        string field,
        AggregateFunction function)
    {
        if (function == AggregateFunction.Count)
            return rows.Count;

        var cellValues = rows
            .Where(r => r.Data.TryGetValue(field, out var v) && v is not null)
            .Select(r => r.Data[field]!)
            .ToList();

        return function switch
        {
            AggregateFunction.DistinctCount => cellValues
                .Select(v => v.ToString())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count(),

            AggregateFunction.Sum     => TryNumericAggregate(cellValues, nums => nums.Sum()),
            AggregateFunction.Average => TryNumericAggregate(cellValues, nums => nums.Average()),
            AggregateFunction.Min     => TryComparableAggregate(cellValues, isMin: true),
            AggregateFunction.Max     => TryComparableAggregate(cellValues, isMin: false),
            _                         => null
        };
    }

    /// <summary>
    /// Converts each value to <see cref="double"/> and applies <paramref name="aggregator"/>.
    /// Returns <c>null</c> when the list is empty or any conversion fails for the whole set.
    /// </summary>
    private static double? TryNumericAggregate(
        IReadOnlyList<object> values,
        Func<IEnumerable<double>, double> aggregator)
    {
        if (values.Count == 0)
            return null;

        try
        {
            var numbers = values.Select(v => Convert.ToDouble(v)).ToList();
            return aggregator(numbers);
        }
        catch (InvalidCastException) { return null; }
        catch (FormatException)      { return null; }
        catch (OverflowException)    { return null; }
    }

    /// <summary>
    /// Finds the minimum or maximum value by walking the list with <see cref="IComparable"/>
    /// native ordering, falling back to ordinal string comparison for incompatible types.
    /// Returns <c>null</c> when the list is empty.
    /// </summary>
    private static object? TryComparableAggregate(IReadOnlyList<object> values, bool isMin)
    {
        if (values.Count == 0)
            return null;

        object? result = null;

        foreach (var value in values)
        {
            if (result is null)
            {
                result = value;
                continue;
            }

            bool isBetter;

            if (value is IComparable comparable)
            {
                try
                {
                    int cmp = comparable.CompareTo(result);
                    isBetter = isMin ? cmp < 0 : cmp > 0;
                }
                catch (ArgumentException)
                {
                    int cmp = string.Compare(
                        value.ToString(), result.ToString(), StringComparison.Ordinal);
                    isBetter = isMin ? cmp < 0 : cmp > 0;
                }
            }
            else
            {
                int cmp = string.Compare(
                    value.ToString(), result.ToString(), StringComparison.Ordinal);
                isBetter = isMin ? cmp < 0 : cmp > 0;
            }

            if (isBetter)
                result = value;
        }

        return result;
    }
}
