# ChartAnnotationExtensions

Static helper methods for working with `ChartAnnotation` instances in the Blazor component library. These extensions provide common operations such as validation, cloning, appearance modification, and text retrieval without requiring direct manipulation of the annotation’s properties.

## API

### GetDisplayText
**Purpose:** Returns the formatted display text for an annotation, suitable for rendering in UI elements.  
**Parameters:**  
- `annotation` – The `ChartAnnotation` to evaluate.  
**Return value:** A string containing the display text; returns an empty string if the annotation has no displayable text.  
**Exceptions:**  
- `ArgumentNullException` if `annotation` is `null`.

### IsValid
**Purpose:** Determines whether the annotation’s current state is considered valid for rendering.  
**Parameters:**  
- `annotation` – The `ChartAnnotation` to validate.  
**Return value:** `true` if the annotation is valid; otherwise `false`.  
**Exceptions:**  
- `ArgumentNullException` if `annotation` is `null`.

### Clone
**Purpose:** Creates a deep copy of the supplied annotation.  
**Parameters:**  
- `annotation` – The `ChartAnnotation` to clone.  
**Return value:** A new `ChartAnnotation` instance with property values identical to the source.  
**Exceptions:**  
- `ArgumentNullException` if `annotation` is `null`.  
- `InvalidOperationException` if the annotation contains non‑cloneable resources (e.g., unmanaged handles).

### SetColor
**Purpose:** Assigns a color to the annotation’s visual representation.  
**Parameters:**  
- `annotation` – The `ChartAnnotation` to modify.  
- `color` – A CSS‑compatible color string (e.g., `"#ff0000"` or `"rgba(0,128,255,0.5)"`).  
**Return value:** None.  
**Exceptions:**  
- `ArgumentNullException` if `annotation` is `null`.  
- `ArgumentException` if `color` is not a valid CSS color specification.

### SetTooltip
**Purpose:** Defines the tooltip text that appears when the user hovers over the annotation.  
**Parameters:**  
- `annotation` – The `ChartAnnotation` to modify.  
- `tooltip` – The tooltip string to assign.  
**Return value:** None.  
**Exceptions:**  
- `ArgumentNullException` if `annotation` is `null`.  
- `ArgumentException` if `tooltip` is `null` or consists only of whitespace.

### GetValueText
**Purpose:** Retrieves the textual representation of the annotation’s underlying value.  
**Parameters:**  
- `annotation` – The `ChartAnnotation` to query.  
**Return value:** A string representing the value; returns `null` if the annotation does not expose a value.  
**Exceptions:**  
- `ArgumentNullException` if `annotation` is `null`.

### HasLabel
**Purpose:** Indicates whether the annotation has an associated label element.  
**Parameters:**  
- `annotation` – The `ChartAnnotation` to inspect.  
**Return value:** `true` if a label is present; otherwise `false`.  
**Exceptions:**  
- `ArgumentNullException` if `annotation` is `null`.

## Usage

### Example 1: Clone an annotation and customize its appearance
```csharp
using BlazorComponentLibrary.Charts;

// Assume 'original' is an existing ChartAnnotation instance
ChartAnnotation original = GetSomeAnnotation();

// Create a copy that can be modified independently
ChartAnnotation copy = ChartAnnotationExtensions.Clone(original);

// Change the copy's color and tooltip
ChartAnnotationExtensions.SetColor(copy, "#ff8800");
ChartAnnotationExtensions.SetTooltip(copy, "Sales target");

// Use 'copy' in a chart series
myChart.Annotations.Add(copy);
```

### Example 2: Validate an annotation and retrieve its display text
```csharp
using BlazorComponentLibrary.Charts;

ChartAnnotation ann = GetAnnotationFromUserInput();

if (ChartAnnotationExtensions.IsValid(ann))
{
    string display = ChartAnnotationExtensions.GetDisplayText(ann);
    string value   = ChartAnnotationExtensions.GetValueText(ann);
    bool   hasLbl  = ChartAnnotationExtensions.HasLabel(ann);

    // Render or log the information as needed
    Console.WriteLine($"Display: {display}, Value: {value}, HasLabel: {hasLbl}");
}
else
{
    Console.WriteLine("The annotation is not valid and cannot be rendered.");
}
```

## Notes
- All extension methods throw `ArgumentNullException` when the supplied `annotation` argument is `null`. Callers should ensure the instance is initialized before invoking these helpers.  
- The methods are stateless and do not retain any internal data; therefore they are thread‑safe with respect to the extension logic itself.  
- `GetDisplayText`, `GetValueText`, `IsValid`, and `HasLabel` are pure read‑only operations and can be safely called concurrently on the same annotation instance.  
- `SetColor` and `SetTooltip` mutate the annotation in place. Concurrent calls to these mutating methods on the same instance may result in race conditions; external synchronization is required if multiple threads may modify the same annotation simultaneously.  
- `Clone` produces a new instance that does not share mutable state with the source, making it safe to use across threads after creation.  
- Implementations may rely on internal validation logic; if an annotation contains resources that cannot be cloned (e.g., unmanaged handles), `Clone` will throw `InvalidOperationException`.  
- Color strings are parsed according to CSS standards; invalid formats trigger `ArgumentException`.  
- Tooltip strings are trimmed internally; providing only whitespace results in an `ArgumentException`.
