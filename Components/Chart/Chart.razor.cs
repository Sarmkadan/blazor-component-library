namespace BlazorComponentLibrary.Components.Chart;

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public sealed partial class Chart<TData> : ComponentBase, IChart<TData>
{
    private IEnumerable<TData> _data = Enumerable.Empty<TData>();
    private string? _dataHash;
    private Dictionary<string, object>? _geometryCache;

    [Parameter]
    public ChartType ChartType { get; set; }

    [Parameter]
    public string Title { get; set; } = string.Empty;

    [Parameter]
    public IEnumerable<string> Labels { get; set; } = Enumerable.Empty<string>();

    [Parameter]
    public IEnumerable<string> Colors { get; set; } = Enumerable.Empty<string>();

    [Parameter]
    public object Options { get; set; } = new object();

    /// <summary>
    /// Gets or sets annotations overlaid on the chart, such as threshold lines,
    /// event markers, and reference bands.
    /// </summary>
    [Parameter]
    public IEnumerable<ChartAnnotation> Annotations { get; set; } = Enumerable.Empty<ChartAnnotation>();

    /// <summary>
    /// Gets the cached geometry/path data for the current data set.
    /// </summary>
    protected Dictionary<string, object> GeometryCache
    {
        get
        {
            if (_geometryCache == null)
            {
                _geometryCache = new Dictionary<string, object>();
            }
            return _geometryCache;
        }
    }

    /// <summary>
    /// Gets the hash of the current data set.
    /// </summary>
    protected string? DataHash => _dataHash;

    /// <summary>
    /// Sets the data source for the chart.
    /// </summary>
    /// <param name="data">The enumerable collection of data items.</param>
    public void SetData(IEnumerable<TData> data)
    {
        _data = data ?? Enumerable.Empty<TData>();
        _dataHash = ComputeDataHash(_data);
        InvalidateCache();
        NotifyStateChanged();
    }

    /// <summary>
    /// Refreshes the chart, re-rendering its content.
    /// </summary>
    public void Refresh() => NotifyStateChanged();

    /// <summary>
    /// Sets the visibility of a series in the chart.
    /// </summary>
    /// <param name="seriesIndex">The index of the series to toggle.</param>
    /// <param name="visible">Whether the series should be visible.</param>
    public void SetSeriesVisibility(int seriesIndex, bool visible)
    {
        // This method is called from JavaScript interop when a legend item is clicked
        // This method signature satisfies the IChart interface
  // The actual visibility toggle is handled in JavaScript
  // This maintains the interface contract defined in IChart<TData>
  StateHasChanged();
    }

    private void NotifyStateChanged()
    {
        try
        {
            StateHasChanged();
        }
        catch (InvalidOperationException)
        {
            // Ignore if the component is not attached to a renderer (e.g. unit tests).
        }
    }

    /// <summary>
    /// Invalidates the geometry cache, forcing recomputation on next access.
    /// This should be called whenever any parameter that affects rendering changes.
    /// </summary>
    private void InvalidateCache()
    {
        _geometryCache = null;
    }

    /// <summary>
    /// Called when parameters are set, including during initial render.
    /// Invalidates cache when any rendering-affecting parameter changes.
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // Invalidate cache if any parameter that affects rendering has changed
        // This ensures geometry is recomputed when chart configuration changes
        InvalidateCache();
    }

    /// <summary>
    /// Computes a hash of the data set for use as a cache key.
    /// This ensures that geometry/path data is only recomputed when the data changes.
    /// </summary>
    /// <param name="data">The data collection to hash.</param>
    /// <returns>A hash string representing the data, or null if data is empty.</returns>
    private string? ComputeDataHash(IEnumerable<TData> data)
    {
        if (!data.Any())
        {
            return null;
        }

        try
        {
            // Create a stable hash by serializing the data to JSON
            // This provides a consistent representation of the data for comparison
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(json);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }
        catch
        {
            // If serialization fails, return null to disable caching for this data set
            return null;
        }
    }

    /// <summary>
    /// Gets or creates cached geometry data for a specific series.
    /// </summary>
    /// <param name="seriesKey">The key identifying the series.</param>
    /// <param name="createFunc">A function that computes the geometry if not cached.</param>
    /// <returns>The cached or newly computed geometry data.</returns>
    protected object GetOrCreateGeometry(string seriesKey, Func<object> createFunc)
    {
        // If data hash is null (empty data) or cache is null, compute fresh
        if (_dataHash == null || _geometryCache == null)
        {
            return createFunc();
        }

        // Try to get cached value
        if (_geometryCache.TryGetValue(seriesKey, out var cachedValue))
        {
            return cachedValue;
        }

        // Compute and cache the geometry
        var geometry = createFunc();
        _geometryCache[seriesKey] = geometry;
        return geometry;
    }

    /// <summary>
    /// Gets or sets custom content rendered inside the chart body, after the
    /// built-in summary and before any annotations.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

/// <summary>
/// Gets or sets a function to format numeric values for axis labels and tooltips.
/// Defaults to invariant culture formatting with "0.##" pattern.
/// </summary>
[Parameter]
public Func<double, string> ValueFormatter { get; set; } = value => value.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
}
