// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;

namespace BlazorComponentLibrary.Repositories;

/// <summary>
/// In-memory implementation of the data repository.
/// Manages table rows and chart datasets.
/// </summary>
public class DataRepository : IDataRepository
{
    private readonly List<DataTableRow> _rows = new();
    private readonly List<ChartDataset> _datasets = new();
    private int _rowIdCounter = 1;
    private int _datasetIdCounter = 1;
    private readonly object _lock = new();

    public async Task<DataTableRow> AddRowAsync(DataTableRow row)
    {
        if (row == null)
            throw new ArgumentNullException(nameof(row));

        // Fix: Added thread synchronization lock to prevent race conditions on shared mutable lists and ID counters
        lock (_lock)
        {
            row.Id = _rowIdCounter++;
            row.CreatedAt = DateTime.UtcNow;
            _rows.Add(row);
        }

        return await Task.FromResult(row);
    }

    public async Task<DataTableRow?> GetRowByIdAsync(int id)
    {
        lock (_lock)
        {
            return await Task.FromResult(_rows.FirstOrDefault(r => r.Id == id));
        }
    }

    public async Task<IEnumerable<DataTableRow>> GetRowsByTableIdAsync(int tableId)
    {
        lock (_lock)
        {
            return await Task.FromResult(_rows.Where(r => r.TableId == tableId).ToList().AsEnumerable());
        }
    }

    public async Task<DataTableRow> UpdateRowAsync(DataTableRow row)
    {
        if (row == null)
            throw new ArgumentNullException(nameof(row));

        // Fix: Added thread synchronization lock to prevent race conditions on shared mutable lists
        lock (_lock)
        {
            var existing = _rows.FirstOrDefault(r => r.Id == row.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Row with ID {row.Id} not found");

            var index = _rows.IndexOf(existing);
            _rows[index] = row;
        }

        return await Task.FromResult(row);
    }

    public async Task<bool> DeleteRowAsync(int id)
    {
        // Fix: Added thread synchronization lock to prevent race conditions on shared mutable lists
        lock (_lock)
        {
            var row = _rows.FirstOrDefault(r => r.Id == id);
            if (row == null)
                return await Task.FromResult(false);

            _rows.Remove(row);
            return await Task.FromResult(true);
        }
    }

    public async Task<ChartDataset> AddChartDatasetAsync(ChartDataset dataset)
    {
        if (dataset == null)
            throw new ArgumentNullException(nameof(dataset));

        // Fix: Added thread synchronization lock to prevent race conditions on shared mutable lists and ID counters
        lock (_lock)
        {
            dataset.Id = _datasetIdCounter++;
            dataset.CreatedAt = DateTime.UtcNow;
            _datasets.Add(dataset);
        }

        return await Task.FromResult(dataset);
    }

    public async Task<ChartDataset?> GetChartDatasetByIdAsync(int id)
    {
        lock (_lock)
        {
            return await Task.FromResult(_datasets.FirstOrDefault(d => d.Id == id));
        }
    }

    public async Task<IEnumerable<ChartDataset>> GetAllChartDatasetsAsync()
    {
        lock (_lock)
        {
            return await Task.FromResult(_datasets.ToList().AsEnumerable());
        }
    }

    public async Task<ChartDataset> UpdateChartDatasetAsync(ChartDataset dataset)
    {
        if (dataset == null)
            throw new ArgumentNullException(nameof(dataset));

        // Fix: Added thread synchronization lock to prevent race conditions on shared mutable lists
        lock (_lock)
        {
            var existing = _datasets.FirstOrDefault(d => d.Id == dataset.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Dataset with ID {dataset.Id} not found");

            var index = _datasets.IndexOf(existing);
            _datasets[index] = dataset;
        }

        return await Task.FromResult(dataset);
    }

    public async Task<bool> DeleteChartDatasetAsync(int id)
    {
        // Fix: Added thread synchronization lock to prevent race conditions on shared mutable lists
        lock (_lock)
        {
            var dataset = _datasets.FirstOrDefault(d => d.Id == id);
            if (dataset == null)
                return await Task.FromResult(false);

            _datasets.Remove(dataset);
            return await Task.FromResult(true);
        }
    }
}
