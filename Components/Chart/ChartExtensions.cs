namespace BlazorComponentLibrary.Components.Chart;

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

/// <summary>
/// Provides extension methods for working with chart components.
/// </summary>
/// <remarks>
/// This static class contains fluent extension methods for <see cref="IChart{TData}"/>
/// that simplify common chart operations like adding annotations, setting data, and configuring chart properties.
/// </remarks>
public static class ChartExtensions
{
    /// <summary>
    /// Sets the chart data and refreshes the chart in a single operation.
    /// </summary>
    /// <typeparam name="TData">The type of data in the chart.</typeparam>
    /// <param name="chart">The chart instance.</param>
    /// <param name="data">The data to set.</param>
    /// <exception cref="ArgumentNullException"><paramref name="chart"/> is <see langword="null"/>.</exception>
    public static void SetDataAndRefresh<TData>(
        this IChart<TData> chart,
        IEnumerable<TData> data)
    {
        ArgumentNullException.ThrowIfNull(chart);
        chart.SetData(data);
        chart.Refresh();
    }

    /// <summary>
    /// Adds a threshold line annotation to the chart at the specified value.
    /// </summary>
    /// <typeparam name="TData">The type of data in the chart.</typeparam>
    /// <param name="chart">The chart instance.</param>
    /// <param name="value">The threshold value where the line should be drawn.</param>
    /// <param name="label">Optional label for the threshold line.</param>
    /// <param name="color">Optional color for the threshold line. Defaults to "#ff6384".</param>
    /// <param name="tooltip">Optional tooltip text shown on hover.</param>
    /// <param name="domain">Optional axis domain the value is checked against. When <see langword="null"/>, no domain check is performed.</param>
    /// <param name="domainPolicy">How a value outside <paramref name="domain"/> is handled. Defaults to <see cref="ChartAnnotationDomainPolicy.Clamp"/>.</param>
    /// <returns>The annotation that was added, allowing for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="chart"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="value"/> is NaN or infinite.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="domainPolicy"/> is not a defined <see cref="ChartAnnotationDomainPolicy"/> value.</exception>
    public static ChartAnnotation AddThresholdLine<TData>(
        this IChart<TData> chart,
        double value,
        string? label = null,
        string? color = null,
        string? tooltip = null,
        ChartAxisDomain? domain = null,
        ChartAnnotationDomainPolicy domainPolicy = ChartAnnotationDomainPolicy.Clamp)
    {
        ArgumentNullException.ThrowIfNull(chart);

        value = PlacePoint(value, nameof(value), domain, domainPolicy);

        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.ThresholdLine,
            Value = value,
            Label = label ?? string.Empty,
            Color = color ?? "#ff6384",
            Tooltip = tooltip ?? $"Threshold at {value}"
        };

        chart.Annotations = chart.Annotations.Append(annotation).ToList();
        chart.Refresh();
        return annotation;
    }

    /// <summary>
    /// Adds an event marker annotation to the chart at the specified position.
    /// </summary>
    /// <typeparam name="TData">The type of data in the chart.</typeparam>
    /// <param name="chart">The chart instance.</param>
    /// <param name="position">The position on the x-axis where the marker should be drawn.</param>
    /// <param name="label">Optional label for the event marker.</param>
    /// <param name="color">Optional color for the event marker. Defaults to "#36a2eb".</param>
    /// <param name="tooltip">Optional tooltip text shown on hover.</param>
    /// <param name="domain">Optional axis domain the position is checked against. When <see langword="null"/>, no domain check is performed.</param>
    /// <param name="domainPolicy">How a position outside <paramref name="domain"/> is handled. Defaults to <see cref="ChartAnnotationDomainPolicy.Clamp"/>.</param>
    /// <returns>The annotation that was added, allowing for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="chart"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="position"/> is NaN or infinite.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="domainPolicy"/> is not a defined <see cref="ChartAnnotationDomainPolicy"/> value.</exception>
    public static ChartAnnotation AddEventMarker<TData>(
        this IChart<TData> chart,
        double position,
        string? label = null,
        string? color = null,
        string? tooltip = null,
        ChartAxisDomain? domain = null,
        ChartAnnotationDomainPolicy domainPolicy = ChartAnnotationDomainPolicy.Clamp)
    {
        ArgumentNullException.ThrowIfNull(chart);

        position = PlacePoint(position, nameof(position), domain, domainPolicy);

        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.EventMarker,
            Value = position,
            Label = label ?? string.Empty,
            Color = color ?? "#36a2eb",
            Tooltip = tooltip ?? $"Event at position {position}"
        };

        chart.Annotations = chart.Annotations.Append(annotation).ToList();
        chart.Refresh();
        return annotation;
    }

    /// <summary>
    /// Adds a reference band annotation to the chart between two values.
    /// </summary>
    /// <typeparam name="TData">The type of data in the chart.</typeparam>
    /// <param name="chart">The chart instance.</param>
    /// <param name="startValue">The start value of the reference band.</param>
    /// <param name="endValue">The end value of the reference band.</param>
    /// <param name="label">Optional label for the reference band.</param>
    /// <param name="color">Optional color for the reference band. Defaults to "#4bc0c080" (30% opacity).</param>
    /// <param name="tooltip">Optional tooltip text shown on hover.</param>
    /// <param name="domain">Optional axis domain the band is checked against. When <see langword="null"/>, no domain check is performed.</param>
    /// <param name="domainPolicy">How a band endpoint outside <paramref name="domain"/> is handled. Defaults to <see cref="ChartAnnotationDomainPolicy.Clamp"/>.</param>
    /// <returns>The annotation that was added, allowing for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="chart"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="startValue"/> or <paramref name="endValue"/> is NaN or infinite.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="startValue"/> is greater than <paramref name="endValue"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="domainPolicy"/> is <see cref="ChartAnnotationDomainPolicy.Clamp"/> and the band lies entirely outside <paramref name="domain"/>.
    /// </exception>
    public static ChartAnnotation AddReferenceBand<TData>(
        this IChart<TData> chart,
        double startValue,
        double endValue,
        string? label = null,
        string? color = null,
        string? tooltip = null,
        ChartAxisDomain? domain = null,
        ChartAnnotationDomainPolicy domainPolicy = ChartAnnotationDomainPolicy.Clamp)
    {
        ArgumentNullException.ThrowIfNull(chart);

        (startValue, endValue) = PlaceRange(startValue, endValue, domain, domainPolicy);

        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.ReferenceBand,
            Value = startValue,
            EndValue = endValue,
            Label = label ?? string.Empty,
            Color = color ?? "#4bc0c080",
            Tooltip = tooltip ?? $"Reference band from {startValue} to {endValue}"
        };

        chart.Annotations = chart.Annotations.Append(annotation).ToList();
        chart.Refresh();
        return annotation;
    }

    /// <summary>
    /// Clears all annotations from the chart.
    /// </summary>
    /// <typeparam name="TData">The type of data in the chart.</typeparam>
    /// <param name="chart">The chart instance.</param>
    /// <exception cref="ArgumentNullException"><paramref name="chart"/> is <see langword="null"/>.</exception>
    public static void ClearAnnotations<TData>(this IChart<TData> chart)
    {
        ArgumentNullException.ThrowIfNull(chart);

        chart.Annotations = [];
        chart.Refresh();
    }

    /// <summary>
    /// Sets the chart title and refreshes the chart.
    /// </summary>
    /// <typeparam name="TData">The type of data in the chart.</typeparam>
    /// <param name="chart">The chart instance.</param>
    /// <param name="title">The title to set.</param>
    /// <exception cref="ArgumentNullException"><paramref name="chart"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="title"/> is <see langword="null"/> or empty.</exception>
    public static void SetTitle<TData>(
        this IChart<TData> chart,
        string title)
    {
        ArgumentNullException.ThrowIfNull(chart);
        ArgumentException.ThrowIfNullOrEmpty(title);

        chart.Title = title;
        chart.Refresh();
    }

    /// <summary>
    /// Sets the chart type and refreshes the chart.
    /// </summary>
    /// <typeparam name="TData">The type of data in the chart.</typeparam>
    /// <param name="chart">The chart instance.</param>
    /// <param name="chartType">The chart type to set.</param>
    /// <exception cref="ArgumentNullException"><paramref name="chart"/> is <see langword="null"/>.</exception>
    public static void SetChartType<TData>(
        this IChart<TData> chart,
        ChartType chartType)
    {
        ArgumentNullException.ThrowIfNull(chart);

        chart.ChartType = chartType;
        chart.Refresh();
    }

    /// <summary>
    /// Sets the visibility of a series in the chart.
    /// </summary>
    /// <typeparam name="TData">The type of data in the chart.</typeparam>
    /// <param name="chart">The chart instance.</param>
    /// <param name="seriesIndex">The index of the series to toggle.</param>
    /// <param name="visible">Whether the series should be visible.</param>
    /// <exception cref="ArgumentNullException"><paramref name="chart"/> is <see langword="null"/>.</exception>
    public static void SetSeriesVisibility<TData>(
        this IChart<TData> chart,
        int seriesIndex,
        bool visible)
    {
        ArgumentNullException.ThrowIfNull(chart);

        chart.SetSeriesVisibility(seriesIndex, visible);
        chart.Refresh();
    }

    /// <summary>
    /// Gets or creates cached geometry data for a specific series.
    /// This method allows components to cache expensive geometry/path computations
    /// and reuse them when the data hasn't changed.
    /// </summary>
    /// <typeparam name="TData">The type of data in the chart.</typeparam>
    /// <param name="chart">The chart instance.</param>
    /// <param name="seriesKey">The key identifying the series.</param>
    /// <param name="createFunc">A function that computes the geometry if not cached.</param>
    /// <returns>The cached or newly computed geometry data.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="chart"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="seriesKey"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="createFunc"/> is <see langword="null"/>.</exception>
    public static object GetOrCreateGeometry<TData>(
        this IChart<TData> chart,
        string seriesKey,
        Func<object> createFunc)
    {
        ArgumentNullException.ThrowIfNull(chart);
        ArgumentNullException.ThrowIfNull(seriesKey);
        ArgumentNullException.ThrowIfNull(createFunc);

        if (chart is Chart<TData> typedChart)
        {
            return typedChart.GetOrCreateGeometry(seriesKey, createFunc);
        }

        // Fallback for interface usage
        return createFunc();
    }

    /// <summary>
    /// Validates a point coordinate and resolves it against an optional axis domain.
    /// </summary>
    /// <param name="value">The coordinate to place.</param>
    /// <param name="paramName">The name of the coordinate parameter, used in exception messages.</param>
    /// <param name="domain">The axis domain, or <see langword="null"/> to skip the domain check.</param>
    /// <param name="domainPolicy">How a coordinate outside the domain is handled.</param>
    /// <returns>The coordinate to store on the annotation.</returns>
    /// <exception cref="ArgumentException"><paramref name="value"/> is NaN or infinite.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="domainPolicy"/> is not a defined value.</exception>
    private static double PlacePoint(double value, string paramName, ChartAxisDomain? domain, ChartAnnotationDomainPolicy domainPolicy)
    {
        EnsureDefinedPolicy(domainPolicy);
        EnsureFinite(value, paramName);

        return domain is null ? value : ResolveAgainstDomain(value, paramName, domain, domainPolicy);
    }

    /// <summary>
    /// Validates a range's endpoints and resolves them against an optional axis domain.
    /// </summary>
    /// <param name="start">The start of the range.</param>
    /// <param name="end">The end of the range.</param>
    /// <param name="domain">The axis domain, or <see langword="null"/> to skip the domain check.</param>
    /// <param name="domainPolicy">How endpoints outside the domain are handled.</param>
    /// <returns>The start and end values to store on the annotation.</returns>
    /// <exception cref="ArgumentException"><paramref name="start"/> or <paramref name="end"/> is NaN or infinite.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="start"/> is greater than <paramref name="end"/>, or the range lies entirely outside
    /// <paramref name="domain"/> under <see cref="ChartAnnotationDomainPolicy.Clamp"/>.
    /// </exception>
    private static (double Start, double End) PlaceRange(double start, double end, ChartAxisDomain? domain, ChartAnnotationDomainPolicy domainPolicy)
    {
        EnsureDefinedPolicy(domainPolicy);
        EnsureFinite(start, "startValue");
        EnsureFinite(end, "endValue");

        if (start > end)
        {
            throw new ArgumentOutOfRangeException(
                "startValue",
                start,
                $"Reference band start ({Format(start)}) must be less than or equal to its end ({Format(end)}).");
        }

        if (domain is null)
        {
            return (start, end);
        }

        if (domainPolicy == ChartAnnotationDomainPolicy.Clamp && (end < domain.Min || start > domain.Max))
        {
            throw new ArgumentOutOfRangeException(
                "startValue",
                start,
                $"Reference band [{Format(start)}, {Format(end)}] lies entirely outside the axis domain " +
                $"[{Format(domain.Min)}, {Format(domain.Max)}] and cannot be clamped onto it.");
        }

        return (
            ResolveAgainstDomain(start, "startValue", domain, domainPolicy),
            ResolveAgainstDomain(end, "endValue", domain, domainPolicy));
    }

    /// <summary>
    /// Applies the domain policy to a single coordinate that is known to be finite.
    /// </summary>
    /// <param name="value">The finite coordinate to resolve.</param>
    /// <param name="paramName">The name of the coordinate parameter, used in exception messages.</param>
    /// <param name="domain">The axis domain.</param>
    /// <param name="domainPolicy">How a coordinate outside the domain is handled.</param>
    /// <returns>The clamped value under <see cref="ChartAnnotationDomainPolicy.Clamp"/>, or the unchanged value under <see cref="ChartAnnotationDomainPolicy.Warn"/>.</returns>
    private static double ResolveAgainstDomain(double value, string paramName, ChartAxisDomain domain, ChartAnnotationDomainPolicy domainPolicy)
    {
        if (domain.Contains(value))
        {
            return value;
        }

        return domainPolicy switch
        {
            ChartAnnotationDomainPolicy.Clamp => Math.Clamp(value, domain.Min, domain.Max),
            _ => WarnOutsideDomain(value, paramName, domain)
        };
    }

    /// <summary>
    /// Emits a trace warning for a coordinate outside the axis domain and returns it unchanged.
    /// </summary>
    /// <param name="value">The out-of-domain coordinate.</param>
    /// <param name="paramName">The name of the coordinate parameter.</param>
    /// <param name="domain">The axis domain that was violated.</param>
    /// <returns>The unchanged <paramref name="value"/>.</returns>
    private static double WarnOutsideDomain(double value, string paramName, ChartAxisDomain domain)
    {
        Trace.TraceWarning(
            $"Chart annotation {paramName} ({Format(value)}) is outside the axis domain " +
            $"[{Format(domain.Min)}, {Format(domain.Max)}]; the value was kept unchanged.");

        return value;
    }

    /// <summary>
    /// Ensures the domain policy is a defined <see cref="ChartAnnotationDomainPolicy"/> value.
    /// </summary>
    /// <param name="domainPolicy">The policy to check.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="domainPolicy"/> is not a defined value.</exception>
    private static void EnsureDefinedPolicy(ChartAnnotationDomainPolicy domainPolicy)
    {
        if (!Enum.IsDefined(domainPolicy))
        {
            throw new ArgumentOutOfRangeException(
                "domainPolicy", domainPolicy, "Value must be a defined ChartAnnotationDomainPolicy.");
        }
    }

    /// <summary>
    /// Ensures a coordinate is a finite number, naming the offending value in the exception message.
    /// </summary>
    /// <param name="value">The coordinate to check.</param>
    /// <param name="paramName">The name of the coordinate parameter.</param>
    /// <exception cref="ArgumentException"><paramref name="value"/> is NaN or infinite.</exception>
    private static void EnsureFinite(double value, string paramName)
    {
        if (!double.IsFinite(value))
        {
            throw new ArgumentException(
                $"Chart annotation {paramName} must be a finite number but was {Format(value)}.", paramName);
        }
    }

    /// <summary>
    /// Formats a coordinate using invariant culture for use in exception and trace messages.
    /// </summary>
    /// <param name="value">The coordinate to format.</param>
    /// <returns>The invariant-culture string representation of <paramref name="value"/>.</returns>
    private static string Format(double value) => value.ToString(CultureInfo.InvariantCulture);
}