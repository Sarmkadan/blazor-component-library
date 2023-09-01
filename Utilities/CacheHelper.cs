// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

namespace BlazorComponentLibrary.Utilities;

/// <summary>
/// Helper class for caching operations and cache key generation.
/// Provides utilities for managing cache expiration and key naming conventions.
/// </summary>
public static class CacheHelper
{
    /// <summary>
    /// Generates a cache key for a component by ID.
    /// </summary>
    public static string GetComponentCacheKey(int id)
    {
        return $"component_{id}";
    }

    /// <summary>
    /// Generates a cache key for all components.
    /// </summary>
    public static string GetComponentListCacheKey()
    {
        return "components_list";
    }

    /// <summary>
    /// Generates a cache key for components by type.
    /// </summary>
    public static string GetComponentTypeCacheKey(string componentType)
    {
        if (string.IsNullOrWhiteSpace(componentType))
            throw new ArgumentException("Component type cannot be empty", nameof(componentType));

        return $"components_type_{componentType}";
    }

    /// <summary>
    /// Generates a cache key for a theme by ID.
    /// </summary>
    public static string GetThemeCacheKey(int id)
    {
        return $"theme_{id}";
    }

    /// <summary>
    /// Generates a cache key for the active theme.
    /// </summary>
    public static string GetActiveThemeCacheKey()
    {
        return "theme_active";
    }

    /// <summary>
    /// Generates a cache key for all themes.
    /// </summary>
    public static string GetThemeListCacheKey()
    {
        return "themes_list";
    }

    /// <summary>
    /// Generates a cache key for a user by ID.
    /// </summary>
    public static string GetUserCacheKey(int id)
    {
        return $"user_{id}";
    }

    /// <summary>
    /// Generates a cache key for a user by username.
    /// </summary>
    public static string GetUserByUsernameCacheKey(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        return $"user_username_{username}";
    }

    /// <summary>
    /// Generates a cache key for all users.
    /// </summary>
    public static string GetUserListCacheKey()
    {
        return "users_list";
    }

    /// <summary>
    /// Generates a cache key for form fields.
    /// </summary>
    public static string GetFormFieldCacheKey(int id)
    {
        return $"form_field_{id}";
    }

    /// <summary>
    /// Generates a cache key for all form fields.
    /// </summary>
    public static string GetFormFieldListCacheKey()
    {
        return "form_fields_list";
    }

    /// <summary>
    /// Generates a cache key for table rows by table ID.
    /// </summary>
    public static string GetTableRowsCacheKey(int tableId)
    {
        return $"table_rows_{tableId}";
    }

    /// <summary>
    /// Generates a cache key for a specific table row.
    /// </summary>
    public static string GetTableRowCacheKey(int rowId)
    {
        return $"row_{rowId}";
    }

    /// <summary>
    /// Generates a cache key for chart datasets.
    /// </summary>
    public static string GetChartDatasetCacheKey(int id)
    {
        return $"chart_dataset_{id}";
    }

    /// <summary>
    /// Generates a cache key for all chart datasets.
    /// </summary>
    public static string GetChartDatasetListCacheKey()
    {
        return "chart_datasets_list";
    }

    /// <summary>
    /// Calculates the cache expiration time based on minutes.
    /// </summary>
    public static TimeSpan GetCacheExpiration(int minutes)
    {
        if (minutes < 1)
            throw new ArgumentException("Minutes must be greater than 0", nameof(minutes));

        return TimeSpan.FromMinutes(minutes);
    }

    /// <summary>
    /// Gets the default cache expiration (30 minutes).
    /// </summary>
    public static TimeSpan GetDefaultCacheExpiration()
    {
        return TimeSpan.FromMinutes(30);
    }

    /// <summary>
    /// Validates a cache key format.
    /// </summary>
    public static bool IsValidCacheKey(string key)
    {
        return !string.IsNullOrWhiteSpace(key) &&
               key.Length <= 256 &&
               key.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-');
    }

    /// <summary>
    /// Extracts the ID from a cache key.
    /// Assumes format: prefix_id
    /// </summary>
    public static int? ExtractIdFromKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        var parts = key.Split('_');
        if (parts.Length > 0 && int.TryParse(parts[^1], out var id))
        {
            return id;
        }

        return null;
    }
}

/// <summary>
/// In-memory cache implementation for the library.
/// Thread-safe cache with expiration support.
/// </summary>
public class MemoryCache
{
    private readonly Dictionary<string, CacheEntry> _cache = new();
    private readonly object _lockObject = new();

    private class CacheEntry
    {
        public object? Value { get; set; }
        public DateTime ExpiresAt { get; set; }

        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    }

    /// <summary>
    /// Sets a value in the cache with expiration.
    /// </summary>
    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        if (!CacheHelper.IsValidCacheKey(key))
            throw new ArgumentException("Invalid cache key format", nameof(key));

        lock (_lockObject)
        {
            var entry = new CacheEntry
            {
                Value = value,
                ExpiresAt = DateTime.UtcNow.Add(expiration ?? CacheHelper.GetDefaultCacheExpiration())
            };

            _cache[key] = entry;
        }
    }

    /// <summary>
    /// Gets a value from the cache.
    /// </summary>
    public T? Get<T>(string key)
    {
        if (!CacheHelper.IsValidCacheKey(key))
            throw new ArgumentException("Invalid cache key format", nameof(key));

        lock (_lockObject)
        {
            if (_cache.TryGetValue(key, out var entry))
            {
                if (entry.IsExpired)
                {
                    _cache.Remove(key);
                    return default;
                }

                return (T?)entry.Value;
            }

            return default;
        }
    }

    /// <summary>
    /// Checks if a key exists in the cache and is not expired.
    /// </summary>
    public bool Exists(string key)
    {
        if (!CacheHelper.IsValidCacheKey(key))
            return false;

        lock (_lockObject)
        {
            if (_cache.TryGetValue(key, out var entry))
            {
                if (entry.IsExpired)
                {
                    _cache.Remove(key);
                    return false;
                }

                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Removes a specific key from the cache.
    /// </summary>
    public bool Remove(string key)
    {
        lock (_lockObject)
        {
            return _cache.Remove(key);
        }
    }

    /// <summary>
    /// Removes all expired entries from the cache.
    /// </summary>
    public int RemoveExpired()
    {
        lock (_lockObject)
        {
            var expiredKeys = _cache
                .Where(kvp => kvp.Value.IsExpired)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _cache.Remove(key);
            }

            return expiredKeys.Count;
        }
    }

    /// <summary>
    /// Clears all entries from the cache.
    /// </summary>
    public void Clear()
    {
        lock (_lockObject)
        {
            _cache.Clear();
        }
    }

    /// <summary>
    /// Gets the total number of entries in the cache.
    /// </summary>
    public int Count
    {
        get
        {
            lock (_lockObject)
            {
                return _cache.Count;
            }
        }
    }

    /// <summary>
    /// Gets the number of non-expired entries.
    /// </summary>
    public int CountValid
    {
        get
        {
            lock (_lockObject)
            {
                return _cache.Count(kvp => !kvp.Value.IsExpired);
            }
        }
    }
}
