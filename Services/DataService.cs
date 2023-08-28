// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;
using BlazorComponentLibrary.Repositories;

namespace BlazorComponentLibrary.Services;

/// <summary>
/// Service for managing table rows and chart datasets.
/// Handles data operations, filtering, sorting, and pagination.
/// </summary>
public class DataService
{
    private readonly IDataRepository _dataRepository;

    public DataService(IDataRepository dataRepository)
    {
        _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
    }

    /// <summary>
    /// Adds a new row to the data table.
    /// </summary>
    public async Task<DataTableRow> AddRowAsync(DataTableRow row)
    {
        if (row == null)
            throw new ArgumentNullException(nameof(row));

        if (row.Data == null || row.Data.Count == 0)
            throw new InvalidOperationException("Row must contain at least one data item");

        return await _dataRepository.AddRowAsync(row);
    }

    /// <summary>
    /// Gets a specific row by ID.
    /// </summary>
    public async Task<DataTableRow?> GetRowByIdAsync(int id)
    {
        if (id <= 0)
            // Fix: Replaced generic ArgumentException with specific ArgumentOutOfRangeException including parameter values
            throw new ArgumentOutOfRangeException(nameof(id), id, "ID must be greater than 0");

        return await _dataRepository.GetRowByIdAsync(id);
    }

    /// <summary>
    /// Gets all rows for a specific table with pagination.
    /// </summary>
    public async Task<PagedResult<DataTableRow>> GetRowsAsync(int tableId, int pageNumber = 1, int pageSize = 25)
    {
        if (tableId <= 0)
            // Fix: Replaced generic ArgumentException with specific ArgumentOutOfRangeException including parameter values
            throw new ArgumentOutOfRangeException(nameof(tableId), tableId, "Table ID must be greater than 0");

        if (pageNumber < 1)
            // Fix: Replaced generic ArgumentException with specific ArgumentOutOfRangeException including parameter values
            throw new ArgumentOutOfRangeException(nameof(pageNumber), pageNumber, "Page number must be 1 or greater");

        if (pageSize < 1 || pageSize > 1000)
            // Fix: Replaced generic ArgumentException with specific ArgumentOutOfRangeException including parameter values
            throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "Page size must be between 1 and 1000");

        var rows = await _dataRepository.GetRowsByTableIdAsync(tableId);
        var totalCount = rows.Count();
        var pagedRows = rows.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PagedResult<DataTableRow>
        {
            Items = pagedRows,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    /// <summary>
    /// Adds a new chart dataset.
    /// </summary>
    public async Task<ChartDataset> AddChartDatasetAsync(ChartDataset dataset)
    {
        if (dataset == null)
            throw new ArgumentNullException(nameof(dataset));

        if (!dataset.IsValid())
            throw new InvalidOperationException("Chart dataset is invalid");

        return await _dataRepository.AddChartDatasetAsync(dataset);
    }

    /// <summary>
    /// Gets a chart dataset by ID.
    /// </summary>
    public async Task<ChartDataset?> GetChartDatasetByIdAsync(int id)
    {
        if (id <= 0)
            // Fix: Replaced generic ArgumentException with specific ArgumentOutOfRangeException including parameter values
            throw new ArgumentOutOfRangeException(nameof(id), id, "ID must be greater than 0");

        return await _dataRepository.GetChartDatasetByIdAsync(id);
    }

    /// <summary>
    /// Updates a chart dataset with new data.
    /// </summary>
    public async Task<ChartDataset> UpdateChartDatasetAsync(int id, ChartDataset dataset)
    {
        if (id <= 0)
            // Fix: Replaced generic ArgumentException with specific ArgumentOutOfRangeException including parameter values
            throw new ArgumentOutOfRangeException(nameof(id), id, "ID must be greater than 0");

        if (dataset == null)
            throw new ArgumentNullException(nameof(dataset));

        var existing = await _dataRepository.GetChartDatasetByIdAsync(id);
        if (existing == null)
            throw new KeyNotFoundException($"Dataset with ID {id} not found");

        dataset.Id = id;
        dataset.CreatedAt = existing.CreatedAt;
        dataset.ModifiedAt = DateTime.UtcNow;

        return await _dataRepository.UpdateChartDatasetAsync(dataset);
    }

    /// <summary>
    /// Filters rows by a column value.
    /// </summary>
    public async Task<IEnumerable<DataTableRow>> FilterRowsAsync(int tableId, string columnKey, object value)
    {
        if (tableId <= 0)
            throw new ArgumentException("Table ID must be greater than 0", nameof(tableId));

        if (string.IsNullOrWhiteSpace(columnKey))
            throw new ArgumentException("Column key cannot be empty", nameof(columnKey));

        var rows = await _dataRepository.GetRowsByTableIdAsync(tableId);
        return rows.Where(r => r.Data.ContainsKey(columnKey) && r.Data[columnKey]?.Equals(value) == true);
    }

    /// <summary>
    /// Sorts rows by a column key.
    /// </summary>
    public async Task<IEnumerable<DataTableRow>> SortRowsAsync(int tableId, string columnKey, bool ascending = true)
    {
        if (tableId <= 0)
            throw new ArgumentException("Table ID must be greater than 0", nameof(tableId));

        if (string.IsNullOrWhiteSpace(columnKey))
            throw new ArgumentException("Column key cannot be empty", nameof(columnKey));

        var rows = await _dataRepository.GetRowsByTableIdAsync(tableId);

        return ascending
            ? rows.OrderBy(r => r.Data.ContainsKey(columnKey) ? r.Data[columnKey] : null)
            : rows.OrderByDescending(r => r.Data.ContainsKey(columnKey) ? r.Data[columnKey] : null);
    }

    /// <summary>
    /// Deletes a row by ID.
    /// </summary>
    public async Task<bool> DeleteRowAsync(int id)
    {
        return await _dataRepository.DeleteRowAsync(id);
    }

    /// <summary>
    /// Bulk updates multiple rows' selection state.
    /// </summary>
    public async Task<int> BulkUpdateSelectionAsync(int tableId, IEnumerable<int> rowIds, bool isSelected)
    {
        if (tableId <= 0)
            throw new ArgumentException("Table ID must be greater than 0", nameof(tableId));

        var rows = await _dataRepository.GetRowsByTableIdAsync(tableId);
        var rowList = rows.ToList();
        int updated = 0;

        foreach (var rowId in rowIds)
        {
            var row = rowList.FirstOrDefault(r => r.Id == rowId);
            if (row != null)
            {
                row.IsSelected = isSelected;
                await _dataRepository.UpdateRowAsync(row);
                updated++;
            }
        }

        return updated;
    }

    /// <summary>
    /// Gets data summary statistics for a table.
    /// </summary>
    public async Task<DataStatistics> GetDataStatisticsAsync(int tableId)
    {
        var rows = await _dataRepository.GetRowsByTableIdAsync(tableId);
        var rowList = rows.ToList();

        return new DataStatistics
        {
            TotalRows = rowList.Count,
            SelectedRows = rowList.Count(r => r.IsSelected),
            ExpandedRows = rowList.Count(r => r.IsExpanded),
            LastUpdated = DateTime.UtcNow
        };
    }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

public class DataStatistics
{
    public int TotalRows { get; set; }
    public int SelectedRows { get; set; }
    public int ExpandedRows { get; set; }
    public DateTime LastUpdated { get; set; }
}
