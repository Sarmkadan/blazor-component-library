// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

namespace BlazorComponentLibrary.Caching;

/// <summary>
/// Interface for caching operations.
/// Abstracts underlying cache implementation (in-memory, Redis, etc).
/// Supports TTL, tags, and invalidation strategies.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Retrieves value from cache by key.
    /// Returns null if key not found or expired.
    /// </summary>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// Retrieves value from cache or executes factory if not found.
    /// Automatically caches the result.
    /// </summary>
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Stores value in cache with optional expiration.
    /// Overwrites existing value if key already exists.
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Removes value from cache by key.
    /// Returns true if key existed and was removed.
    /// </summary>
    Task<bool> RemoveAsync(string key);

    /// <summary>
    /// Removes all cached values matching a pattern.
    /// Useful for cache invalidation strategies.
    /// </summary>
    Task<int> RemoveByPatternAsync(string pattern);

    /// <summary>
    /// Checks if key exists in cache.
    /// </summary>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// Extends expiration time for existing cache entry.
    /// Useful for extending session timeouts.
    /// </summary>
    Task<bool> ExtendExpirationAsync(string key, TimeSpan expiration);

    /// <summary>
    /// Clears entire cache.
    /// Use with caution - affects all cached data.
    /// </summary>
    Task ClearAsync();

    /// <summary>
    /// Gets cache statistics.
    /// Useful for monitoring and optimization.
    /// </summary>
    Task<CacheStatistics> GetStatisticsAsync();
}

/// <summary>
/// Statistics about cache performance and usage.
/// </summary>
public class CacheStatistics
{
    public int TotalKeys { get; set; }
    public long TotalSize { get; set; }
    public int Hits { get; set; }
    public int Misses { get; set; }
    public double HitRate => (Hits + Misses) == 0 ? 0 : (double)Hits / (Hits + Misses);
    public DateTime? LastCleaned { get; set; }

    public CacheStatistics()
    {
        LastCleaned = DateTime.UtcNow;
    }
}
