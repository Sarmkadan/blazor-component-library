using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BlazorComponentLibrary.Components.DataTable;
using BlazorComponentLibrary.Components.DragDropList;
using System.Collections.Generic;
using System.Linq;

namespace Benchmarks;

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

    [BenchmarkCategory("Sorting")]
    [Benchmark(Baseline = true)]
    public List<string?> SortWithNullSafeComparer()
    {
        return _dataWithNulls
            .OrderBy(x => x, NullSafeComparer.Instance)
            .ToList();
    }

    [BenchmarkCategory("Sorting")]
    [Benchmark]
    public List<string?> SortWithNullChecks()
    {
        return _dataWithNulls
            .OrderBy(x => x != null)
            .ThenBy(x => x)
            .ToList();
    }

    [BenchmarkCategory("Sorting")]
    [Benchmark]
    public List<DataItem> SortComplexDataById()
    {
        return _complexData
            .OrderBy(x => x.Id)
            .ToList();
    }

    [BenchmarkCategory("Sorting")]
    [Benchmark]
    public List<DataItem> SortComplexDataByName()
    {
        return _complexData
            .OrderBy(x => x.Name)
            .ToList();
    }

    [BenchmarkCategory("Sorting")]
    [Benchmark]
    public List<DataItem> SortComplexDataByStatus()
    {
        return _complexData
            .OrderBy(x => x.Status)
            .ToList();
    }

    [BenchmarkCategory("DataTable")]
    [Benchmark]
    public void DataTableSetData()
    {
        _dataTable.SetData(_complexData);
    }

    [BenchmarkCategory("DataTable")]
    [Benchmark]
    public void DataTableSortById()
    {
        _dataTable.SortBy(x => x.Id);
    }

    [BenchmarkCategory("DataTable")]
    [Benchmark]
    public void DataTableSortByName()
    {
        _dataTable.SortBy(x => x.Name);
    }

    [BenchmarkCategory("DataTable")]
    [Benchmark]
    public void DataTableSortByStatus()
    {
        _dataTable.SortBy(x => x.Status);
    }

    [BenchmarkCategory("DragDropList")]
    [Benchmark]
    public List<string> DragDropListReorderSmall()
    {
        return DragDropList<string>.Reorder(_list, 10, 50);
    }

    [BenchmarkCategory("DragDropList")]
    [Benchmark]
    public List<string> DragDropListReorderLarge()
    {
        return DragDropList<string>.Reorder(_list, 100, 500);
    }

    [BenchmarkCategory("DragDropList")]
    [Benchmark]
    public List<string> DragDropListReorderFirstToLast()
    {
        return DragDropList<string>.Reorder(_list, 0, _list.Count - 1);
    }

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