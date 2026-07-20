namespace BlazorComponentLibrary.Components.ProgressBar;

/// <summary>
/// Represents a progress bar component with configurable value, maximum value, and display options.
/// </summary>
public interface IProgressBar
{
    /// <summary>Gets or sets the current value of the progress bar.</summary>
    double Value { get; set; }

    /// <summary>Gets or sets the maximum value of the progress bar. Default is 100.</summary>
    double Max { get; set; }

    /// <summary>Gets or sets whether to show the label with percentage. Default is true.</summary>
    bool ShowLabel { get; set; }

    /// <summary>Gets or sets whether the progress bar is in indeterminate mode.</summary>
    bool Indeterminate { get; set; }

    /// <summary>Gets or sets the CSS class for the progress bar.</summary>
    string? Class { get; set; }

    /// <summary>Gets or sets the CSS style for the progress bar.</summary>
    string? Style { get; set; }
}
