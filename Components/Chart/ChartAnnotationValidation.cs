namespace BlazorComponentLibrary.Components.Chart;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

/// <summary>
/// Provides validation helpers for <see cref="ChartAnnotation"/> instances.
/// </summary>
public static class ChartAnnotationValidation
{
    /// <summary>
    /// Validates a <see cref="ChartAnnotation"/> instance and returns a list of human-readable problems.
    /// </summary>
    /// <param name="value">The annotation instance to validate.</param>
    /// <returns>An enumerable of validation problems; empty if the annotation is valid.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public static IReadOnlyList<string> Validate(this ChartAnnotation value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = new List<string>();

        // Validate Type - must be a valid enum value
        if (value.Type == default)
        {
            problems.Add("ChartAnnotation.Type must be set to a valid ChartAnnotationType value.");
        }

        // Validate Value - must not be NaN or Infinity
        if (double.IsNaN(value.Value))
        {
            problems.Add("ChartAnnotation.Value cannot be NaN (Not a Number).");
        }

        if (double.IsInfinity(value.Value))
        {
            problems.Add("ChartAnnotation.Value cannot be infinite.");
        }


        // Validate Label - must not be null or whitespace
        if (string.IsNullOrWhiteSpace(value.Label))
        {
            problems.Add("ChartAnnotation.Label cannot be null or whitespace.");
        }

        // Validate Color - must not be null or whitespace
        if (string.IsNullOrWhiteSpace(value.Color))
        {
            problems.Add("ChartAnnotation.Color cannot be null or whitespace.");
        }

        // Validate Tooltip - can be empty but not null
        ArgumentNullException.ThrowIfNull(value.Tooltip);

        // Validate EndValue based on annotation type
        switch (value.Type)
        {
            case ChartAnnotationType.ReferenceBand:
                if (!value.EndValue.HasValue)
                {
                    problems.Add("ChartAnnotation.EndValue must be set for ReferenceBand annotations.");
                }
                else
                {
                    // Validate EndValue is not NaN or Infinity
                    if (double.IsNaN(value.EndValue.Value))
                    {
                        problems.Add("ChartAnnotation.EndValue cannot be NaN (Not a Number).");
                    }

                    if (double.IsInfinity(value.EndValue.Value))
                    {
                        problems.Add("ChartAnnotation.EndValue cannot be infinite.");
                    }

                    // Validate Value <= EndValue for ReferenceBand
                    if (value.Value > value.EndValue.Value)
                    {
                        problems.Add(
                            $"ChartAnnotation.Value ({value.Value.ToString(CultureInfo.InvariantCulture)}) " +
                            $"must be less than or equal to EndValue ({value.EndValue.Value.ToString(CultureInfo.InvariantCulture)}).");
                    }
                }
                break;

            case ChartAnnotationType.ThresholdLine:
            case ChartAnnotationType.EventMarker:
                // EndValue should be null for these types
                if (value.EndValue.HasValue)
                {
                    problems.Add(
                        $"ChartAnnotation.EndValue should not be set for {value.Type} annotations. " +
                        "Use Value only for threshold lines and event markers.");
                }
                break;

            default:
                problems.Add($"Unknown ChartAnnotationType value: {value.Type}.");
                break;
        }

        return problems.AsReadOnly();
    }

    /// <summary>
    /// Determines whether a <see cref="ChartAnnotation"/> instance is valid.
    /// </summary>
    /// <param name="value">The annotation instance to check.</param>
    /// <returns><see langword="true"/> if the annotation is valid; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public static bool IsValid(this ChartAnnotation value) => Validate(value).Count == 0;

    /// <summary>
    /// Ensures that a <see cref="ChartAnnotation"/> instance is valid, throwing an exception if it is not.
    /// </summary>
    /// <param name="value">The annotation instance to validate.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="value"/> has validation problems.</exception>
    public static void EnsureValid(this ChartAnnotation value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = Validate(value);

        if (problems.Count > 0)
        {
            throw new ArgumentException(
                $"ChartAnnotation validation failed:{Environment.NewLine}{string.Join(Environment.NewLine, problems)}");
        }
    }
}
