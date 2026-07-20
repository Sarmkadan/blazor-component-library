using Microsoft.AspNetCore.Components;

namespace BlazorComponentLibrary.Components.Breadcrumbs;

/// <summary>
/// Represents a breadcrumb navigation component that displays a hierarchy of links.
/// </summary>
public interface IBreadcrumbs
{
    /// <summary>Gets or sets the collection of breadcrumb items to display.</summary>
    IReadOnlyList<BreadcrumbItem> Items { get; set; }

    /// <summary>
    /// Gets or sets the template for the separator between breadcrumb items.
    /// Defaults to "/".
    /// </summary>
    RenderFragment? SeparatorTemplate { get; set; }

    /// <summary>
    /// Gets or sets the CSS class for the breadcrumbs container.
    /// </summary>
    string? Class { get; set; }

    /// <summary>
    /// Gets or sets the CSS style for the breadcrumbs container.
    /// </summary>
    string? Style { get; set; }
}
