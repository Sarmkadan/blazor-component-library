namespace BlazorComponentLibrary.Components.DataTable;

using System;
using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// Provides validation helpers for <see cref="NullSafeComparer"/> instances.
/// </summary>
public static class NullSafeComparerValidation
{
    /// <summary>
    /// Validates that the <see cref="NullSafeComparer"/> instance is properly configured.
    /// </summary>
    /// <param name="value">The comparer instance to validate.</param>
    /// <returns>A list of validation problems; empty list if valid.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static IReadOnlyList<string> Validate(this NullSafeComparer? value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = new List<string>();

        return problems.AsReadOnly();
    }

    /// <summary>
    /// Determines whether the specified <see cref="NullSafeComparer"/> instance is valid.
    /// </summary>
    /// <param name="value">The comparer instance to check.</param>
    /// <returns>True if the instance is valid; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static bool IsValid(this NullSafeComparer? value)
    {
        return value?.Validate() is null || value.Validate().Count == 0;
    }

    /// <summary>
    /// Ensures that the specified <see cref="NullSafeComparer"/> instance is valid.
    /// </summary>
    /// <param name="value">The comparer instance to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the comparer instance has validation problems.</exception>
    public static void EnsureValid(this NullSafeComparer? value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = value.Validate();
        if (problems.Count > 0)
        {
            throw new ArgumentException($"NullSafeComparer is not valid. Problems: {string.Join("; ", problems)}");
        }
    }
}