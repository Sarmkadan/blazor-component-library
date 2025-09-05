// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Security.Cryptography;
using System.Text;

namespace BlazorComponentLibrary.Caching;

/// <summary>
/// Generates consistent cache keys for different data types.
/// Prevents key collisions and ensures consistent naming.
/// Useful for organizing and invalidating cache by patterns.
/// </summary>
public static class CacheKeyGenerator
{
    private const string Separator = ":";

    /// <summary>
    /// Generates cache key for component data.
    /// Format: component:{id}
    /// </summary>
    public static string GenerateComponentKey(int id)
    {
        return $"component{Separator}{id}";
    }

    /// <summary>
    /// Generates cache key for component list.
    /// Format: component:list:{type}
    /// </summary>
    public static string GenerateComponentListKey(string? type = null)
    {
        return string.IsNullOrEmpty(type)
            ? $"component{Separator}list"
            : $"component{Separator}list{Separator}{type}";
    }

    /// <summary>
    /// Generates cache key for form data.
    /// Format: form:{id}
    /// </summary>
    public static string GenerateFormKey(int formId)
    {
        return $"form{Separator}{formId}";
    }

    /// <summary>
    /// Generates cache key for form submissions.
    /// Format: form:{formId}:submissions:{page}
    /// </summary>
    public static string GenerateFormSubmissionsKey(int formId, int page = 1)
    {
        return $"form{Separator}{formId}{Separator}submissions{Separator}{page}";
    }

    /// <summary>
    /// Generates cache key for table data.
    /// Format: table:{tableId}:{page}:{pageSize}
    /// </summary>
    public static string GenerateTableDataKey(string tableId, int page, int pageSize)
    {
        return $"table{Separator}{tableId}{Separator}{page}{Separator}{pageSize}";
    }

    /// <summary>
    /// Generates cache key for user data.
    /// Format: user:{userId}
    /// </summary>
    public static string GenerateUserKey(int userId)
    {
        return $"user{Separator}{userId}";
    }

    /// <summary>
    /// Generates cache key for theme data.
    /// Format: theme:{themeId}
    /// </summary>
    public static string GenerateThemeKey(int themeId)
    {
        return $"theme{Separator}{themeId}";
    }

    /// <summary>
    /// Generates cache key for theme list.
    /// Format: theme:list
    /// </summary>
    public static string GenerateThemeListKey()
    {
        return $"theme{Separator}list";
    }

    /// <summary>
    /// Generates cache key for search results.
    /// Format: search:{entity}:{term}:{page}
    /// </summary>
    public static string GenerateSearchKey(string entity, string searchTerm, int page = 1)
    {
        var hash = GenerateHash(searchTerm);
        return $"search{Separator}{entity}{Separator}{hash}{Separator}{page}";
    }

    /// <summary>
    /// Generates cache key for API call results.
    /// Format: api:{endpoint}:{hash}
    /// </summary>
    public static string GenerateApiKey(string endpoint, object? queryParams = null)
    {
        var key = $"api{Separator}{endpoint}";

        if (queryParams != null)
        {
            var json = JsonConvert.SerializeObject(queryParams);
            var hash = GenerateHash(json);
            key += $"{Separator}{hash}";
        }

        return key;
    }

    /// <summary>
    /// Generates cache key for session/temporary data.
    /// Format: session:{sessionId}:{key}
    /// </summary>
    public static string GenerateSessionKey(string sessionId, string key)
    {
        return $"session{Separator}{sessionId}{Separator}{key}";
    }

    /// <summary>
    /// Generates cache key for statistics/aggregated data.
    /// Format: stats:{type}:{period}
    /// </summary>
    public static string GenerateStatsKey(string statsType, string period = "daily")
    {
        return $"stats{Separator}{statsType}{Separator}{period}";
    }

    /// <summary>
    /// Generates cache key for configuration data.
    /// Format: config:{section}
    /// </summary>
    public static string GenerateConfigKey(string section)
    {
        return $"config{Separator}{section}";
    }

    /// <summary>
    /// Generates cache key for queued jobs.
    /// Format: job:{jobId}
    /// </summary>
    public static string GenerateJobKey(string jobId)
    {
        return $"job{Separator}{jobId}";
    }

    /// <summary>
    /// Generates cache key for rate limiting.
    /// Format: ratelimit:{clientId}
    /// </summary>
    public static string GenerateRateLimitKey(string clientId)
    {
        return $"ratelimit{Separator}{clientId}";
    }

    /// <summary>
    /// Generates pattern for cache invalidation.
    /// Regex pattern to match related keys.
    /// </summary>
    public static string GeneratePatternForEntity(string entityType)
    {
        // Returns pattern like "component:.*" for regex matching
        return $"^{entityType}{Separator}.*";
    }

    /// <summary>
    /// Generates pattern to invalidate all list caches.
    /// </summary>
    public static string GenerateListInvalidationPattern(string entityType)
    {
        return $"^{entityType}{Separator}list.*";
    }

    /// <summary>
    /// Generates pattern to invalidate all user-specific caches.
    /// </summary>
    public static string GenerateUserInvalidationPattern(int userId)
    {
        return $"^.*{Separator}{userId}{Separator}.*";
    }

    /// <summary>
    /// Generates hash of a string for key components.
    /// Used for long parameters to keep key length manageable.
    /// </summary>
    private static string GenerateHash(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "empty";

        using (var sha = SHA256.Create())
        {
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(hash).ToLower().Substring(0, 8);
        }
    }

    /// <summary>
    /// Generates cache key from a template with parameters.
    /// Template: "component:{id}:details"
    /// </summary>
    public static string GenerateKey(string template, params object?[] parameters)
    {
        return string.Format(template, parameters);
    }

    /// <summary>
    /// Gets common cache key namespaces.
    /// Used for organizing cache structure.
    /// </summary>
    public static class Namespaces
    {
        public const string Components = "component";
        public const string Forms = "form";
        public const string Tables = "table";
        public const string Users = "user";
        public const string Themes = "theme";
        public const string Search = "search";
        public const string Api = "api";
        public const string Session = "session";
        public const string Stats = "stats";
        public const string Config = "config";
        public const string Jobs = "job";
        public const string RateLimit = "ratelimit";
    }
}

/// <summary>
/// Extension methods for cache key generation.
/// Fluent API for building cache keys.
/// </summary>
public static class CacheKeyBuilderExtensions
{
    /// <summary>
    /// Builds cache key fluently.
    /// Example: new CacheKeyBuilder().Add("component").Add(123).Build()
    /// </summary>
    public class CacheKeyBuilder
    {
        private readonly List<string> _parts = new();

        public CacheKeyBuilder Add(object? value)
        {
            if (value != null)
                _parts.Add(value.ToString() ?? string.Empty);

            return this;
        }

        public CacheKeyBuilder AddIf(bool condition, object? value)
        {
            if (condition && value != null)
                _parts.Add(value.ToString() ?? string.Empty);

            return this;
        }

        public string Build()
        {
            return string.Join(CacheKeyGenerator.Separator, _parts);
        }

        public string BuildPattern()
        {
            var key = Build();
            return $"^{key}.*";
        }
    }

    public static CacheKeyBuilder CreateKey(this ICacheService cache)
    {
        return new CacheKeyBuilder();
    }
}
