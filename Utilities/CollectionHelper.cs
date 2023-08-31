// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Runtime.CompilerServices;

namespace BlazorComponentLibrary.Utilities;

/// <summary>
/// Utility class for collection operations.
/// Provides extension methods and helper functions for lists, arrays, and enumerables.
/// </summary>
public static class CollectionHelper
{
    /// <summary>
    /// Checks if collection is null or empty.
    /// Null-safe operation for defensive programming.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? collection)
    {
        return collection?.Any() != true;
    }

    /// <summary>
    /// Checks if collection has any items.
    /// Opposite of IsNullOrEmpty, useful for clarity in conditions.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasItems<T>(this IEnumerable<T>? collection)
    {
        return collection?.Any() == true;
    }

    /// <summary>
    /// Safely returns first item or default value.
    /// Avoids InvalidOperationException.
    /// </summary>
    public static T? SafeFirst<T>(this IEnumerable<T>? collection)
    {
        return collection?.FirstOrDefault();
    }

    /// <summary>
    /// Safely returns first item matching predicate or default.
    /// </summary>
    public static T? SafeFirst<T>(this IEnumerable<T>? collection, Func<T, bool> predicate)
    {
        return collection?.FirstOrDefault(predicate);
    }

    /// <summary>
    /// Safely returns last item or default value.
    /// </summary>
    public static T? SafeLast<T>(this IEnumerable<T>? collection)
    {
        return collection?.LastOrDefault();
    }

    /// <summary>
    /// Groups and counts items by key.
    /// Useful for statistics and aggregations.
    /// </summary>
    public static Dictionary<TKey, int> CountByKey<T, TKey>(this IEnumerable<T>? collection, Func<T, TKey> keySelector) where TKey : notnull
    {
        return collection?
            .GroupBy(keySelector)
            .ToDictionary(g => g.Key, g => g.Count()) ?? new Dictionary<TKey, int>();
    }

    /// <summary>
    /// Batches collection into chunks of specified size.
    /// Useful for pagination and batch processing.
    /// </summary>
    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T>? collection, int batchSize)
    {
        if (batchSize <= 0)
            throw new ArgumentException("Batch size must be greater than 0", nameof(batchSize));

        if (collection == null)
            yield break;

        var batch = new List<T>(batchSize);

        foreach (var item in collection)
        {
            batch.Add(item);
            if (batch.Count >= batchSize)
            {
                yield return batch;
                batch = new List<T>(batchSize);
            }
        }

        if (batch.Count > 0)
            yield return batch;
    }

    /// <summary>
    /// Filters collection by type, returning only items matching target type.
    /// </summary>
    public static IEnumerable<TTarget> OfTypeAs<TTarget>(this IEnumerable? collection) where TTarget : class
    {
        return collection?.OfType<TTarget>() ?? Enumerable.Empty<TTarget>();
    }

    /// <summary>
    /// Flattens nested collections into single sequence.
    /// </summary>
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>>? collection)
    {
        return collection?.SelectMany(x => x) ?? Enumerable.Empty<T>();
    }

    /// <summary>
    /// Finds duplicate items in collection.
    /// Returns items that appear more than once.
    /// </summary>
    public static IEnumerable<T> FindDuplicates<T>(this IEnumerable<T>? collection) where T : notnull
    {
        return collection?
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key) ?? Enumerable.Empty<T>();
    }

    /// <summary>
    /// Performs action for each item in collection.
    /// Alternative to foreach loop for functional style.
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T>? collection, Action<T> action)
    {
        if (collection == null)
            return;

        foreach (var item in collection)
        {
            action(item);
        }
    }

    /// <summary>
    /// Performs action for each item with index.
    /// </summary>
    public static void ForEachWithIndex<T>(this IEnumerable<T>? collection, Action<T, int> action)
    {
        if (collection == null)
            return;

        var index = 0;
        foreach (var item in collection)
        {
            action(item, index);
            index++;
        }
    }

    /// <summary>
    /// Shuffles collection randomly.
    /// Modifies order of items for randomization.
    /// </summary>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T>? collection)
    {
        if (collection == null)
            return Enumerable.Empty<T>();

        var list = collection.ToList();
        var rng = new Random();
        var n = list.Count;

        while (n > 1)
        {
            n--;
            var k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }

        return list;
    }

    /// <summary>
    /// Joins collection items into string with separator.
    /// Shorthand for string.Join.
    /// </summary>
    public static string Join<T>(this IEnumerable<T>? collection, string separator = ", ")
    {
        return string.Join(separator, collection ?? Enumerable.Empty<T>());
    }

    /// <summary>
    /// Gets distinct items from collection, ignoring case for strings.
    /// </summary>
    public static IEnumerable<string> DistinctIgnoreCase(this IEnumerable<string>? collection)
    {
        return collection?
            .Distinct(StringComparer.OrdinalIgnoreCase) ?? Enumerable.Empty<string>();
    }

    /// <summary>
    /// Chunks collection by comparing consecutive items.
    /// Groups consecutive equal items together.
    /// </summary>
    public static IEnumerable<IEnumerable<T>> ChunkByConsecutive<T>(this IEnumerable<T>? collection) where T : IEquatable<T>
    {
        if (collection == null)
            yield break;

        var chunk = new List<T>();
        T? previous = default;
        var hasValue = false;

        foreach (var item in collection)
        {
            if (hasValue && !item.Equals(previous))
            {
                yield return chunk;
                chunk = new List<T>();
            }

            chunk.Add(item);
            previous = item;
            hasValue = true;
        }

        if (chunk.Count > 0)
            yield return chunk;
    }

    /// <summary>
    /// Gets the sum of values extracted by selector.
    /// Null-safe decimal summation.
    /// </summary>
    public static decimal SafeSum<T>(this IEnumerable<T>? collection, Func<T, decimal> selector)
    {
        // Fix: Added missing input validation for selector to prevent ArgumentNullException inside LINQ methods
        if (selector == null)
            throw new ArgumentNullException(nameof(selector), "Selector cannot be null");

        return collection?.Sum(selector) ?? 0m;
    }

    /// <summary>
    /// Gets the average of values extracted by selector.
    /// Null-safe decimal averaging.
    /// </summary>
    public static decimal SafeAverage<T>(this IEnumerable<T>? collection, Func<T, decimal> selector)
    {
        // Fix: Added missing input validation for selector to prevent ArgumentNullException inside LINQ methods
        if (selector == null)
            throw new ArgumentNullException(nameof(selector), "Selector cannot be null");

        return collection?.Any() == true ? collection.Average(selector) : 0m;
    }
}
