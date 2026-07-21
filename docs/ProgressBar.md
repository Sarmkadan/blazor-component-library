# ProgressBar

A visual component for displaying the progress of a task, supporting both determinate (percentage-based) and indeterminate (ongoing activity) states.

## API

### `double Value`
Represents the current progress value.  
**Parameters:** Accepts any `double` value.  
**Return Value:** The current progress value.  
**Exceptions:** Throws `ArgumentOutOfRangeException` if set to a value less than 0 or greater than `Max`.

### `double Max`
Defines the maximum value for the progress range.  
**Parameters:** Accepts any `double` value.  
**Return Value:** The maximum value.  
**Exceptions:** Throws `ArgumentOutOfRangeException` if set to a value less than or equal to 0.

### `bool ShowLabel`
Determines whether the numeric label (e.g., "75%") is displayed alongside the progress bar.  
**Parameters:** Accepts a `bool` value.  
**Return Value:** `true` if the label is visible; otherwise, `false`.

### `bool Indeterminate`
Enables an indeterminate state for the progress bar, typically used when the duration or completion percentage is unknown.  
**Parameters:** Accepts a `bool` value.  
**Return Value:** `true` if the progress bar is in indeterminate mode; otherwise, `false`.

### `string? Class`
Specifies additional CSS class names to apply to the progress bar element.  
**Parameters:** Accepts a `string` or `null`.  
**Return Value:** The assigned CSS class string or `null`.

### `string? Style`
Specifies inline CSS styles to apply to the progress bar element.  
**Parameters:** Accepts a `string` or `null`.  
**Return Value:** The assigned style string or `null`.

## Usage

### Determinate Progress Bar
```razor
<ProgressBar Value="75" Max="100" ShowLabel="true" Class="custom-progress" />
```

### Indeterminate Progress Bar
```razor
<ProgressBar Indeterminate="true" ShowLabel="false" Style="height: 20px;" />
```

## Notes

- When `Indeterminate` is `true`, the `Value` and `Max` properties are ignored.
- Setting `Value` beyond `Max` or to a negative number will throw an exception.
- Null values for `Class` or `Style` are permitted and result in no additional styling being applied.
- This component is not thread-safe. All updates to its properties must occur on the UI thread to prevent rendering inconsistencies.
