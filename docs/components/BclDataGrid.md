# BclDataGrid

A feature-rich data grid component with sorting, filtering, pagination, and optional row virtualisation for large datasets.

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `TableHeader` | `RenderFragment` | — | Column header markup rendered inside `<thead>` |
| `RowTemplate` | `RenderFragment<TItem>` | — | Template for each data row |
| `OnRowClick` | `EventCallback<TItem>` | — | Callback invoked when a row is clicked |
| `IsSortable` | `bool` | `false` | Enables column sort controls |
| `IsFilterable` | `bool` | `false` | Enables a global filter input above the grid |
| `PageSize` | `int` | `10` | Number of rows per page (ignored when `EnableVirtualization` is `true`) |
| `EnableVirtualization` | `bool` | `false` | Renders only visible rows using Blazor's `Virtualize` component — ideal for thousands of rows |

## Methods

| Method | Signature | Description |
|--------|-----------|-------------|
| `SetData` | `void SetData(IEnumerable<TItem>)` | Replaces the grid's data source |
| `Refresh` | `void Refresh()` | Forces a re-render without replacing data |
| `SortBy` | `void SortBy(Func<TItem, object?>, SortDirection)` | Sorts the grid programmatically |

## Basic usage

```razor
<BclDataGrid TItem="Product" PageSize="15" IsSortable="true" @ref="grid">
    <TableHeader>
        <tr>
            <th>Name</th>
            <th>Price</th>
            <th>In Stock</th>
        </tr>
    </TableHeader>
    <RowTemplate Context="item">
        <tr>
            <td>@item.Name</td>
            <td>@item.Price.ToString("C")</td>
            <td>@(item.InStock ? "Yes" : "No")</td>
        </tr>
    </RowTemplate>
</BclDataGrid>

@code {
    private BclDataGrid<Product> grid = default!;

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
            grid.SetData(ProductService.GetAll());
    }
}
```

## Virtualised large dataset

```razor
<BclDataGrid TItem="LogEntry" EnableVirtualization="true" @ref="logGrid">
    <TableHeader>
        <tr><th>Timestamp</th><th>Message</th></tr>
    </TableHeader>
    <RowTemplate Context="entry">
        <tr><td>@entry.Timestamp</td><td>@entry.Message</td></tr>
    </RowTemplate>
</BclDataGrid>
```

## Accessibility

- Wrap `<TableHeader>` content in `<thead>` and row templates in `<tbody>` so screen readers identify header cells correctly.
- Add `scope="col"` to `<th>` elements.
- When sorting is active, set `aria-sort="ascending"` or `aria-sort="descending"` on the active column header.
- The `OnRowClick` handler fires on `Enter`/`Space` as well as mouse click when the row has `tabindex="0"` and `role="row"`.

## Theming

```css
:root {
    --bcl-grid-header-bg:        #f8fafc;
    --bcl-grid-header-text:      #1e293b;
    --bcl-grid-row-bg:           #ffffff;
    --bcl-grid-row-bg-alt:       #f1f5f9;
    --bcl-grid-row-bg-hover:     #e2e8f0;
    --bcl-grid-border:           #cbd5e1;
    --bcl-grid-font-size:        0.875rem;
    --bcl-grid-cell-padding:     0.5rem 0.75rem;
    --bcl-grid-radius:           0.5rem;
}
```
