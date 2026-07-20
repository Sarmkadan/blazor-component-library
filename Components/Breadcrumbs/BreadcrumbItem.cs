namespace BlazorComponentLibrary.Components.Breadcrumbs;

/// <summary>
/// Represents a single item in a breadcrumb trail.
/// </summary>
/// <param name="Text">The display text for the breadcrumb item.</param>
/// <param name="Href">The URL to navigate to when the breadcrumb item is clicked. If null, the item is rendered as plain text.</param>
public record BreadcrumbItem(string Text, string? Href = null);
