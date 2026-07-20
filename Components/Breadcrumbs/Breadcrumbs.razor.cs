using Microsoft.AspNetCore.Components;

namespace BlazorComponentLibrary.Components.Breadcrumbs;

/// <summary>
/// A breadcrumb navigation component that displays a hierarchy of links.
/// </summary>
public sealed partial class Breadcrumbs : ComponentBase, IBreadcrumbs
{
    /// <summary>
    /// Gets or sets the collection of breadcrumb items to display.
    /// </summary>
    [Parameter]
    public IReadOnlyList<BreadcrumbItem> Items { get; set; } = new List<BreadcrumbItem>();

    /// <summary>
    /// Gets or sets the template for the separator between breadcrumb items.
    /// Defaults to "/".
    /// </summary>
    [Parameter]
    public RenderFragment? SeparatorTemplate { get; set; } = builder => builder.AddContent(0, "/");

    /// <summary>
    /// Gets or sets the CSS class for the breadcrumbs container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the CSS style for the breadcrumbs container.
    /// </summary>
    [Parameter]
    public string? Style { get; set; }
}
