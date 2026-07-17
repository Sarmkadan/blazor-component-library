namespace BlazorComponentLibrary.Components.DragDropList;

using System;
using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// Provides validation helpers for <see cref="DragDropList{TItem}"/> components.
/// </summary>
public static class DragDropListValidation
{
    /// <summary>
    /// Validates a <see cref="DragDropList{TItem}"/> instance and returns any problems found.
    /// </summary>
    /// <typeparam name="TItem">The type of each list item.</typeparam>
    /// <param name="value">The <see cref="DragDropList{TItem}"/> instance to validate.</param>
    /// <returns>An <see cref="IReadOnlyList{string}"/> of human-readable problems; empty if valid.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static IReadOnlyList<string> Validate<TItem>(this DragDropList<TItem> value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = new List<string>();

        // Validate Items
        if (value.Items is null)
        {
            problems.Add("Items cannot be null.");
        }
        else if (value.Items.Count == 0)
        {
            problems.Add("Items collection cannot be empty.");
        }

        // Validate ItemTemplate
        if (value.ItemTemplate is null)
        {
            problems.Add("ItemTemplate cannot be null.");
        }

        // Validate OnOrderChanged
        // Note: EventCallback can be null/empty delegate, which is valid

        // Validate Enabled (no validation needed - can be true or false)

        // Validate CssClass (can be null or empty string, both are valid)

        return problems.AsReadOnly();
    }

    /// <summary>
    /// Determines whether a <see cref="DragDropList{TItem}"/> instance is valid.
    /// </summary>
    /// <typeparam name="TItem">The type of each list item.</typeparam>
    /// <param name="value">The <see cref="DragDropList{TItem}"/> instance to check.</param>
    /// <returns><see langword="true"/> if valid; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static bool IsValid<TItem>(this DragDropList<TItem> value) => Validate(value).Count == 0;

    /// <summary>
    /// Ensures that a <see cref="DragDropList{TItem}"/> instance is valid, throwing an exception if not.
    /// </summary>
    /// <typeparam name="TItem">The type of each list item.</typeparam>
    /// <param name="value">The <see cref="DragDropList{TItem}"/> instance to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is invalid, containing a list of problems.</exception>
    public static void EnsureValid<TItem>(this DragDropList<TItem> value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = Validate(value);

        if (problems.Count > 0)
        {
            throw new ArgumentException(
                $"DragDropList is invalid. Problems: {string.Join(" ", problems)}",
                nameof(value));
        }
    }
}