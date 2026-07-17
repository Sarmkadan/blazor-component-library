namespace BlazorComponentLibrary.Components.DataTable;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A generic null-safe comparer that handles null values by treating them as less than any non-null value.
/// </summary>
/// <typeparam name="T">The type to compare.</typeparam>
internal sealed class NullSafeComparer<T> : IComparer<T?>
{
    public static readonly NullSafeComparer<T> Instance = new();

    public NullSafeComparer() { }

    public int Compare(T? x, T? y)
    {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        return Comparer<T>.Default.Compare(x, y);
    }
}

/// <summary>
/// Provides extension methods for null-safe comparison that enable advanced sorting, filtering,
/// and collection operations with null-safe comparison semantics.
/// </summary>
public static class NullSafeComparerExtensions
{
    /// <summary>
    /// Sorts the elements of a sequence in ascending order by the specified key using null-safe comparison.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>An <see cref="IOrderedEnumerable<TSource>"/> whose elements are sorted according to a key.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="keySelector"/> is null.</exception>
    public static IOrderedEnumerable<TSource> OrderByNullSafe<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        return System.Linq.Enumerable.OrderBy(source, keySelector, NullSafeComparer<TKey>.Instance);
    }

    /// <summary>
    /// Sorts the elements of a sequence in descending order by the specified key using null-safe comparison.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>An <see cref="IOrderedEnumerable<TSource>"/> whose elements are sorted in descending order according to a key.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="keySelector"/> is null.</exception>
    public static IOrderedEnumerable<TSource> OrderByDescendingNullSafe<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        return System.Linq.Enumerable.OrderByDescending(source, keySelector, NullSafeComparer<TKey>.Instance);
    }

    /// <summary>
    /// Returns the minimum value in a sequence using null-safe comparison.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values to determine the minimum of.</param>
    /// <returns>The minimum value in the sequence, or default if the sequence is empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the sequence contains only null values and no default value exists.</exception>
    public static TSource? Min<TSource>(this IEnumerable<TSource> source) where TSource : IComparable<TSource>
    {
        ArgumentNullException.ThrowIfNull(source);

        return source
            .Where(x => x is not null)
            .OrderByNullSafe(x => x)
            .FirstOrDefault();
    }

    /// <summary>
    /// Returns the maximum value in a sequence using null-safe comparison.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values to determine the maximum of.</param>
    /// <returns>The maximum value in the sequence, or default if the sequence is empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the sequence contains only null values and no default value exists.</exception>
    public static TSource? Max<TSource>(this IEnumerable<TSource> source) where TSource : IComparable<TSource>
    {
        ArgumentNullException.ThrowIfNull(source);

        return source
            .Where(x => x is not null)
            .OrderByDescendingNullSafe(x => x)
            .FirstOrDefault();
    }

    /// <summary>
    /// Returns a new sequence sorted by the specified key selector in the specified direction.
    /// This is a convenience method that wraps the LINQ OrderBy method with null-safe comparison.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <param name="direction">The sort direction (ascending or descending).</param>
    /// <returns>A new sequence sorted according to the specified direction.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="keySelector"/> is null.</exception>
    public static IEnumerable<TSource> SortBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        SortDirection direction = SortDirection.Ascending)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        return direction == SortDirection.Ascending
            ? source.OrderByNullSafe(keySelector)
            : source.OrderByDescendingNullSafe(keySelector);
    }

    /// <summary>
    /// Returns a sequence with null values filtered out, using null-safe comparison semantics.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values to filter.</param>
    /// <returns>A new sequence containing only non-null elements.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    public static IEnumerable<TSource> WhereNotNull<TSource>(this IEnumerable<TSource> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return source.Where(item => item is not null);
    }

    /// <summary>
    /// Returns a sequence with null values filtered out, using null-safe comparison semantics.
    /// This overload works with nullable value types.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of nullable values to filter.</param>
    /// <returns>A new sequence containing only non-null values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    public static IEnumerable<TSource> WhereNotNull<TSource>(this IEnumerable<TSource?> source) where TSource : struct
    {
        ArgumentNullException.ThrowIfNull(source);

        return source.Where(item => item.HasValue).Select(item => item!.Value);
    }
}
