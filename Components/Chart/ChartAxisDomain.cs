namespace BlazorComponentLibrary.Components.Chart;

using System;
using System.Globalization;

/// <summary>
/// Specifies how annotation coordinates outside an <see cref="ChartAxisDomain"/> are handled.
/// </summary>
public enum ChartAnnotationDomainPolicy
{
    /// <summary>Clamp out-of-domain coordinates to the nearest domain boundary.</summary>
    Clamp,
    /// <summary>Keep out-of-domain coordinates unchanged and emit a trace warning.</summary>
    Warn
}

/// <summary>
/// Describes the visible numeric range of a chart axis that annotation coordinates are checked against.
/// </summary>
public sealed class ChartAxisDomain
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartAxisDomain"/> class.
    /// </summary>
    /// <param name="min">The inclusive lower bound of the axis.</param>
    /// <param name="max">The inclusive upper bound of the axis.</param>
    /// <exception cref="ArgumentException"><paramref name="min"/> or <paramref name="max"/> is NaN or infinite.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
    public ChartAxisDomain(double min, double max)
    {
        if (!double.IsFinite(min))
        {
            throw new ArgumentException(
                $"Axis domain minimum must be a finite number but was {min.ToString(CultureInfo.InvariantCulture)}.", nameof(min));
        }

        if (!double.IsFinite(max))
        {
            throw new ArgumentException(
                $"Axis domain maximum must be a finite number but was {max.ToString(CultureInfo.InvariantCulture)}.", nameof(max));
        }

        ArgumentOutOfRangeException.ThrowIfGreaterThan(min, max);

        Min = min;
        Max = max;
    }

    /// <summary>Gets the inclusive lower bound of the axis.</summary>
    public double Min { get; }

    /// <summary>Gets the inclusive upper bound of the axis.</summary>
    public double Max { get; }

    /// <summary>
    /// Determines whether a coordinate lies within the domain, inclusive of both bounds.
    /// </summary>
    /// <param name="value">The coordinate to test.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> lies within the domain; otherwise, <see langword="false"/>.</returns>
    public bool Contains(double value) => value >= Min && value <= Max;
}
