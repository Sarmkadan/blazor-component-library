# ChartAnnotation
The `ChartAnnotation` type in the `blazor-component-library` project represents a visual annotation that can be added to a chart, providing additional context or information about specific data points. This type allows developers to customize the appearance and behavior of annotations, making it easier to create informative and engaging charts.

## API
The `ChartAnnotation` type has the following public members:
* `Type`: A property of type `ChartAnnotationType` that determines the type of annotation to be displayed.
* `Value`: A property of type `double` that specifies the value at which the annotation should be placed.
* `EndValue`: A nullable property of type `double?` that specifies the end value of the annotation, used for range-based annotations.
* `Label`: A property of type `string` that sets the text label to be displayed with the annotation.
* `Color`: A property of type `string` that sets the color of the annotation.
* `Tooltip`: A property of type `string` that sets the tooltip text to be displayed when the annotation is hovered over.

## Usage
Here are two examples of using the `ChartAnnotation` type:
```csharp
// Example 1: Creating a simple annotation
var annotation = new ChartAnnotation
{
    Type = ChartAnnotationType.Line,
    Value = 50,
    Label = "Target Value",
    Color = "red"
};

// Example 2: Creating a range-based annotation
var rangeAnnotation = new ChartAnnotation
{
    Type = ChartAnnotationType.Range,
    Value = 20,
    EndValue = 80,
    Label = "Acceptable Range",
    Color = "green",
    Tooltip = "Values within this range are considered acceptable"
};
```

## Notes
When using the `ChartAnnotation` type, note that the `EndValue` property is only applicable when the `Type` is set to `ChartAnnotationType.Range`. In all other cases, `EndValue` should be set to `null`. Additionally, the `Color` property should be set to a valid CSS color string to ensure proper rendering. The `ChartAnnotation` type is designed to be thread-safe, but it is recommended to create and configure annotations on the UI thread to avoid any potential issues with chart rendering.
