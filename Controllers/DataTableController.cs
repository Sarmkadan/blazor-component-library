// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;
using BlazorComponentLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorComponentLibrary.Controllers;

/// <summary>
/// REST API controller for data table operations.
/// Handles CRUD, filtering, sorting, and pagination of tabular data.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DataTableController : ControllerBase
{
    private readonly DataService _dataService;
    private readonly ILogger<DataTableController> _logger;

    public DataTableController(DataService dataService, ILogger<DataTableController> logger)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves paginated table data with optional filtering and sorting.
    /// Supports column-level filtering and multi-column sorting.
    /// </summary>
    [HttpPost("query")]
    [ProducesResponseType(typeof(DataTableResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> QueryData([FromBody] DataTableQueryRequest request)
    {
        try
        {
            if (request == null)
                return BadRequest("Query request cannot be null");

            _logger.LogInformation("Querying table data: Page {Page}, PageSize {PageSize}",
                request.Page, request.PageSize);

            var response = await _dataService.QueryDataAsync(request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid query parameters");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying data");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Exports table data in specified format (CSV, JSON, XML).
    /// Respects current filters and sorting applied to table.
    /// </summary>
    [HttpPost("export")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportData([FromBody] DataTableExportRequest request)
    {
        try
        {
            if (request == null)
                return BadRequest("Export request cannot be null");

            if (!Enum.TryParse<ExportFormat>(request.Format, true, out var format))
                return BadRequest("Invalid export format. Supported: CSV, JSON, XML");

            _logger.LogInformation("Exporting data in format: {Format}", format);

            var fileContent = await _dataService.ExportDataAsync(request, format);
            var fileName = $"export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{format.ToString().ToLower()}";

            return File(fileContent, GetMimeType(format), fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting data");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Inserts a new data row with validation.
    /// Returns the created row with generated ID.
    /// </summary>
    [HttpPost("rows")]
    [ProducesResponseType(typeof(DataTableRow), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> InsertRow([FromBody] DataTableRow row)
    {
        try
        {
            if (row == null)
                return BadRequest("Row data cannot be null");

            _logger.LogInformation("Inserting new data row");
            var created = await _dataService.InsertRowAsync(row);
            return CreatedAtAction(nameof(GetRow), new { id = created.Id }, created);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Row validation failed");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inserting row");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Retrieves a specific row by ID.
    /// </summary>
    [HttpGet("rows/{id}")]
    [ProducesResponseType(typeof(DataTableRow), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRow(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Row ID must be greater than 0");

            _logger.LogInformation("Fetching row with ID: {RowId}", id);
            var row = await _dataService.GetRowAsync(id);

            if (row == null)
                return NotFound($"Row with ID {id} not found");

            return Ok(row);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching row {RowId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Updates an existing row.
    /// Validates data integrity before persistence.
    /// </summary>
    [HttpPut("rows/{id}")]
    [ProducesResponseType(typeof(DataTableRow), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRow(int id, [FromBody] DataTableRow row)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Row ID must be greater than 0");

            if (row == null)
                return BadRequest("Row data cannot be null");

            _logger.LogInformation("Updating row: {RowId}", id);
            var updated = await _dataService.UpdateRowAsync(id, row);

            if (updated == null)
                return NotFound($"Row with ID {id} not found");

            return Ok(updated);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Row validation failed during update");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating row {RowId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Deletes a row by ID.
    /// Performs soft delete to maintain audit trail.
    /// </summary>
    [HttpDelete("rows/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRow(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("Row ID must be greater than 0");

            _logger.LogInformation("Deleting row: {RowId}", id);
            var deleted = await _dataService.DeleteRowAsync(id);

            if (!deleted)
                return NotFound($"Row with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting row {RowId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Bulk deletes multiple rows by IDs.
    /// Useful for batch operations from UI selections.
    /// </summary>
    [HttpPost("rows/bulk-delete")]
    [ProducesResponseType(typeof(BulkDeleteResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> BulkDeleteRows([FromBody] int[] rowIds)
    {
        try
        {
            if (rowIds == null || rowIds.Length == 0)
                return BadRequest("Row IDs list cannot be empty");

            _logger.LogInformation("Bulk deleting {Count} rows", rowIds.Length);
            var result = await _dataService.BulkDeleteRowsAsync(rowIds);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk deleting rows");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Validates row data without persisting.
    /// Useful for client-side validation feedback.
    /// </summary>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status200OK)]
    public IActionResult ValidateRow([FromBody] DataTableRow row)
    {
        try
        {
            if (row == null)
                return BadRequest("Row data cannot be null");

            _logger.LogInformation("Validating row data");
            var validation = _dataService.ValidateRow(row);
            return Ok(validation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating row");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    private string GetMimeType(ExportFormat format) => format switch
    {
        ExportFormat.CSV => "text/csv",
        ExportFormat.JSON => "application/json",
        ExportFormat.XML => "application/xml",
        _ => "application/octet-stream"
    };
}

public enum ExportFormat { CSV, JSON, XML }

public class DataTableQueryRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string SortDirection { get; set; } = "asc";
    public Dictionary<string, string>? Filters { get; set; }
}

public class DataTableExportRequest
{
    public string Format { get; set; } = "CSV";
    public Dictionary<string, string>? Filters { get; set; }
}

public class DataTableResponse
{
    public IEnumerable<DataTableRow> Rows { get; set; } = new List<DataTableRow>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class BulkDeleteResponse
{
    public int DeletedCount { get; set; }
    public List<int> DeletedIds { get; set; } = new();
    public List<int> FailedIds { get; set; } = new();
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public Dictionary<string, string> Errors { get; set; } = new();
}
