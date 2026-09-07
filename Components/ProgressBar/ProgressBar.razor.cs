using Microsoft.AspNetCore.Components;

namespace BlazorComponentLibrary.Components.ProgressBar;

/// <summary>
/// A progress bar component that displays progress with configurable value, max, and display options.
/// </summary>
public sealed partial class ProgressBar : ComponentBase, IProgressBar
{
    /// <summary>
    /// Gets or sets the current value of the progress bar. Values outside the
    /// <c>[0, Max]</c> range are silently clamped when parameters are applied.
    /// </summary>
    [Parameter]
    public double Value { get; set; }

    /// <summary>
    /// Gets or sets the maximum value of the progress bar. Default is 100.
    /// Non-positive or non-finite values are silently reset to the default.
    /// </summary>
    [Parameter]
    public double Max { get; set; } = 100;

    /// <summary>
    /// Gets or sets whether to show the label with percentage. Default is true.
    /// </summary>
    [Parameter]
    public bool ShowLabel { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the progress bar is in indeterminate mode.
    /// When true, the progress bar shows an animated indeterminate state.
    /// </summary>
    [Parameter]
    public bool Indeterminate { get; set; }

    /// <summary>
    /// Gets or sets the CSS class for the progress bar.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the CSS style for the progress bar.
    /// </summary>
    [Parameter]
    public string? Style { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (!double.IsFinite(Max) || Max <= 0)
        {
            Max = 100;
        }

        if (!double.IsFinite(Value))
        {
            Value = 0;
        }

        Value = Math.Clamp(Value, 0, Max);
    }

    private string? GetProgressValue()
    {
        if (Indeterminate)
        {
            return null;
        }

        if (Max <= 0)
        {
            return "0";
        }

        var clampedValue = Value < 0 ? 0 : Value;
        var progress = Math.Min(clampedValue / Max * 100, 100);
        return $"{progress}%";
    }

    private string? GetAriaValueNow()
    {
        if (Indeterminate)
        {
            return null;
        }

        if (Max <= 0)
        {
            return "0";
        }

        var clampedValue = Value < 0 ? 0 : Value;
        return Math.Min(clampedValue, Max).ToString();
    }

    private string? GetAriaValueMax()
    {
        if (Indeterminate)
        {
            return null;
        }

        return Max <= 0 ? "100" : Max.ToString();
    }

    private string GetCssClasses()
    {
        return Class == null
            ? "bcl-progress-bar"
            : $"bcl-progress-bar {Class}";
    }
}
