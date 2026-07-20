# Pagination Component

A pagination component that displays page numbers with ellipsis logic and supports keyboard navigation.

## Features

- **Ellipsis Logic**: Shows first and last pages with ellipsis for large page ranges
- **Keyboard Accessible**: Supports arrow keys, Home, and End navigation
- **Event Callbacks**: `PageChanged` event for handling page navigation
- **Accessibility**: Proper ARIA labels and aria-current attributes
- **Customizable**: CSS classes and styles can be applied
- **Responsive**: Uses CSS variables for theming

## Usage

```razor
<Pagination 
    TotalPages="10" 
    CurrentPage="@currentPage" 
    PageChanged="@(page => currentPage = page)" />
```

## Parameters


| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `CurrentPage` | `int` | `1` | The current page number (1-based) |
| `TotalPages` | `int` | `1` | The total number of pages |
| `SiblingCount` | `int` | `1` | Number of sibling pages to show around current page |
| `MaxVisiblePages` | `int` | `7` | Maximum number of pages to display |
| `Class` | `string` | `null` | CSS class for the pagination container |
| `Style` | `string` | `null` | Inline style for the pagination container |
| `PageChanged` | `EventCallback<int>` | `null` | Event callback when page changes |

## Example

```razor
@page "/pagination-example"

<h3>Pagination Example</h3>

<Pagination 
    TotalPages="20" 
    CurrentPage="@currentPage"
    SiblingCount="2"
    PageChanged="HandlePageChanged" />

@code {
    private int currentPage = 1;

    private void HandlePageChanged(int page)
    {
        currentPage = page;
        // Load data for the selected page
    }
}
```

## Styling

The component uses CSS variables for easy theming. You can override these in your own CSS:

```css
:root {
    --bcl-pagination-bg: transparent;
    --bcl-pagination-color: currentColor;
    --bcl-pagination-hover-bg: rgba(0, 0, 0, 0.05);
    --bcl-pagination-hover-color: currentColor;
    --bcl-pagination-focus-outline: currentColor;
    --bcl-pagination-active-bg: currentColor;
    --bcl-pagination-active-color: #fff;
    --bcl-pagination-active-border: currentColor;
    --bcl-pagination-ellipsis-color: currentColor;
}
```

## Keyboard Navigation

- **Left Arrow**: Move to previous page
- **Right Arrow**: Move to next page
- **Home**: Move to first page
- **End**: Move to last page

## Browser Support

The component works in all modern browsers and follows Blazor's browser support policy.
