using System;
using System.Collections.Generic;
using System.Globalization;

namespace BlazorComponentLibrary.Components.Chart;

/// <summary>
/// Provides extension methods for working with <see cref="ChartAnnotation"/> instances.
/// </summary>
public static class ChartAnnotationExtensions
{
    /// <summary>
    /// Gets the display text for the annotation based on its type and configuration.
    /// </summary>
    /// <param name="annotation">The annotation instance.</param>
    /// <returns>A formatted string representing the annotation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="annotation"/> is <see langword="null"/>.</exception>
    public static string GetDisplayText(this ChartAnnotation annotation)
    {
        ArgumentNullException.ThrowIfNull(annotation);

        return annotation.Type switch
        {
            ChartAnnotationType.ThresholdLine => string.IsNullOrEmpty(annotation.Label)
                ? $"Threshold: {annotation.Value.ToString(CultureInfo.InvariantCulture)}"
                : annotation.Label,
            ChartAnnotationType.EventMarker => string.IsNullOrEmpty(annotation.Label)
                ? $"Event at {annotation.Value.ToString(CultureInfo.InvariantCulture)}"
                : annotation.Label,
            ChartAnnotationType.ReferenceBand => string.IsNullOrEmpty(annotation.Label)
                ? $"Range: {annotation.Value.ToString(CultureInfo.InvariantCulture)} - {annotation.EndValue?.ToString(CultureInfo.InvariantCulture)}"
                : annotation.Label,
            _ => string.IsNullOrEmpty(annotation.Label) ? annotation.Type.ToString() : annotation.Label
        };
    }

    /// <summary>
    /// Determines whether the annotation has a valid configuration for rendering.
    /// </summary>
    /// <param name="annotation">The annotation instance.</param>
    /// <returns><see langword="true"/> if the annotation is valid; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="annotation"/> is <see langword="null"/>.</exception>
    public static bool IsValid(this ChartAnnotation annotation)
    {
        ArgumentNullException.ThrowIfNull(annotation);

        return annotation.Type switch
        {
            ChartAnnotationType.ThresholdLine or ChartAnnotationType.EventMarker =>
                !double.IsNaN(annotation.Value) && !double.IsInfinity(annotation.Value),
            ChartAnnotationType.ReferenceBand =>
                !double.IsNaN(annotation.Value) &&
                !double.IsNaN(annotation.EndValue ?? double.NaN) &&
                !double.IsInfinity(annotation.Value) &&
                !double.IsInfinity(annotation.EndValue ?? double.NaN) &&
                annotation.EndValue.HasValue,
            _ => false
        };
    }

    /// <summary>
    /// Creates a deep copy of the annotation to allow modifications without affecting the original.
    /// </summary>
    /// <param name="annotation">The annotation instance.</param>
    /// <returns>A new <see cref="ChartAnnotation"/> instance with the same values.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="annotation"/> is <see langword="null"/>.</exception>
    public static ChartAnnotation Clone(this ChartAnnotation annotation)
    {
        ArgumentNullException.ThrowIfNull(annotation);

        return new ChartAnnotation
        {
            Type = annotation.Type,
            Value = annotation.Value,
            EndValue = annotation.EndValue,
            Label = annotation.Label,
            Color = annotation.Color,
            Tooltip = annotation.Tooltip
        };
    }

    /// <summary>
    /// Updates the annotation's color while preserving its other properties.
    /// </summary>
    /// <param name="annotation">The annotation instance.</param>
    /// <param name="color">The new color value in CSS format (e.g., "#ff0000", "red", "rgba(255,0,0,0.5)").</param>
    /// <exception cref="ArgumentNullException"><paramref name="annotation"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="color"/> is <see langword="null"/> or empty.</exception>
    public static void SetColor(this ChartAnnotation annotation, string color)
    {
        ArgumentNullException.ThrowIfNull(annotation);
        ArgumentException.ThrowIfNullOrEmpty(color);

        annotation.Color = color;
    }

    /// <summary>
    /// Updates the annotation's tooltip text while preserving its other properties.
    /// </summary>
    /// <param name="annotation">The annotation instance.</param>
    /// <param name="tooltip">The new tooltip text.</param>
    /// <exception cref="ArgumentNullException"><paramref name="annotation"/> is <see langword="null"/>.</exception>
    public static void SetTooltip(this ChartAnnotation annotation, string tooltip)
    {
        ArgumentNullException.ThrowIfNull(annotation);

        annotation.Tooltip = tooltip ?? string.Empty;
    }

    /// <summary>
    /// Gets the annotation's value as a formatted string using invariant culture.
    /// </summary>
    /// <param name="annotation">The annotation instance.</param>
    /// <returns>A formatted string representation of the value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="annotation"/> is <see langword="null"/>.</exception>
    public static string GetValueText(this ChartAnnotation annotation)
    {
        ArgumentNullException.ThrowIfNull(annotation);

        return annotation.Type == ChartAnnotationType.ReferenceBand && annotation.EndValue.HasValue
            ? $"{annotation.Value.ToString(CultureInfo.InvariantCulture)} - {annotation.EndValue.Value.ToString(CultureInfo.InvariantCulture)}"
            : annotation.Value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Determines whether the annotation has a label set.
    /// </summary>
    /// <param name="annotation">The annotation instance.</param>
    /// <returns><see langword="true"/> if the annotation has a non-empty label; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="annotation"/> is <see langword="null"/>.</exception>
    public static bool HasLabel(this ChartAnnotation annotation)
    {
        ArgumentNullException.ThrowIfNull(annotation);

        return !string.IsNullOrEmpty(annotation.Label);
    }
}