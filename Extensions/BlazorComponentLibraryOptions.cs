namespace BlazorComponentLibrary.Extensions;

/// <summary>
/// Options for configuring the BlazorComponentLibrary services.
/// </summary>
public sealed class BlazorComponentLibraryOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the library should register logging services.
    /// The default is <c>true</c>.
    /// </summary>
    public bool RegisterLogging { get; set; } = true;
}
