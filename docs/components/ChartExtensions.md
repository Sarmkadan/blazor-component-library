# ChartExtensions

Provides extension methods for configuring and annotating Blazor charts with data, titles, and visual annotations such as threshold lines, event markers, and reference bands.

## API

### SetDataAndRefresh<TData>

Configures the chart with the provided data and triggers a refresh to render the updated state.

- **Type parameter**: `TData` - The type of data to display in the chart.
- **Parameters**:
  - `chart`: The chart instance to update.
  - `data`: The data series to set on the chart.
- **Throws**: `ArgumentNullException` if `chart` or `data` is null.
- **Remarks**: This method replaces any existing data on the chart and immediately refreshes the display.

### AddThresholdLine<TData>

Adds a horizontal threshold line annotation to the chart at the specified value.

- **Type parameter**: `TData` - The type of data the chart displays.
- **Parameters**:
  - `chart`: The chart instance.
  - `value`: The y-axis value where the threshold line should be drawn.
  - `label`: Optional text label to display alongside the threshold line.
  - `color`: The color of the threshold line. Defaults to a standard warning color if not specified.
  - `lineWidth`: The width of the threshold line. Defaults to 2 if not specified.
- **Returns**: A `ChartAnnotation` instance representing the added threshold line, which can be used to modify or remove the annotation later.
- **Throws**: `ArgumentNullException` if `chart` is null.
- **Remarks**: The threshold line is drawn horizontally across the chart at the specified y-axis value. If a label is provided, it appears near the line.

### AddEventMarker<TData>

Adds a vertical event marker annotation to the chart at the specified x-axis position.

- **Type parameter**: `TData` - The type of data the chart displays.
- **Parameters**:
  - `chart`: The chart instance.
  - `xValue`: The x-axis value where the event marker should be drawn.
  - `label`: Optional text label to display alongside the event marker.
  - `color`: The color of the event marker. Defaults to a standard accent color if not specified.
  - `lineWidth`: The width of the event marker line. Defaults to 2 if not specified.
- **Returns**: A `ChartAnnotation` instance representing the added event marker, which can be used to modify or remove the annotation later.
- **Throws**: `ArgumentNullException` if `chart` is null.
- **Remarks**: The event marker is drawn vertically from the top to bottom of the chart at the specified x-axis position. If a label is provided, it appears near the marker.

### AddReferenceBand<TData>

Adds a reference band annotation to the chart between two y-axis values, highlighting a range of interest.

- **Type parameter**: `TData` - The type of data the chart displays.
- **Parameters**:
  - `chart`: The chart instance.
  - `lowerValue`: The lower bound of the reference band on the y-axis.
  - `upperValue`: The upper bound of the reference band on the y-axis.
  - `label`: Optional text label to display within the reference band.
  - `color`: The fill color of the reference band. Defaults to a subtle highlight color if not specified.
  - `opacity`: The opacity of the reference band fill. Defaults to 0.2 if not specified.
- **Returns**: A `ChartAnnotation` instance representing the added reference band, which can be used to modify or remove the annotation later.
- **Throws**: `ArgumentNullException` if `chart` is null.
- **Remarks**: The reference band is drawn as a filled rectangle between the specified y-axis values, spanning the entire width of the chart. If a label is provided, it appears within the band area.

### ClearAnnotations<TData>

Removes all annotations (threshold lines, event markers, and reference bands) from the chart.

- **Type parameter**: `TData` - The type of data the chart displays.
- **Parameters**:
  - `chart`: The chart instance to clear.
- **Throws**: `ArgumentNullException` if `chart` is null.
- **Remarks**: This method only removes annotations added via the `ChartExtensions` methods. It does not affect the chart's data or other configuration.

### SetTitle<TData>

Sets the title text for the chart.

- **Type parameter**: `TData` - The type of data the chart displays.
- **Parameters**:
  - `chart`: The chart instance to update.
  - `title`: The title text to display at the top of the chart.
  - `fontSize`: Optional font size for the title. Defaults to the chart's default title font size if not specified.
  - `color`: Optional color for the title text. Defaults to the chart's default title color if not specified.
- **Throws**: `ArgumentNullException` if `chart` or `title` is null.
- **Remarks**: The title is displayed at the top of the chart and provides context for the data being visualized.

### SetChartType<TData>

Changes the chart type (e.g., line, bar, pie) and refreshes the display.

- **Type parameter**: `TData` - The type of data the chart displays.
- **Parameters**:
  - `chart`: The chart instance to update.
  - `chartType`: The new chart type to apply.
- **Throws**: `ArgumentNullException` if `chart` is null.
- **Remarks**: Changing the chart type replaces the current visualization with the new type while preserving the data and annotations. The chart automatically refreshes to show the updated visualization.

## Usage

### Example 1: Creating a threshold line to highlight a performance target

```csharp
@using Blazor.Charts

<Chart @ref="chartRef" />

<button @onclick="AddPerformanceThreshold">Add Threshold</button>

@code {
    private Chart chartRef;
    private List<SalesData> salesData = new();

    
    protected override void OnInitialized()
    {
        // Initialize with sample data
        salesData = GetSalesData();
        ChartExtensions.SetDataAndRefresh(chartRef, salesData);
    }

    private void AddPerformanceThreshold()
    {
        // Add a threshold line at $10,000 sales
        ChartExtensions.AddThresholdLine(chartRef, 10000, "Target: $10K", "#ff9800", 3);
    }

    private List<SalesData> GetSalesData()
    {
        return new List<SalesData> {
            new SalesData { Month = "Jan", Amount = 8500 },
            new SalesData { Month = "Feb", Amount = 9200 },
            new SalesData { Month = "Mar", Amount = 11500 },
            new SalesData { Month = "Apr", Amount = 10800 }
        };
    }

    public class SalesData
    {
        public string Month { get; set; }
        public decimal Amount { get; set; }
    }
}
```

### Example 2: Adding multiple annotations to highlight key events and ranges

```csharp
@using Blazor.Charts

<Chart @ref="chartRef" />

<button @onclick="AddAnnotations">Add Annotations</button>

<button @onclick="ClearAllAnnotations">Clear Annotations</button>

@code {
    private Chart chartRef;
    private List<PerformanceData> performanceData = new();

    protected override void OnInitialized()
    {
        // Initialize with sample data
        performanceData = GetPerformanceData();
        ChartExtensions.SetDataAndRefresh(chartRef, performanceData);
    }

    private void AddAnnotations()
    {
        // Add a reference band for acceptable performance range (70-90)
        ChartExtensions.AddReferenceBand(chartRef, 70, 90, "Acceptable Range", "#4caf50", 0.3);
        
        // Add an event marker for the product launch
        ChartExtensions.AddEventMarker(chartRef, new DateTime(2024, 3, 15), "Product Launch", "#9c27b0", 3);
        
        // Add a threshold line for the target performance
        ChartExtensions.AddThresholdLine(chartRef, 95, "Target: 95%", "#ff5722", 2);
    }

    private void ClearAllAnnotations()
    {
        ChartExtensions.ClearAnnotations(chartRef);
    }

    private List<PerformanceData> GetPerformanceData()
    {
        return new List<PerformanceData> {
            new PerformanceData { Date = new DateTime(2024, 1, 1), Score = 65 },
            new PerformanceData { Date = new DateTime(2024, 2, 1), Score = 72 },
            new PerformanceData { Date = new DateTime(2024, 3, 1), Score = 88 },
            new PerformanceData { Date = new DateTime(2024, 4, 1), Score = 91 },
            new PerformanceData { Date = new DateTime(2024, 5, 1), Score = 96 }
        };
    }

    public class PerformanceData
    {
        public DateTime Date { get; set; }
        public int Score { get; set; }
    }
}
```

## Notes

- **Thread Safety**: These extension methods are designed for single-threaded UI contexts typical in Blazor applications. They should not be called from background threads without proper synchronization.

- **Chart Refresh**: Methods that modify the chart's visual state (`SetDataAndRefresh`, `SetChartType`) automatically trigger a refresh. Methods that add annotations do not automatically refresh the chart; call `chart.Refresh()` explicitly if needed.

- **Annotation Management**: Each annotation method returns a `ChartAnnotation` object that can be stored and used to modify or remove the annotation later. Annotations persist until explicitly cleared or the chart is disposed.

- **Parameter Validation**: All methods validate their parameters and throw `ArgumentNullException` for null chart references. Optional parameters have sensible defaults to maintain consistent behavior.

- **Type Safety**: The generic type parameter `TData` ensures type safety when working with chart data, preventing invalid data types from being assigned to the chart.
- **Memory Management**: Annotations added to the chart are managed by the chart component. Ensure proper disposal of chart instances to avoid memory leaks in long-running applications.