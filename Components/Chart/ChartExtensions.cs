namespace BlazorComponentLibrary.Components.Chart;

/// <summary>
/// Provides extension methods for working with chart components.
/// </summary>
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
    /// <returns>The annotation that was added, allowing for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="chart"/> is <see langword="null"/>.</exception>
    public static ChartAnnotation AddThresholdLine<TData>(
        this IChart<TData> chart,
        double value,
        string? label = null,
        string? color = null,
        string? tooltip = null)
    {
        ArgumentNullException.ThrowIfNull(chart);

        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.ThresholdLine,
            Value = value,
            Label = label ?? string.Empty,
            Color = color ?? "#ff6384",
            Tooltip = tooltip ?? $"Threshold at {value}"
        };

        var annotations = chart.Annotations.ToList();
        annotations.Add(annotation);
        chart.Annotations = annotations;

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
    /// <returns>The annotation that was added, allowing for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="chart"/> is <see langword="null"/>.</exception>
    public static ChartAnnotation AddEventMarker<TData>(
        this IChart<TData> chart,
        double position,
        string? label = null,
        string? color = null,
        string? tooltip = null)
    {
        ArgumentNullException.ThrowIfNull(chart);

        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.EventMarker,
            Value = position,
            Label = label ?? string.Empty,
            Color = color ?? "#36a2eb",
            Tooltip = tooltip ?? $"Event at position {position}"
        };

        var annotations = chart.Annotations.ToList();
        annotations.Add(annotation);
        chart.Annotations = annotations;

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
    /// <param name="color">Optional color for the reference band. Defaults to "#4bc0c0" with 30% opacity.</param>
    /// <param name="tooltip">Optional tooltip text shown on hover.</param>
    /// <returns>The annotation that was added, allowing for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="chart"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="startValue"/> is greater than <paramref name="endValue"/>.</exception>
    public static ChartAnnotation AddReferenceBand<TData>(
        this IChart<TData> chart,
        double startValue,
        double endValue,
        string? label = null,
        string? color = null,
        string? tooltip = null)
    {
        ArgumentNullException.ThrowIfNull(chart);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startValue, endValue);

        var annotation = new ChartAnnotation
        {
            Type = ChartAnnotationType.ReferenceBand,
            Value = startValue,
            EndValue = endValue,
            Label = label ?? string.Empty,
            Color = color ?? "#4bc0c080", // 30% opacity
            Tooltip = tooltip ?? $"Reference band from {startValue} to {endValue}"
        };

        var annotations = chart.Annotations.ToList();
        annotations.Add(annotation);
        chart.Annotations = annotations;

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

        chart.Annotations = Enumerable.Empty<ChartAnnotation>();
        chart.Refresh();
    }

    /// <summary>
    /// Sets the chart title and refreshes the chart.
    /// </summary>
    /// <typeparam name="TData">The type of data in the chart.</typeparam>
    /// <param name="chart">The chart instance.</param>
    /// <param name="title">The title to set.</param>
    /// <exception cref="ArgumentNullException"><paramref name="chart"/> is <see langword="null"/>.</exception>
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
}