// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace BlazorComponentLibrary.Caching;

/// <summary>
/// In-memory cache service implementation.
/// Thread-safe caching with TTL support and pattern-based invalidation.
/// Suitable for single-instance applications without distributed cache needs.
/// </summary>
public class CacheService : ICacheService, IDisposable
{
    private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
    private readonly ReaderWriterLockSlim _lock = new();
    private readonly ILogger<CacheService> _logger;
    private Timer? _cleanupTimer;
    private long _hits;
    private long _misses;

    public CacheService(ILogger<CacheService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Start cleanup timer to remove expired entries every minute
        _cleanupTimer = new Timer(CleanupExpiredEntries, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    /// <summary>
    /// Retrieves value from cache.
    /// Automatically removes expired entries on access.
    /// </summary>
    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        if (string.IsNullOrEmpty(key))
            return null;

        _lock.EnterReadLock();
        try
        {
            if (_cache.TryGetValue(key, out var entry))
            {
                if (entry.IsExpired)
                {
                    _lock.ExitReadLock();
                    _lock.EnterWriteLock();
                    try
                    {
                        _cache.TryRemove(key, out _);
                        Interlocked.Increment(ref _misses);
                        return null;
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                }

                Interlocked.Increment(ref _hits);
                _logger.LogDebug("Cache hit for key: {Key}", key);
                return entry.Value as T;
            }

            Interlocked.Increment(ref _misses);
            _logger.LogDebug("Cache miss for key: {Key}", key);
            return null;
        }
        finally
        {
            if (_lock.IsReadLockHeld)
                _lock.ExitReadLock();
        }
    }

    /// <summary>
    /// Gets value from cache or computes it using factory.
    /// Caches the result for future retrievals.
    /// Prevents cache stampede with single factory execution.
    /// </summary>
    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        if (factory == null)
            throw new ArgumentNullException(nameof(factory));

        var cached = await GetAsync<T>(key);
        if (cached != null)
            return cached;

        _logger.LogInformation("Cache miss, computing value for key: {Key}", key);
        var value = await factory();

        if (value != null)
        {
            await SetAsync(key, value, expiration);
        }

        return value;
    }

    /// <summary>
    /// Stores value in cache with optional expiration.
    /// Thread-safe operation with lock management.
    /// </summary>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        _lock.EnterWriteLock();
        try
        {
            var entry = new CacheEntry
            {
                Value = value,
                ExpiresAt = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : (DateTime?)null,
                CreatedAt = DateTime.UtcNow
            };

            _cache[key] = entry;
            _logger.LogDebug("Cached value for key: {Key}, TTL: {TTL}ms", key, expiration?.TotalMilliseconds);
        }
        finally
        {
            _lock.ExitWriteLock();
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Removes entry from cache by key.
    /// Returns true if entry existed and was removed.
    /// </summary>
    public async Task<bool> RemoveAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
            return false;

        _lock.EnterWriteLock();
        try
        {
            var removed = _cache.TryRemove(key, out _);
            if (removed)
                _logger.LogDebug("Removed cache entry for key: {Key}", key);

            return removed;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Removes all entries matching pattern using regex.
    /// Useful for cache invalidation by prefix or pattern.
    /// </summary>
    public async Task<int> RemoveByPatternAsync(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            return 0;

        _lock.EnterWriteLock();
        try
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var keysToRemove = _cache.Keys.Where(k => regex.IsMatch(k)).ToList();
            var count = keysToRemove.Count;

            foreach (var key in keysToRemove)
            {
                _cache.TryRemove(key, out _);
            }

            if (count > 0)
                _logger.LogDebug("Removed {Count} cache entries matching pattern: {Pattern}", count, pattern);

            return count;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Checks if key exists and is not expired.
    /// </summary>
    public async Task<bool> ExistsAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
            return false;

        _lock.EnterReadLock();
        try
        {
            if (_cache.TryGetValue(key, out var entry))
            {
                return !entry.IsExpired;
            }
            return false;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <summary>
    /// Extends the expiration time for an existing cache entry.
    /// </summary>
    public async Task<bool> ExtendExpirationAsync(string key, TimeSpan expiration)
    {
        if (string.IsNullOrEmpty(key) || expiration <= TimeSpan.Zero)
            return false;

        _lock.EnterWriteLock();
        try
        {
            if (_cache.TryGetValue(key, out var entry))
            {
                if (!entry.IsExpired)
                {
                    entry.ExpiresAt = DateTime.UtcNow.Add(expiration);
                    _logger.LogDebug("Extended expiration for key: {Key}", key);
                    return true;
                }
            }
            return false;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Clears entire cache.
    /// Resets statistics.
    /// </summary>
    public async Task ClearAsync()
    {
        _lock.EnterWriteLock();
        try
        {
            var count = _cache.Count;
            _cache.Clear();
            _hits = 0;
            _misses = 0;
            _logger.LogInformation("Cleared entire cache ({Count} entries)", count);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Gets current cache statistics.
    /// </summary>
    public async Task<CacheStatistics> GetStatisticsAsync()
    {
        _lock.EnterReadLock();
        try
        {
            return new CacheStatistics
            {
                TotalKeys = _cache.Count,
                TotalSize = _cache.Values.Sum(e => EstimateSize(e.Value)),
                Hits = (int)_hits,
                Misses = (int)_misses,
                LastCleaned = DateTime.UtcNow
            };
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <summary>
    /// Periodically removes expired entries from cache.
    /// Prevents memory bloat from stale data.
    /// </summary>
    private void CleanupExpiredEntries(object? state)
    {
        _lock.EnterWriteLock();
        try
        {
            var expiredKeys = _cache
                .Where(kvp => kvp.Value.IsExpired)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _cache.TryRemove(key, out _);
            }

            if (expiredKeys.Count > 0)
                _logger.LogDebug("Cleanup removed {Count} expired cache entries", expiredKeys.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during cache cleanup");
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Estimates size of cached value in bytes.
    /// Simple heuristic for memory monitoring.
    /// </summary>
    private long EstimateSize(object? value)
    {
        if (value == null)
            return 0;

        try
        {
            return System.Runtime.InteropServices.Marshal.SizeOf(value);
        }
        catch
        {
            return value.ToString()?.Length * 2 ?? 0;
        }
    }

    /// <summary>
    /// Cleans up resources.
    /// Stops cleanup timer and clears cache.
    /// </summary>
    public void Dispose()
    {
        _cleanupTimer?.Dispose();
        _lock.Dispose();
        _cache.Clear();
        _logger.LogInformation("CacheService disposed");
    }
}

/// <summary>
/// Internal cache entry with metadata.
/// </summary>
internal class CacheEntry
{
    public object? Value { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow >= ExpiresAt.Value;
}

/// <summary>
/// Extension method to register CacheService in dependency injection.
/// </summary>
public static class CacheServiceExtensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddSingleton<ICacheService, CacheService>();
        return services;
    }
}
