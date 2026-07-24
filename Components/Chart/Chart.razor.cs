namespace BlazorComponentLibrary.Components.Chart;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;

public sealed partial class Chart<TData> : ComponentBase, IChart<TData>, IDisposable, IAsyncDisposable
{
    private IEnumerable<TData> _data = Enumerable.Empty<TData>();
    private string? _dataHash;
    private Dictionary<string, object>? _geometryCache;
    private bool _disposed;
    private DotNetObjectReference<Chart<TData>>? _dotNetObjectReference;
    private bool _isInitialized;
    private bool _renderPending;
    private readonly object _renderLock = new object();
    private int _renderQueueId;
    private RenderQueueEntry? _pendingRender;
    private readonly SemaphoreSlim _jsModuleLoadLock = new SemaphoreSlim(1, 1);
    private IJSObjectReference? _jsModule;
    private ElementReference _chartElement;
    private DotNetObjectReference<IChart<TData>>? _dotNetObjectRefForAnnotations;

    [Inject]
    private ILogger<Chart<TData>>? Logger { get; set; }

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
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/></exception>
    public void SetData(IEnumerable<TData> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        _data = data;
        _dataHash = ComputeDataHash(_data);
        InvalidateCache();
        ScheduleRender(forceRefresh: false);
    }

    /// <summary>
    /// Refreshes the chart, re-rendering its content.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the chart has not been initialized yet.</exception>
    public void Refresh()
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("Chart must be initialized before calling Refresh(). Call SetData() first.");
        }

        ScheduleRender(forceRefresh: true);
    }

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
    /// Schedules a render operation with debouncing to prevent rapid successive renders.
    /// </summary>
    /// <param name="forceRefresh">Whether to force a full refresh regardless of cache.</param>
    private void ScheduleRender(bool forceRefresh)
    {
        lock (_renderLock)
        {
            if (_disposed || _renderPending)
            {
                return;
            }

            _renderPending = true;
            _renderQueueId++;
            var currentQueueId = _renderQueueId;

            _ = RenderChartAsync(currentQueueId, forceRefresh);
        }
    }

    /// <summary>
    /// Renders the chart via JavaScript interop with proper error handling and disposal checks.
    /// </summary>
    /// <param name="queueId">The current render queue ID to check for cancellation.</param>
    /// <param name="forceRefresh">Whether to force a full refresh regardless of cache.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task RenderChartAsync(int queueId, bool forceRefresh)
    {
        // Check if this render operation has been superseded by a newer one
        lock (_renderLock)
        {
            if (_renderQueueId != queueId || _disposed)
            {
                _renderPending = false;
                return;
            }
        }

        try
        {
            // Wait for the next render cycle to allow batching of multiple parameter changes
            await Task.Delay(16); // ~60fps equivalent

            // Check again after delay to see if this render is still valid
            lock (_renderLock)
            {
                if (_renderQueueId != queueId || _disposed)
                {
                    _renderPending = false;
                    return;
                }
            }

            // Create DotNetObjectReference for JavaScript interop if not already created
            if (_dotNetObjectReference == null)
            {
                _dotNetObjectReference = DotNetObjectReference.Create(this);
            }

            // Get current geometry cache for this render
            var geometryCache = new Dictionary<string, object>();
            if (_geometryCache != null)
            {
                foreach (var kvp in _geometryCache)
                {
                    geometryCache[kvp.Key] = kvp.Value;
                }
            }

            // Render the chart via JavaScript interop
            await RenderChartCoreAsync(geometryCache, _dataHash, forceRefresh);
        }
        catch (JSDisconnectedException)
        {
            // Component is being disposed, ignore this error
            Logger?.LogDebug("Chart render aborted due to JS disconnect during disposal");
        }
        catch (ObjectDisposedException)
        {
            // Component or DotNetObjectReference has been disposed
            Logger?.LogDebug("Chart render aborted due to object disposal");
        }
        catch (Exception ex) when (ex is not InvalidOperationException and not ArgumentException)
        {
            // Log other JavaScript interop errors but don't throw
            Logger?.LogError(ex, "Error rendering chart via JavaScript interop");
        }
        finally
        {
            lock (_renderLock)
            {
                if (_renderQueueId == queueId)
                {
                    _renderPending = false;
                }
            }
        }
    }

    /// <summary>
    /// Core chart rendering logic via JavaScript interop.
    /// </summary>
    /// <param name="geometryCache">The geometry cache to use for rendering.</param>
    /// <param name="dataHash">The data hash for cache validation.</param>
    /// <param name="forceRefresh">Whether to force a full refresh regardless of cache.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task RenderChartCoreAsync(Dictionary<string, object> geometryCache, string? dataHash, bool forceRefresh)
    {
        if (_disposed || JSRuntime == null)
        {
            return;
        }

        try
        {
            // Only initialize once
            if (!_isInitialized)
            {
                await InitializeChartInterop();
                _isInitialized = true;
            }

            // Invoke JavaScript to render/update the chart
            // This will handle the actual rendering based on current parameters
            await JSRuntime.InvokeVoidAsync(
                "blazorChartInterop.renderChart",
                _chartElement,
                ChartType,
                Title,
                Labels,
                Colors,
                Options,
                Annotations,
                ValueFormatter,
                geometryCache,
                dataHash,
                forceRefresh,
                _dotNetObjectReference
            );
        }
        catch (JSException ex) when (ex.Message.Contains("prerender", StringComparison.OrdinalIgnoreCase) ||
                                      ex.Message.Contains("not available", StringComparison.OrdinalIgnoreCase))
        {
            // Ignore errors during prerendering or when JS is not available
            Logger?.LogDebug(ex, "Chart render skipped during prerender or unavailable JS environment");
        }
        catch (JSException ex)
        {
            // Log other JavaScript errors
            Logger?.LogWarning(ex, "Chart render failed with JavaScript error");
        }
    }

    /// <summary>
    /// Initializes JavaScript interop modules lazily to avoid errors during prerendering.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task InitializeChartInterop()
    {
        if (_disposed || JSRuntime == null)
        {
            return;
        }

        try
        {
            // Lazy-load the JavaScript module
            if (_jsModule == null)
            {
                await _jsModuleLoadLock.WaitAsync();
                try
                {
                    if (_jsModule == null)
                    {
                        _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                            "import", "./_content/BlazorComponentLibrary/js/blazor-chart-interop.js");
                    }
                }
                finally
                {
                    _jsModuleLoadLock.Release();
                }
            }

            // Initialize the chart interop
            await JSRuntime.InvokeVoidAsync("blazorChartInterop.initialize", _chartElement);
        }
        catch (JSException ex) when (ex.Message.Contains("prerender", StringComparison.OrdinalIgnoreCase) ||
                                      ex.Message.Contains("not available", StringComparison.OrdinalIgnoreCase) ||
                                      ex.Message.Contains("import", StringComparison.OrdinalIgnoreCase))
        {
            // Ignore errors during prerendering, when JS is not available, or module import fails
            Logger?.LogDebug(ex, "Chart interop initialization skipped during prerender or unavailable JS environment");
        }
        catch (Exception ex)
        {
            // Log initialization errors but don't throw
            Logger?.LogDebug(ex, "Chart interop initialization failed");
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

        // Schedule a render when parameters change
        // This handles cases where parameters are set before the component is rendered
        if (_isInitialized)
        {
            ScheduleRender(forceRefresh: true);
        }
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

    /// <summary>
    /// Gets or sets a value indicating whether the chart should be rendered lazily.
    /// When true, JavaScript interop initialization is deferred until the chart is visible.
    /// </summary>
    [Parameter]
    public bool LazyLoad { get; set; }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        // Clean up DotNetObjectReference if it exists
        if (_dotNetObjectReference != null)
        {
            try
            {
                _dotNetObjectReference.Dispose();
            }
            catch (ObjectDisposedException)
            {
                // Ignore if already disposed
            }
            _dotNetObjectReference = null;
        }

        // Clean up DotNetObjectReference for annotations
        if (_dotNetObjectRefForAnnotations != null)
        {
            try
            {
                _dotNetObjectRefForAnnotations.Dispose();
            }
            catch (ObjectDisposedException)
            {
                // Ignore if already disposed
            }
            _dotNetObjectRefForAnnotations = null;
        }

        // Clean up JavaScript module
        if (_jsModule != null)
        {
            try
            {
                _jsModule.DisposeAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex) when (ex is not InvalidOperationException and not ArgumentException)
            {
                Logger?.LogDebug(ex, "Error disposing JavaScript module");
            }
            _jsModule = null;
        }

        // Clean up any JavaScript interop event subscriptions
        // Note: Actual cleanup would be implemented in JavaScript interop layer
        _disposed = true;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        // Clean up DotNetObjectReference if it exists
        if (_dotNetObjectReference != null)
        {
            try
            {
                _dotNetObjectReference.Dispose();
            }
            catch (ObjectDisposedException)
            {
                // Ignore if already disposed
            }
            _dotNetObjectReference = null;
        }

        // Clean up DotNetObjectReference for annotations
        if (_dotNetObjectRefForAnnotations != null)
        {
            try
            {
                _dotNetObjectRefForAnnotations.Dispose();
            }
            catch (ObjectDisposedException)
            {
                // Ignore if already disposed
            }
            _dotNetObjectRefForAnnotations = null;
        }

        // Clean up JavaScript module
        if (_jsModule != null)
        {
            try
            {
                await _jsModule.DisposeAsync();
            }
            catch (Exception ex) when (ex is not InvalidOperationException and not ArgumentException)
            {
                Logger?.LogDebug(ex, "Error disposing JavaScript module");
            }
            _jsModule = null;
        }

        // Clean up any JavaScript interop event subscriptions
        // Note: Actual cleanup would be implemented in JavaScript interop layer
        _disposed = true;
        await ValueTask.CompletedTask;
    }

    /// <summary>
    /// Represents a pending render operation in the queue.
    /// </summary>
    private sealed class RenderQueueEntry
    {
        public int QueueId { get; }
        public DateTime Timestamp { get; }
        public Dictionary<string, object> GeometryCache { get; }
        public string? DataHash { get; }
        public bool ForceRefresh { get; }

        public RenderQueueEntry(int queueId, Dictionary<string, object> geometryCache, string? dataHash, bool forceRefresh)
        {
            QueueId = queueId;
            Timestamp = DateTime.UtcNow;
            GeometryCache = geometryCache;
            DataHash = dataHash;
            ForceRefresh = forceRefresh;
        }
    }
}