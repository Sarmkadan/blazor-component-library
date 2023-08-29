// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BenchmarkDotNet.Attributes;
using BlazorComponentLibrary.Caching;

namespace BlazorComponentLibrary.Benchmarks;

/// <summary>
/// Measures cache key generation patterns across the three major call sites:
///
///   - Simple entity keys (component, user) — called on every cache read/write
///   - Compound keys (table pagination) — called during paged data queries
///   - Hashed keys (search) — involve SHA-256 and are the most expensive variant
///   - Fluent builder — used when key structure is determined at runtime
///
/// The SHA-256 path in GenerateSearchKey exercises the SHA256.HashData improvement
/// (framework-pooled instance, stackalloc output buffer) vs the old SHA256.Create().
/// </summary>
[MemoryDiagnoser]
[SimpleJob(launchCount: 1, warmupCount: 3, iterationCount: 10)]
public class CacheKeyBenchmarks
{
    private const string TableId    = "users";
    private const string SearchTerm = "data table widget with pagination";

    [Benchmark(Baseline = true)]
    public string GenerateComponentKey() => CacheKeyGenerator.GenerateComponentKey(42);

    [Benchmark]
    public string GenerateUserKey() => CacheKeyGenerator.GenerateUserKey(1001);

    [Benchmark]
    public string GenerateTableDataKey() => CacheKeyGenerator.GenerateTableDataKey(TableId, page: 3, pageSize: 25);

    [Benchmark]
    public string GenerateThemeListKey() => CacheKeyGenerator.GenerateThemeListKey();

    /// <summary>
    /// Most expensive key type — SHA-256 hash of the search term keeps keys short.
    /// Benchmarking this validates the SHA256.HashData pooling improvement.
    /// </summary>
    [Benchmark]
    public string GenerateSearchKey() => CacheKeyGenerator.GenerateSearchKey("component", SearchTerm);

    [Benchmark]
    public string FluentBuilder()
    {
        return new CacheKeyBuilderExtensions.CacheKeyBuilder()
            .Add(CacheKeyGenerator.Namespaces.Components)
            .Add(42)
            .Add("details")
            .Build();
    }
}
