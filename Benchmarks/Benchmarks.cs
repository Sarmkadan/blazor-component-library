using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BlazorComponentLibrary.Components.DataTable;
using BlazorComponentLibrary.Components.DragDropList;
using System.Collections.Generic;
using System.Linq;

namespace Benchmarks;

/// <summary>
/// Contains benchmarks for various BlazorComponentLibrary operations including sorting algorithms,
/// DataTable operations, and DragDropList reordering functionality.
/// </summary>
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class LibraryBenchmarks
{
    private List<string?> _dataWithNulls = new();
    private List<string> _list = new();
    private List<DataItem> _complexData = new();
    private DataTable<DataItem> _dataTable = new();
    private DragDropList<string> _dragDropList = new();

    /// <summary>
    /// Sets up benchmark data for all benchmark methods.
    /// Initializes test data with null values for sorting benchmarks, creates a large list for drag-and-drop benchmarks,
    /// and populates a complex data collection with 5000 items containing various properties for DataTable benchmarks.
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        // Setup for null-safe sorting benchmarks
        _dataWithNulls = Enumerable.Range(0, 1000)
            .Select(i => i % 10 == 0 ? null : $"Item {i}")
            .ToList();

        // Setup for drag-and-drop reordering benchmarks
        _list = Enumerable.Range(0, 1000)
            .Select(i => $"Item {i}")
            .ToList();

        // Setup for complex data table sorting benchmarks
        _complexData = Enumerable.Range(0, 5000)
            .Select(i => new DataItem
            {
                Id = i,
                Name = $"Name {i}",
                Email = $"user{i}@example.com",
                Status = i % 3 == 0 ? "Active" : i % 3 == 1 ? "Pending" : "Inactive",
                CreatedDate = DateTime.Now.AddDays(-i)
            })
            .ToList();

        _dataTable.SetData(_complexData);
        _dragDropList.Items = _list;
    }

    /// <summary>
    /// Benchmarks the null-safe sorting algorithm using NullSafeComparer.Instance.
    /// Measures the performance of sorting a list containing null values using the library's custom null-safe comparer.
    /// This serves as the baseline comparison for other sorting approaches.
    /// </summary>
    /// <returns>A new list containing the sorted elements, with null values placed at the beginning.</returns>
    [BenchmarkCategory("Sorting")]
    [Benchmark(Baseline = true)]
    public List<string?> SortWithNullSafeComparer()
    {
        return _dataWithNulls
            .OrderBy(x => x, NullSafeComparer.Instance)
            .ToList();
    }

    /// <summary>
    /// Benchmarks a traditional null-safe sorting approach using LINQ's OrderBy with null checks.
    /// Measures the performance of sorting a list containing null values using standard LINQ operations
    /// with explicit null handling via OrderBy(x => x != null).ThenBy(x => x).
    /// </summary>
    /// <returns>A new list containing the sorted elements, with non-null values first followed by null values.</returns>
    [BenchmarkCategory("Sorting")]
    [Benchmark]
    public List<string?> SortWithNullChecks()
    {
        return _dataWithNulls
            .OrderBy(x => x != null)
            .ThenBy(x => x)
            .ToList();
    }

    /// <summary>
    /// Benchmarks sorting complex DataItem objects by their Id property.
    /// Measures the performance of sorting 5000 DataItem objects by their integer Id field using LINQ's OrderBy.
    /// </summary>
    /// <returns>A new list containing DataItem objects sorted by their Id property in ascending order.</returns>
    [BenchmarkCategory("Sorting")]
    [Benchmark]
    public List<DataItem> SortComplexDataById()
    {
        return _complexData
            .OrderBy(x => x.Id)
            .ToList();
    }

    /// <summary>
    /// Benchmarks sorting complex DataItem objects by their Name property.
    /// Measures the performance of sorting 5000 DataItem objects by their string Name field using LINQ's OrderBy.
    /// </summary>
    /// <returns>A new list containing DataItem objects sorted by their Name property in ascending order.</returns>
    [BenchmarkCategory("Sorting")]
    [Benchmark]
    public List<DataItem> SortComplexDataByName()
    {
        return _complexData
            .OrderBy(x => x.Name)
            .ToList();
    }

    /// <summary>
    /// Benchmarks sorting complex DataItem objects by their Status property.
    /// Measures the performance of sorting 5000 DataItem objects by their string Status field using LINQ's OrderBy.
    /// </summary>
    /// <returns>A new list containing DataItem objects sorted by their Status property in ascending order.</returns>
    [BenchmarkCategory("Sorting")]
    [Benchmark]
    public List<DataItem> SortComplexDataByStatus()
    {
        return _complexData
            .OrderBy(x => x.Status)
            .ToList();
    }

    /// <summary>
    /// Benchmarks the DataTable.SetData method for populating the DataTable with complex data.
    /// Measures the performance of setting data on a DataTable component with 5000 DataItem objects.
    /// </summary>
    [BenchmarkCategory("DataTable")]
    [Benchmark]
    public void DataTableSetData()
    {
        _dataTable.SetData(_complexData);
    }

    /// <summary>
    /// Benchmarks sorting a DataTable by the Id property of DataItem objects.
    /// Measures the performance of sorting DataTable contents by the integer Id field using the DataTable.SortBy method.
    /// </summary>
    [BenchmarkCategory("DataTable")]
    [Benchmark]
    public void DataTableSortById()
    {
        _dataTable.SortBy(x => x.Id);
    }

    /// <summary>
    /// Benchmarks sorting a DataTable by the Name property of DataItem objects.
    /// Measures the performance of sorting DataTable contents by the string Name field using the DataTable.SortBy method.
    /// </summary>
    [BenchmarkCategory("DataTable")]
    [Benchmark]
    public void DataTableSortByName()
    {
        _dataTable.SortBy(x => x.Name);
    }

    /// <summary>
    /// Benchmarks sorting a DataTable by the Status property of DataItem objects.
    /// Measures the performance of sorting DataTable contents by the string Status field using the DataTable.SortBy method.
    /// </summary>
    [BenchmarkCategory("DataTable")]
    [Benchmark]
    public void DataTableSortByStatus()
    {
        _dataTable.SortBy(x => x.Status);
    }

    /// <summary>
    /// Benchmarks reordering items in a DragDropList with a small move operation.
    /// Measures the performance of moving an item from position 10 to position 50 in a list of 1000 items.
    /// </summary>
    /// <returns>A new list containing the reordered items.</returns>
    [BenchmarkCategory("DragDropList")]
    [Benchmark]
    public List<string> DragDropListReorderSmall()
    {
        return DragDropList<string>.Reorder(_list, 10, 50);
    }

    /// <summary>
    /// Benchmarks reordering items in a DragDropList with a large move operation.
    /// Measures the performance of moving an item from position 100 to position 500 in a list of 1000 items.
    /// </summary>
    /// <returns>A new list containing the reordered items.</returns>
    [BenchmarkCategory("DragDropList")]
    [Benchmark]
    public List<string> DragDropListReorderLarge()
    {
        return DragDropList<string>.Reorder(_list, 100, 500);
    }

    /// <summary>
    /// Benchmarks reordering items in a DragDropList by moving the first item to the last position.
    /// Measures the performance of moving the first item (position 0) to the last position in a list of 1000 items.
    /// </summary>
    /// <returns>A new list containing the reordered items.</returns>
    [BenchmarkCategory("DragDropList")]
    [Benchmark]
    public List<string> DragDropListReorderFirstToLast()
    {
        return DragDropList<string>.Reorder(_list, 0, _list.Count - 1);
    }

    /// <summary>
    /// Benchmarks reordering items in a DragDropList by moving the last item to the first position.
    /// Measures the performance of moving the last item to the first position in a list of 1000 items.
    /// </summary>
    /// <returns>A new list containing the reordered items.</returns>
    [BenchmarkCategory("DragDropList")]
    [Benchmark]
    public List<string> DragDropListReorderLastToFirst()
    {
        return DragDropList<string>.Reorder(_list, _list.Count - 1, 0);
    }
}

public class DataItem
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedDate { get; set; }
}