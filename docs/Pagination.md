# Pagination

A Blazor component that renders a paginated control, allowing users to navigate between pages of content. It supports customizable page ranges, styling, and event-driven page changes.

## API

### `CurrentPage`
- **Purpose**: Gets or sets the currently active page index (1-based).
- **Type**: `int`
- **Default**: `1`
- **Remarks**: Must be between `1` and `TotalPages`. No validation is performed; invalid values may result in unexpected behavior.

### `TotalPages`
- **Purpose**: Gets or sets the total number of pages available.
- **Type**: `int`
- **Default**: `1`
- **Remarks**: Must be a positive integer. If set to a value less than `1`, the component may render inconsistently.

### `SiblingCount`
- **Purpose**: Gets or sets the number of page buttons to display around the current page (excluding ellipses).
- **Type**: `int`
- **Default**: `1`
- **Remarks**: Must be a non-negative integer. Values greater than `MaxVisiblePages - 2` may reduce the effective range.

### `MaxVisiblePages`
- **Purpose**: Gets or sets the maximum number of page buttons to render (including ellipses).
- **Type**: `int`
- **Default**: `7`
- **Remarks**: Must be an odd integer greater than `1`. Even values are treated as the next lower odd number.

### `Class`
- **Purpose**: Gets or sets an optional CSS class string applied to the root element.
- **Type**: `string?`
- **Default**: `null`
- **Remarks**: Applied in addition to any built-in classes. No sanitization is performed.

### `Style`
- **Purpose**: Gets or sets an optional inline style string applied to the root element.
- **Type**: `string?`
- **Default**: `null`
- **Remarks**: Applied in addition to any built-in styles. No sanitization is performed.

### `PageChanged`
- **Purpose**: An `EventCallback<int>` triggered when the active page changes.
- **Type**: `EventCallback<int>`
- **Parameters**: The new page index (1-based).
- **Remarks**: The callback is synchronous and does not block rendering. Ensure handlers are thread-safe if modified concurrently.

## Usage

### Basic Usage
