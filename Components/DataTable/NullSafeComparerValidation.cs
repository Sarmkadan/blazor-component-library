namespace BlazorComponentLibrary.Components.DataTable;

using System;
using System.Collections.Generic;

/// <summary>
/// Provides validation helpers for <see cref="NullSafeComparer{T}"/> instances.
/// </summary>
public static class NullSafeComparerValidation
{
    /// <summary>
    /// Validates that the <see cref="NullSafeComparer{T}"/> instance is properly configured.
    /// </summary>
    /// <param name="value">The comparer instance to validate.</param>
    /// <returns>A list of validation problems; empty list if valid.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static IReadOnlyList<string> Validate(this NullSafeComparer? value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = new List<string>();

        // NullSafeComparer<T> is a sealed internal class with a static Instance property.
        // There's no configuration to validate, but we can verify the comparer behaves correctly.
        // Since we can't instantiate it directly, we validate the static Instance is accessible.
        try
        {
            // Verify the static Instance property exists and is not null
            var instance = NullSafeComparer<object>.Instance;
            if (instance is null)
            {
                problems.Add("NullSafeComparer<T>.Instance returned null");
            }
        }
        catch (Exception ex)
        {
            problems.Add($"NullSafeComparer<T>.Instance threw an exception: {ex.Message}");
        }

        return problems.AsReadOnly();
    }

    /// <summary>
    /// Determines whether the specified <see cref="NullSafeComparer{T}"/> instance is valid.
    /// </summary>
    /// <param name="value">The comparer instance to check.</param>
    /// <returns>True if the instance is valid; otherwise, false.</returns>
    public static bool IsValid(this NullSafeComparer? value) => value?.Validate()?.Count == 0;

    /// <summary>
    /// Ensures that the specified <see cref="NullSafeComparer{T}"/> instance is valid.
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