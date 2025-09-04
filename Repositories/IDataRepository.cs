// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;

namespace BlazorComponentLibrary.Repositories;

/// <summary>
/// Repository interface for table rows and chart datasets.
/// </summary>
public interface IDataRepository
{
    Task<DataTableRow> AddRowAsync(DataTableRow row);
    Task<DataTableRow?> GetRowByIdAsync(int id);
    Task<IEnumerable<DataTableRow>> GetRowsByTableIdAsync(int tableId);
    Task<DataTableRow> UpdateRowAsync(DataTableRow row);
    Task<bool> DeleteRowAsync(int id);

    Task<ChartDataset> AddChartDatasetAsync(ChartDataset dataset);
    Task<ChartDataset?> GetChartDatasetByIdAsync(int id);
    Task<IEnumerable<ChartDataset>> GetAllChartDatasetsAsync();
    Task<ChartDataset> UpdateChartDatasetAsync(ChartDataset dataset);
    Task<bool> DeleteChartDatasetAsync(int id);
}
