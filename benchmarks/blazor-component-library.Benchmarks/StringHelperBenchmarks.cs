// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BenchmarkDotNet.Attributes;
using BlazorComponentLibrary.Utilities;

namespace BlazorComponentLibrary.Benchmarks;

/// <summary>
/// Measures the cost of the most frequently called StringHelper operations.
///
/// Scenarios chosen to represent real workloads:
///   - ToKebabCase / ToSnakeCase: called on every component name during render
///   - ToPascalCase: used when mapping configuration keys back to property names
///   - ToUrlSlug: called on user-supplied titles before persisting to storage
///   - Sanitize: called on every user-controlled string entering the render pipeline
///   - Reverse: utility operation included for completeness
/// </summary>
[MemoryDiagnoser]
[SimpleJob(launchCount: 1, warmupCount: 3, iterationCount: 10)]
public class StringHelperBenchmarks
{
    private const string PascalInput    = "MyComponentDataTableRowRenderer";
    private const string KebabInput     = "my-component-data-table-row-renderer";
    private const string SlugInput      = "Hello World! This is a Test -- with <Special> Chars & More Content Here";
    private const string SanitizeClean  = "Hello World — this text has no dangerous characters at all";
    private const string SanitizeDirty  = "Hello <script>alert('xss')</script> & \"world\" > evil";

    [Benchmark(Baseline = true)]
    public string ToKebabCase() => StringHelper.ToKebabCase(PascalInput);

    [Benchmark]
    public string ToSnakeCase() => StringHelper.ToSnakeCase(PascalInput);

    [Benchmark]
    public string ToPascalCase() => StringHelper.ToPascalCase(KebabInput);

    [Benchmark]
    public string ToUrlSlug() => StringHelper.ToUrlSlug(SlugInput);

    /// <summary>Fast path: no dangerous chars — returns the original string without any allocation.</summary>
    [Benchmark]
    public string Sanitize_Clean() => StringHelper.Sanitize(SanitizeClean);

    /// <summary>Slow path: dangerous chars present — builds a new sanitised string.</summary>
    [Benchmark]
    public string Sanitize_Dirty() => StringHelper.Sanitize(SanitizeDirty);

    [Benchmark]
    public string Reverse() => StringHelper.Reverse(PascalInput);
}
