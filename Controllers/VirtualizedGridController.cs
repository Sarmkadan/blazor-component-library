// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BlazorComponentLibrary.Models;
using BlazorComponentLibrary.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BlazorComponentLibrary.Controllers;

/// <summary>
/// REST API controller that exposes the complete virtualized data grid surface over HTTP.
/// <para>
/// Endpoints cover the full feature set:
/// <list type="bullet">
///   <item>Windowed virtual queries with filters, multi-column sort, and full-text search.</item>
///   <item>Single and batch inline cell editing with optimistic-concurrency detection.</item>
///   <item>Server-side edit validation without committing any change.</item>
///   <item>Undo of the most recently committed edit per table.</item>
///   <item>Per-table and per-row edit history.</item>
///   <item>Column-definition retrieval and updates (validation rules, display overrides).</item>
///   <item>Column-level aggregate computation (sum, average, min, max, distinct count).</item>
///   <item>Full filtered-set export to CSV, JSON, or XML.</item>
///   <item>Manual cache invalidation.</item>
/// </list>
/// </para>
/// </summary>
[ApiController]
[Route("api/virtualized-grid")]
[Produces("application/json")]
public sealed class VirtualizedGridController : ControllerBase
{
    private readonly IVirtualizedGridService    _gridService;
    private readonly IGridAggregationService    _aggregationService;
    private readonly IGridExportService         _exportService;
    private readonly ILogger<VirtualizedGridController> _logger;

    /// <summary>
    /// Initialises a new <see cref="VirtualizedGridController"/> with all required grid services.
    /// </summary>
    /// <param name="gridService">Core grid engine for queries, edits, undo, and cache management.</param>
    /// <param name="aggregationService">Service for computing column-level summary aggregates.</param>
    /// <param name="exportService">Service for serialising filtered row sets to downloadable files.</param>
    /// <param name="logger">Structured logger for request telemetry and error reporting.</param>
    public VirtualizedGridController(
        IVirtualizedGridService gridService,
        IGridAggregationService aggregationService,
        IGridExportService exportService,
        ILogger<VirtualizedGridController> logger)
    {
        _gridService        = gridService        ?? throw new ArgumentNullException(nameof(gridService));
        _aggregationService = aggregationService ?? throw new ArgumentNullException(nameof(aggregationService));
        _exportService      = exportService      ?? throw new ArgumentNullException(nameof(exportService));
        _logger             = logger             ?? throw new ArgumentNullException(nameof(logger));
    }

    // ── Query ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Executes a virtual-window query against a table and returns the requested row slice
    /// together with unfiltered and filtered total counts needed by the client to drive
    /// further virtual scrolling.
    /// </summary>
    /// <param name="tableId">Identifier of the source data table.</param>
    /// <param name="request">
    ///   Window parameters: zero-based start index, row count, active column filters,
    ///   sort descriptors, and an optional full-text search term.
    /// </param>
    /// <param name="cancellationToken">Propagated cancellation token.</param>
    [HttpPost("{tableId:int}/query")]
    [ProducesResponseType(typeof(GridVirtualResult<DataTableRow>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Query(
        int tableId,
        [FromBody] GridVirtualRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            _logger.LogInformation(
                "Grid query — table={TableId} start={Start} count={Count} filters={Filters} search='{Search}'.",
                tableId, request.StartIndex, request.Count,
                request.Filters.Count, request.SearchTerm ?? string.Empty);

            var result = await _gridService.QueryAsync(tableId, request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Grid query failed for table {TableId}.", tableId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
        }
    }

    // ── Inline editing ────────────────────────────────────────────────────────

    /// <summary>
    /// Applies a single inline cell edit to a row.
    /// The edit is validated against the column's constraints; on success the previous value
    /// is pushed onto the per-table undo stack and the result cache is invalidated.
    /// </summary>
    /// <param name="tableId">Table that owns the target row.</param>
    /// <param name="edit">
    ///   Edit descriptor: row ID, column field key, proposed new value, and an optional original
    ///   value for optimistic-concurrency verification.
    /// </param>
    /// <param name="cancellationToken">Propagated cancellation token.</param>
    [HttpPost("{tableId:int}/edits")]
    [ProducesResponseType(typeof(GridEditResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GridEditResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ApplyEdit(
        int tableId,
        [FromBody] GridEditRequest edit,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _gridService.ApplyEditAsync(tableId, edit, cancellationToken);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Edit failed — table={TableId} row={RowId} field={Field}.",
                tableId, edit.RowId, edit.Field);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
        }
    }

    /// <summary>
    /// Applies a batch of inline cell edits atomically.
    /// All edits are validated before any write occurs; a single validation failure prevents
    /// every edit in the batch from being committed.  On success, each edit is recorded in
    /// the undo stack and the result cache is invalidated once per distinct row touched.
    /// </summary>
    /// <param name="tableId">Table that owns the target rows.</param>
    /// <param name="edits">Ordered list of edit descriptors to commit.</param>
    /// <param name="cancellationToken">Propagated cancellation token.</param>
    [HttpPost("{tableId:int}/edits/batch")]
    [ProducesResponseType(typeof(IReadOnlyList<GridEditResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IReadOnlyList<GridEditResult>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ApplyBatchEdits(
        int tableId,
        [FromBody] IEnumerable<GridEditRequest> edits,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var results      = await _gridService.ApplyBatchEditsAsync(tableId, edits, cancellationToken);
            var allSucceeded = results.All(r => r.Success);
            return allSucceeded ? Ok(results) : BadRequest(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Batch edit failed for table {TableId}.", tableId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
        }
    }

    /// <summary>
    /// Validates a proposed edit against the column's constraints without committing any change.
    /// Intended for real-time inline-editor feedback while the user is typing.
    /// </summary>
    /// <param name="tableId">Table whose column definitions supply the validation rules.</param>
    /// <param name="edit">Edit descriptor to validate (row ID, field key, and proposed value).</param>
    [HttpPost("{tableId:int}/validate")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidateEdit(int tableId, [FromBody] GridEditRequest edit)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var isValid = await _gridService.ValidateEditAsync(tableId, edit);
        return Ok(isValid);
    }

    // ── Undo ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Reverses the most recently committed edit for a table by writing the captured
    /// previous cell value back to the repository.
    /// </summary>
    /// <param name="tableId">Table whose last committed edit should be undone.</param>
    /// <param name="cancellationToken">Propagated cancellation token.</param>
    [HttpDelete("{tableId:int}/edits/last")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UndoLastEdit(int tableId, CancellationToken cancellationToken)
    {
        try
        {
            var undone = await _gridService.UndoLastEditAsync(tableId);
            return undone
                ? Ok(true)
                : NotFound("No committed edits are available to undo for this table.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Undo failed for table {TableId}.", tableId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
        }
    }

    // ── Edit history ──────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the per-table edit history in reverse-chronological order (newest entry first).
    /// </summary>
    /// <param name="tableId">Target table identifier.</param>
    /// <param name="rowId">
    ///   When provided via query string, restricts the result to edits on that specific row.
    ///   Omit to return the full table history.
    /// </param>
    [HttpGet("{tableId:int}/edits/history")]
    [ProducesResponseType(typeof(IReadOnlyList<GridEditHistoryEntry>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEditHistory(int tableId, [FromQuery] int? rowId)
    {
        var history = await _gridService.GetEditHistoryAsync(tableId, rowId);
        return Ok(history);
    }

    // ── Column definitions ────────────────────────────────────────────────────

    /// <summary>
    /// Returns the effective column definitions for a table, including all grid-specific
    /// overrides registered via <see cref="UpdateColumnDefinition"/>.
    /// </summary>
    /// <param name="tableId">Target table identifier.</param>
    [HttpGet("{tableId:int}/columns")]
    [ProducesResponseType(typeof(IReadOnlyList<GridColumnDefinition>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetColumnDefinitions(int tableId)
    {
        var columns = await _gridService.GetColumnDefinitionsAsync(tableId);
        return Ok(columns);
    }

    /// <summary>
    /// Persists an updated column definition for a table, matched by the column's
    /// <see cref="DataTableColumn.Key"/>.  Updates validation rules, display overrides,
    /// frozen state, and all other grid-specific column properties.
    /// </summary>
    /// <param name="tableId">Table whose column definition should be replaced.</param>
    /// <param name="column">Updated column definition carrying the new constraints and display settings.</param>
    [HttpPut("{tableId:int}/columns")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateColumnDefinition(
        int tableId,
        [FromBody] GridColumnDefinition column)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _gridService.UpdateColumnDefinitionAsync(tableId, column);
        return updated
            ? NoContent()
            : NotFound($"Column '{column.Key}' was not found in table {tableId}.");
    }

    // ── Cache management ──────────────────────────────────────────────────────

    /// <summary>
    /// Purges all cached query windows for a table, forcing the next query to be fully
    /// recomputed from the data repository.  Useful after bulk data imports or external
    /// data changes that bypass the inline editing pipeline.
    /// </summary>
    /// <param name="tableId">Table whose result cache should be invalidated.</param>
    [HttpDelete("{tableId:int}/cache")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> InvalidateCache(int tableId)
    {
        await _gridService.InvalidateCacheAsync(tableId);
        return NoContent();
    }

    // ── Aggregation ───────────────────────────────────────────────────────────

    /// <summary>
    /// Computes column-level aggregate values (count, sum, average, min, max, distinct count)
    /// for the rows that survive the supplied filter and search conditions.
    /// Intended to drive the optional summary footer row rendered below the virtual scroll area.
    /// </summary>
    /// <param name="tableId">Source data table.</param>
    /// <param name="request">
    ///   Aggregation parameters: active filters, optional search term, and the set of column–function
    ///   pairs to evaluate.  The <c>TableId</c> field in the body is overridden by the route parameter.
    /// </param>
    /// <param name="cancellationToken">Propagated cancellation token.</param>
    [HttpPost("{tableId:int}/aggregates")]
    [ProducesResponseType(typeof(GridAggregateResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ComputeAggregates(
        int tableId,
        [FromBody] GridAggregateRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _aggregationService.ComputeAggregatesAsync(
                request with { TableId = tableId }, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Aggregate computation failed for table {TableId}.", tableId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
        }
    }

    // ── Export ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Exports the complete filtered and sorted result set in the requested format.
    /// The entire row set matching the query's filter and search conditions is serialised —
    /// not just the current visible window — and returned as a file download attachment.
    /// </summary>
    /// <param name="tableId">Source data table.</param>
    /// <param name="request">
    ///   Export parameters combining the virtual query (filters, sorts, search) with the desired
    ///   output format key (<c>csv</c>, <c>json</c>, or <c>xml</c>).
    /// </param>
    /// <param name="cancellationToken">Propagated cancellation token.</param>
    [HttpPost("{tableId:int}/export")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Export(
        int tableId,
        [FromBody] GridExportRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!_exportService.SupportedFormats.Contains(
                request.Format, StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(
                $"Unsupported export format '{request.Format}'. " +
                $"Supported formats: {string.Join(", ", _exportService.SupportedFormats)}.");
        }

        try
        {
            var bytes    = await _exportService.ExportAsync(
                               tableId, request.Query, request.Format, cancellationToken);
            var mimeType = GetMimeType(request.Format);
            var fileName = $"export_{tableId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{request.Format.ToLower()}";

            _logger.LogInformation(
                "Export completed — table={TableId} format={Format} bytes={Bytes}.",
                tableId, request.Format, bytes.Length);

            return File(bytes, mimeType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Export failed for table {TableId}.", tableId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static string GetMimeType(string format) => format.ToLowerInvariant() switch
    {
        "csv"  => "text/csv",
        "json" => "application/json",
        "xml"  => "application/xml",
        _      => "application/octet-stream"
    };
}

/// <summary>
/// Request body for the <c>POST api/virtualized-grid/{tableId}/export</c> endpoint.
/// Combines the virtual query parameters that determine which rows are included with the
/// desired serialisation format for the downloaded file.
/// </summary>
/// <param name="Query">
///   Virtual window parameters whose filter, sort, and search conditions are applied to the
///   full table to produce the export data set.
/// </param>
/// <param name="Format">
///   Case-insensitive output format key: <c>csv</c>, <c>json</c>, or <c>xml</c>.
///   Defaults to <c>csv</c>.
/// </param>
public sealed record GridExportRequest(
    [property: Required] GridVirtualRequest Query,
    [property: JsonPropertyName("format")] string Format = "csv"
);
