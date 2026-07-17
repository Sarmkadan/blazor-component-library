# Chart

The `Chart` component renders a configurable chart inside a Blazor application. It exposes properties to define the chart type, title, labels, colors, and additional options, as well as a collection of annotations. The chart can be updated programmatically via the `SetData` and `Refresh` methods, and custom child content can be injected through the `ChildContent` render fragment.

## API

### `ChartType ChartType`
Gets or sets the type of chart to render. The value is a member of the `ChartType` enum (e.g., `Bar`, `Line`, `Pie`).  
**Throws:** `ArgumentOutOfRangeException` if the assigned value is not a valid `ChartType` member.

### `string Title`
Gets or sets the title displayed above the chart.  
**Throws:** `ArgumentNullException` if the value is set to `null`.

### `IEnumerable<string> Labels`
Gets or sets the labels for the chart’s categories or data points.  
**Throws:** `ArgumentNullException` if the value is `null`.

### `IEnumerable<string> Colors`
Gets or sets the color codes (e.g., hex, RGB) used for the chart’s data series or segments.  
**Throws:** `ArgumentNullException` if the value is `null`.

### `object Options`
Gets or sets a free-form object containing additional chart configuration (e.g., scales, tooltips, legend settings). The object is passed directly to the underlying charting library.  
**Throws:** `ArgumentNullException` if the value is `null`.

### `IEnumerable<ChartAnnotation> Annotations`
Gets or sets a collection of `ChartAnnotation` objects that define overlays (e.g., lines, boxes, labels) on the chart.  
**Throws:** `ArgumentNullException` if the value is `null`.

### `void SetData()`
Triggers the component to read its current property values (`Labels`, `Colors`, `Options`, `Annotations`, etc.) and apply them to the underlying chart data model. This method does not automatically re-render the chart; call `Refresh` afterward to update the visual output.  
**Throws:** `InvalidOperationException` if the chart has not yet been rendered (e.g., called before the component’s first render cycle).

### `void Refresh()`
Forces the chart to re-render using the most recent data and configuration. This method should be called after modifying any chart properties or after calling `SetData`.  
**Throws:** `InvalidOperationException` if the chart has not yet been rendered.

### `RenderFragment? ChildContent`
Gets or sets a `RenderFragment` that can be used to inject custom Blazor content inside the chart container (e.g., a loading indicator or overlay). When `null`, no additional content is rendered.

## Usage

### Example 1: Basic bar chart with labels and colors

```csharp
@* In a Blazor component *@
<Chart @ref="myChart"
       ChartType="ChartType.Bar"
       Title="Monthly Sales"
       Labels='new[] { "Jan", "Feb", "Mar" }'
       Colors='new[] { "#4e79a7", "#f28e2b", "#e15759" }' />

@code {
    private Chart myChart;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            myChart.SetData();
            myChart.Refresh();
        }
    }
}
```

### Example 2: Updating chart data and options at runtime

```csharp
<Chart @ref="dynamicChart"
       ChartType="ChartType.Line"
       Title="Dynamic Data"
       Labels="@labels"
       Colors="@colors"
       Options="@options" />

<button @onclick="UpdateData">Update Chart</button>

@code {
    private Chart dynamicChart;
    private List<string> labels = new() { "A", "B", "C" };
    private List<string> colors = new() { "#ff0000" };
    private object options = new { responsive = true };

    private void UpdateData()
    {
        labels.Add("D");
        colors.Add("#00ff00");
        options = new { responsive = true, maintainAspectRatio = false };

        dynamicChart.SetData();
        dynamicChart.Refresh();
    }
}
```

## Notes

- **Edge cases:**  
  - If `Labels` or `Colors` are empty, the chart may render with no data or use default colors, depending on the underlying charting library.  
  - The number of colors should match the number of data series or segments; mismatched counts may cause unexpected visual results.  
  - Setting `Options` to an object that does not conform to the expected schema of the charting library can lead to runtime errors or silent failures.  
  - `Annotations` that reference non‑existent data indices are ignored.

- **Thread safety:**  
  The `Chart` component is not thread‑safe. All property assignments and method calls (`SetData`, `Refresh`) must occur on the Blazor UI thread (typically within component lifecycle methods or event handlers). Concurrent access from multiple threads may cause rendering corruption or exceptions.
