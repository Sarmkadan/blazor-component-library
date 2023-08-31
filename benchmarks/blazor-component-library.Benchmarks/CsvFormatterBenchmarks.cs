// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BenchmarkDotNet.Attributes;
using BlazorComponentLibrary.Formatters;

namespace BlazorComponentLibrary.Benchmarks;

/// <summary>
/// Measures CSV serialisation and deserialisation at two realistic dataset sizes.
///
/// Scenarios:
///   - ToCsv_100Rows  — typical paginated export (one screen of data)
///   - ToCsv_1000Rows — batch export covering one full table
///   - FromCsv_100Rows — import / round-trip validation for a single page
///
/// SampleRow is a flat POCO that exercises all code paths in CsvFormatter:
/// numeric, text with embedded quotes, and standard identifier fields.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(launchCount: 1, warmupCount: 3, iterationCount: 10)]
public class CsvFormatterBenchmarks
{
    private List<SampleRow> _rows100  = null!;
    private List<SampleRow> _rows1000 = null!;
    private string _csv100 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _rows100 = Enumerable.Range(1, 100).Select(BuildRow).ToList();
        _rows1000 = Enumerable.Range(1, 1000).Select(BuildRow).ToList();
        _csv100   = CsvFormatter.ToCsv(_rows100);
    }

    [Benchmark(Baseline = true)]
    public string ToCsv_100Rows() => CsvFormatter.ToCsv(_rows100);

    [Benchmark]
    public string ToCsv_1000Rows() => CsvFormatter.ToCsv(_rows1000);

    [Benchmark]
    public List<SampleRow> FromCsv_100Rows() => CsvFormatter.FromCsv<SampleRow>(_csv100);

    // -------------------------------------------------------------------------

    private static SampleRow BuildRow(int i) => new()
    {
        Id       = i,
        Name     = $"Product {i}",
        // Intentionally includes a comma and a quote to exercise EscapeField's slow path.
        Notes    = i % 10 == 0 ? $"Special, item with \"quotes\" #{i}" : $"Standard item #{i}",
        Email    = $"user{i}@example.com",
        Value    = Math.Round(i * 1.5m, 2),
        IsActive = i % 2 == 0,
    };

    public class SampleRow
    {
        public int     Id       { get; set; }
        public string  Name     { get; set; } = "";
        public string  Notes    { get; set; } = "";
        public string  Email    { get; set; } = "";
        public decimal Value    { get; set; }
        public bool    IsActive { get; set; }
    }
}
