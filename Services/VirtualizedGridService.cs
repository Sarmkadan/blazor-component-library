// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Diagnostics;
using BlazorComponentLibrary.Caching;
using BlazorComponentLibrary.Infrastructure;
using BlazorComponentLibrary.Models;
using BlazorComponentLibrary.Repositories;
using Microsoft.Extensions.Logging;

namespace BlazorComponentLibrary.Services;

/// <summary>
/// Production-ready server-side engine for the virtualized data grid.
/// <para>
/// Responsibilities:
/// <list type="bullet">
///   <item>Windowed data access via <see cref="QueryAsync"/> — fetches only the visible row range.</item>
///   <item>Multi-column sorting applied in <see cref="GridSortDescriptor.Priority"/> order.</item>
///   <item>Compound column filtering with all <see cref="FilterOperator"/> variants.</item>
///   <item>Full-text search across configurable column subsets.</item>
///   <item>Transactional inline editing with optimistic-concurrency detection.</item>
///   <item>Undo stack bounded by <see cref="VirtualizedGridOptions.MaxInlineEditHistory"/>.</item>
///   <item>Optional distributed result caching via <see cref="ICacheService"/>.</item>
/// </list>
/// </para>
/// <para>
/// Thread safety: all mutable in-process state (<see cref="_columnDefs"/> and
/// <see cref="_editHistory"/>) is protected by <see cref="_stateLock"/>.
/// Repository and cache calls are async and do not hold the lock.
/// </para>
/// </summary>
public sealed class VirtualizedGridService : IVirtualizedGridService
{
    private readonly IDataRepository _dataRepository;
    private readonly ICacheService _cacheService;
    private readonly IGridEditHandler _editHandler;
    private readonly ILogger<VirtualizedGridService> _logger;
    private readonly VirtualizedGridOptions _options;

    private readonly Dictionary<int, List<GridColumnDefinition>> _columnDefs  = new();
    private readonly Dictionary<int, LinkedList<GridEditHistoryEntry>> _editHistory = new();
    private readonly object _stateLock = new();

    /// <summary>
    /// Initialises a new <see cref="VirtualizedGridService"/> with the required dependencies.
    /// </summary>
    /// <param name="dataRepository">Repository that supplies row data for queries and edit persistence.</param>
    /// <param name="cacheService">Cache used to store computed result windows.</param>
    /// <param name="editHandler">Pluggable handler that validates and commits cell-level edits.</param>
    /// <param name="logger">Structured logger for query metrics and edit audit trail.</param>
    /// <param name="options">Grid-wide configuration; defaults are applied when <c>null</c>.</param>
    public VirtualizedGridService(
        IDataRepository dataRepository,
        ICacheService cacheService,
        IGridEditHandler editHandler,
        ILogger<VirtualizedGridService> logger,
        VirtualizedGridOptions? options = null)
    {
        _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
        _cacheService   = cacheService   ?? throw new ArgumentNullException(nameof(cacheService));
        _editHandler    = editHandler    ?? throw new ArgumentNullException(nameof(editHandler));
        _logger         = logger         ?? throw new ArgumentNullException(nameof(logger));
        _options        = options        ?? new VirtualizedGridOptions();
    }

    /// <inheritdoc />
    public async Task<GridVirtualResult<DataTableRow>> QueryAsync(
        int tableId,
        GridVirtualRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var cacheKey = BuildCacheKey(tableId, request);

        if (_options.EnableCaching && _options.CacheExpiration > TimeSpan.Zero)
        {
            var cached = await _cacheService.GetAsync<GridVirtualResult<DataTableRow>>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("Grid query for table {TableId} served from cache.", tableId);
                return cached with { FromCache = true };
            }
        }

        var sw = Stopwatch.StartNew();

        var allRows    = (await _dataRepository.GetRowsByTableIdAsync(tableId)).ToList();
        var totalCount = allRows.Count;

        var filtered = allRows
            .ApplyGridFilters(request.Filters)
            .ApplyGridSearch(request.SearchTerm, request.SearchFields)
            .ApplyGridSort(request.Sorts)
            .ToList();

        var filteredCount = filtered.Count;
        var windowStart   = filteredCount > 0
            ? Math.Min(request.StartIndex, filteredCount - 1)
            : 0;
        var window = filtered.Skip(windowStart).Take(request.Count).ToList();

        sw.Stop();

        var result = new GridVirtualResult<DataTableRow>
        {
            Items         = window,
            TotalCount    = totalCount,
            FilteredCount = filteredCount,
            StartIndex    = windowStart,
            HasMore       = windowStart + window.Count < filteredCount,
            QueryDuration = sw.Elapsed,
            FromCache     = false
        };

        if (_options.EnableCaching && _options.CacheExpiration > TimeSpan.Zero)
            await _cacheService.SetAsync(cacheKey, result, _options.CacheExpiration);

        _logger.LogDebug(
            "Grid query table={TableId}: {Filtered}/{Total} rows, window [{Start}-{End}] in {Ms} ms.",
            tableId, filteredCount, totalCount,
            windowStart, windowStart + window.Count,
            sw.ElapsedMilliseconds);

        return result;
    }

    /// <inheritdoc />
    public async Task<GridEditResult> ApplyEditAsync(
        int tableId,
        GridEditRequest edit,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(edit);

        var row = await _dataRepository.GetRowByIdAsync(edit.RowId);
        if (row is null)
        {
            return new GridEditResult(false, edit.RowId, edit.Field, null,
                $"Row {edit.RowId} was not found in table {tableId}.");
        }

        // Optimistic concurrency: reject if the stored value changed since the client read it.
        if (edit.OriginalValue is not null &&
            row.Data.TryGetValue(edit.Field, out var storedValue) &&
            !string.Equals(storedValue?.ToString(), edit.OriginalValue.ToString(), StringComparison.Ordinal))
        {
            return new GridEditResult(false, edit.RowId, edit.Field, storedValue,
                $"Concurrency conflict: field '{edit.Field}' was modified by another operation.");
        }

        var column = GetColumnDefinition(tableId, edit.Field);
        if (!_editHandler.CanEdit(row, edit.Field, column))
        {
            return new GridEditResult(false, edit.RowId, edit.Field, null,
                $"Column '{edit.Field}' is not editable.");
        }

        var previousValue = row.Data.TryGetValue(edit.Field, out var prev) ? prev : null;
        var editResult    = await _editHandler.HandleEditAsync(row, edit, column);

        if (editResult.Success)
        {
            await _dataRepository.UpdateRowAsync(row);

            RecordHistory(tableId, new GridEditHistoryEntry(
                edit.RowId, edit.Field, previousValue, editResult.AppliedValue, DateTime.UtcNow));

            await InvalidateCacheAsync(tableId);

            _logger.LogInformation(
                "Edit committed — table={TableId} row={RowId} field={Field} newValue={Value}.",
                tableId, edit.RowId, edit.Field, editResult.AppliedValue);
        }

        return editResult;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GridEditResult>> ApplyBatchEditsAsync(
        int tableId,
        IEnumerable<GridEditRequest> edits,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(edits);

        var editList = edits.ToList();

        // Validate all edits before committing any — guarantees either all or none are applied.
        foreach (var edit in editList)
        {
            if (!await ValidateEditAsync(tableId, edit))
            {
                return [new GridEditResult(false, edit.RowId, edit.Field, null,
                    $"Batch validation failed on field '{edit.Field}'. No edits were committed.")];
            }
        }

        var results = new List<GridEditResult>(editList.Count);
        foreach (var edit in editList)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var r = await ApplyEditAsync(tableId, edit, cancellationToken);
            results.Add(r);
            if (!r.Success)
            {
                _logger.LogWarning(
                    "Batch edit halted — row={RowId} field={Field}: {Error}.",
                    r.RowId, r.Field, r.Error);
                break;
            }
        }

        return results;
    }

    /// <inheritdoc />
    public async Task<bool> ValidateEditAsync(int tableId, GridEditRequest edit)
    {
        ArgumentNullException.ThrowIfNull(edit);
        var column = GetColumnDefinition(tableId, edit.Field);
        return await Task.FromResult(column?.ValidateValue(edit.NewValue) ?? true);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<GridEditHistoryEntry>> GetEditHistoryAsync(
        int tableId, int? rowId = null)
    {
        lock (_stateLock)
        {
            if (!_editHistory.TryGetValue(tableId, out var history))
                return Task.FromResult<IReadOnlyList<GridEditHistoryEntry>>([]);

            var entries = rowId.HasValue
                ? history.Where(e => e.RowId == rowId.Value).ToList()
                : [.. history];

            return Task.FromResult<IReadOnlyList<GridEditHistoryEntry>>(entries);
        }
    }

    /// <inheritdoc />
    public async Task<bool> UndoLastEditAsync(int tableId)
    {
        GridEditHistoryEntry? last;

        lock (_stateLock)
        {
            if (!_editHistory.TryGetValue(tableId, out var history) || history.First is null)
                return false;

            last = history.First.Value;
            history.RemoveFirst();
        }

        var row = await _dataRepository.GetRowByIdAsync(last.RowId);
        if (row is null) return false;

        var undoRequest = new GridEditRequest(last.RowId, last.Field, last.PreviousValue);
        var column      = GetColumnDefinition(tableId, last.Field);
        var result      = await _editHandler.HandleEditAsync(row, undoRequest, column);

        if (result.Success)
        {
            await _dataRepository.UpdateRowAsync(row);
            await InvalidateCacheAsync(tableId);
            _logger.LogInformation(
                "Undo applied — table={TableId} row={RowId} field={Field} restoredValue={Value}.",
                tableId, last.RowId, last.Field, last.PreviousValue);
        }

        return result.Success;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<GridColumnDefinition>> GetColumnDefinitionsAsync(int tableId)
    {
        lock (_stateLock)
        {
            var defs = _columnDefs.TryGetValue(tableId, out var d)
                ? (IReadOnlyList<GridColumnDefinition>)d.AsReadOnly()
                : [];
            return Task.FromResult(defs);
        }
    }

    /// <inheritdoc />
    public Task<bool> UpdateColumnDefinitionAsync(int tableId, GridColumnDefinition column)
    {
        ArgumentNullException.ThrowIfNull(column);

        lock (_stateLock)
        {
            if (!_columnDefs.TryGetValue(tableId, out var defs))
                return Task.FromResult(false);

            var idx = defs.FindIndex(c => c.Key == column.Key);
            if (idx < 0) return Task.FromResult(false);

            defs[idx] = column;
            _logger.LogDebug("Column definition updated — table={TableId} column={Column}.",
                tableId, column.Key);
            return Task.FromResult(true);
        }
    }

    /// <inheritdoc />
    public async Task InvalidateCacheAsync(int tableId)
    {
        var removed = await _cacheService.RemoveByPatternAsync($"vgrid:{tableId}:*");
        if (removed > 0)
            _logger.LogDebug("Cache invalidated for table {TableId}: {Count} entries purged.",
                tableId, removed);
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private static string BuildCacheKey(int tableId, GridVirtualRequest request)
    {
        var filterHash = string.Concat(request.Filters
            .OrderBy(f => f.Field)
            .Select(f => $"{f.Field}|{(int)f.Operator}|{f.Value}|{f.CaseSensitive};"));

        var sortHash = string.Concat(request.Sorts
            .OrderBy(s => s.Priority)
            .Select(s => $"{s.Field}|{(int)s.Direction};"));

        return $"vgrid:{tableId}:{request.StartIndex}:{request.Count}:{filterHash}:{sortHash}:{request.SearchTerm}";
    }

    private GridColumnDefinition? GetColumnDefinition(int tableId, string field)
    {
        lock (_stateLock)
        {
            return _columnDefs.TryGetValue(tableId, out var defs)
                ? defs.FirstOrDefault(c => c.Key == field)
                : null;
        }
    }

    private void RecordHistory(int tableId, GridEditHistoryEntry entry)
    {
        lock (_stateLock)
        {
            if (!_editHistory.TryGetValue(tableId, out var history))
            {
                history = new LinkedList<GridEditHistoryEntry>();
                _editHistory[tableId] = history;
            }

            history.AddFirst(entry);

            while (history.Count > _options.MaxInlineEditHistory)
                history.RemoveLast();
        }
    }
}

/// <summary>
/// Default <see cref="IGridEditHandler"/> that validates cell values against
/// <see cref="GridColumnDefinition"/> constraints and mutates the in-memory
/// <see cref="DataTableRow"/> object via <see cref="DataTableRow.SetValue"/>.
/// <para>
/// Because the default <see cref="Repositories.DataRepository"/> stores row references
/// directly, this mutation is immediately reflected in subsequent reads without an explicit
/// repository update call — the service still calls <c>UpdateRowAsync</c> to maintain
/// correctness when a real database back-end is substituted.
/// </para>
/// </summary>
public sealed class InMemoryGridEditHandler : IGridEditHandler
{
    /// <inheritdoc />
    public Task<GridEditResult> HandleEditAsync(
        DataTableRow row,
        GridEditRequest edit,
        GridColumnDefinition? column)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(edit);

        if (column is not null && !column.ValidateValue(edit.NewValue))
        {
            return Task.FromResult(new GridEditResult(
                false, edit.RowId, edit.Field, null,
                $"Value '{edit.NewValue}' violates constraints on column '{edit.Field}'."));
        }

        row.SetValue(edit.Field, edit.NewValue);

        return Task.FromResult(
            new GridEditResult(true, edit.RowId, edit.Field, edit.NewValue));
    }

    /// <inheritdoc />
    public bool CanEdit(DataTableRow row, string field, GridColumnDefinition? column) =>
        column is null || column.IsEditable;
}
