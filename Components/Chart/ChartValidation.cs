namespace BlazorComponentLibrary.Components.Chart;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Provides validation helpers for chart components.
/// </summary>
public static class ChartValidation
{
    /// <summary>
    /// Validates a chart instance and returns a list of human-readable problems.
    /// </summary>
    /// <param name="value">The chart instance to validate.</param>
    /// <returns>An enumerable of validation problems; empty if the chart is valid.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public static IReadOnlyList<string> Validate(this IChart<object> value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = new List<string>();

        // Validate ChartType
        if (value.ChartType == default)
        {
            problems.Add("ChartType must be set to a valid value.");
        }

        // Validate Title
        if (string.IsNullOrWhiteSpace(value.Title))
        {
            problems.Add("Title must be a non-empty string.");
        }

        // Validate Labels
        if (value.Labels is null)
        {
            problems.Add("Labels collection cannot be null.");
        }
        else if (!value.Labels.Any())
        {
            problems.Add("Labels collection cannot be empty.");
        }
        else
        {
            var labelProblems = ValidateStringCollection(value.Labels, "Label");
            problems.AddRange(labelProblems);
        }

        // Validate Colors
        if (value.Colors is not null)
        {
            var colorProblems = ValidateStringCollection(value.Colors, "Color");
            problems.AddRange(colorProblems);
        }

        // Validate Annotations
        if (value.Annotations is not null)
        {
            var annotationProblems = ValidateAnnotations(value.Annotations);
            problems.AddRange(annotationProblems);
        }

        return problems.AsReadOnly();
    }

    /// <summary>
    /// Determines whether a chart instance is valid.
    /// </summary>
    /// <param name="value">The chart instance to check.</param>
    /// <returns><see langword="true"/> if the chart is valid; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public static bool IsValid(this IChart<object> value) => Validate(value).Count == 0;

    /// <summary>
    /// Ensures that a chart instance is valid, throwing an exception if it is not.
    /// </summary>
    /// <param name="value">The chart instance to validate.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="value"/> has validation problems.</exception>
    public static void EnsureValid(this IChart<object> value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = Validate(value);

        if (problems.Count > 0)
        {
            throw new ArgumentException(
                $"Chart validation failed:{Environment.NewLine}{string.Join(Environment.NewLine, problems)}");
        }
    }

    /// <summary>
    /// Validates that a collection of strings contains no null or whitespace entries.
    /// </summary>
    /// <param name="collection">The collection to validate.</param>
    /// <param name="itemTypeName">The type of items in the collection (used in error messages).</param>
    /// <returns>An enumerable of validation problems; empty if the collection is valid.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
    private static IReadOnlyList<string> ValidateStringCollection(IEnumerable<string> collection, string itemTypeName)
    {
        ArgumentNullException.ThrowIfNull(collection);

        var problems = new List<string>();

        var index = 0;
        foreach (var item in collection)
        {
            if (string.IsNullOrWhiteSpace(item))
            {
                problems.Add($"{itemTypeName} at index {index} cannot be null or whitespace.");
            }

            index++;
        }

        return problems.AsReadOnly();
    }

    /// <summary>
    /// Validates chart annotations for correctness.
    /// </summary>
    /// <param name="annotations">The annotations to validate.</param>
    /// <returns>An enumerable of validation problems; empty if the annotations are valid.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="annotations"/> is <see langword="null"/>.</exception>
    private static IReadOnlyList<string> ValidateAnnotations(IEnumerable<ChartAnnotation> annotations)
    {
        ArgumentNullException.ThrowIfNull(annotations);

        var problems = new List<string>();

        var index = 0;
        foreach (var annotation in annotations)
        {
            if (annotation is null)
            {
                problems.Add($"Annotation at index {index} cannot be null.");
                continue;
            }

            // Validate annotation properties
            if (string.IsNullOrWhiteSpace(annotation.Label))
            {
                problems.Add($"Annotation.Label at index {index} cannot be null or whitespace.");
            }

            if (string.IsNullOrWhiteSpace(annotation.Color))
            {
                problems.Add($"Annotation.Color at index {index} cannot be null or whitespace.");
            }

            if (string.IsNullOrWhiteSpace(annotation.Tooltip))
            {
                problems.Add($"Annotation.Tooltip at index {index} cannot be null or whitespace.");
            }

            // Validate Value based on annotation type
            if (annotation.Type == ChartAnnotationType.ReferenceBand && !annotation.EndValue.HasValue)
            {
                problems.Add($"Annotation of type ReferenceBand at index {index} must have EndValue set.");
            }
            else if (annotation.Type == ChartAnnotationType.ReferenceBand && annotation.EndValue.HasValue)
            {
                if (annotation.Value > annotation.EndValue.Value)
                {
                    problems.Add(
                        $"Annotation of type ReferenceBand at index {index} has Value ({annotation.Value}) " +
                        $"greater than EndValue ({annotation.EndValue.Value}).");
                }
            }

            index++;
        }

        return problems.AsReadOnly();
    }
}
