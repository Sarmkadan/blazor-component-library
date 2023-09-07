# Migration Guide: v0.x to v1.0

## Overview

Version 1.0 is the first stable release of BlazorComponentLibrary. It introduces a
cleaner, consistent API across all components. Several parameter names were updated,
a new `CascadingThemeProvider` wrapper is required, and the legacy `FormField`
component was replaced by the new `InputGroup` API.

This guide lists every breaking change with before/after examples so you can upgrade
with confidence.

---

## Breaking Changes

### 1. Required `CascadingThemeProvider` wrapper

All components now read theme tokens from a cascading value. Without this wrapper
the components render but produce a warning and fall back to default styles.

**v0.x** — no wrapper needed:
```razor
<DataTable .../>
```

**v1.0** — wrap your app (or page) in `CascadingThemeProvider`:
```razor
<CascadingThemeProvider>
    <DataTable .../>
</CascadingThemeProvider>
```

The provider is typically placed in `App.razor` or `MainLayout.razor` so it covers
the entire application.

---

### 2. Renamed parameters — DataTable

| v0.x parameter | v1.0 parameter | Notes |
|---|---|---|
| `Columns` | `TableHeader` | Now a `RenderFragment` for full header flexibility |
| `Items` | _(use `SetData()`)_ | Data is set imperatively via the `SetData` method |
| `RowsPerPage` | `PageSize` | Renamed for consistency |
| `Clickable` | `OnRowClick` | Changed from `bool` flag to `EventCallback<TItem>` |

**v0.x:**
```razor
<DataTable Items="@myList" RowsPerPage="20" Clickable="true" />
```

**v1.0:**
```razor
<DataTable @ref="table" PageSize="20" OnRowClick="HandleRowClick">
    <TableHeader>
        <th>Name</th>
        <th>Date</th>
    </TableHeader>
    <RowTemplate Context="item">
        <td>@item.Name</td>
        <td>@item.Date</td>
    </RowTemplate>
</DataTable>

@code {
    private DataTable<MyItem> table;

    protected override void OnInitialized()
        => table.SetData(myList);

    private void HandleRowClick(MyItem item) { ... }
}
```

---

### 3. Renamed parameters — Modal

| v0.x parameter | v1.0 parameter | Notes |
|---|---|---|
| `Header` | `Title` | Accepts a plain string |
| `Body` | `ChildContent` | Standard Blazor child content pattern |
| `Footer` | `FooterContent` | Nullable; omit to hide footer |
| `OnDismiss` | `OnClose` | Renamed for clarity |
| `DismissOnBackdrop` | `CloseOnOverlayClick` | Renamed for clarity |

**v0.x:**
```razor
<Modal Header="Confirm" OnDismiss="Cancel" DismissOnBackdrop="false">
    <Body>Are you sure?</Body>
</Modal>
```

**v1.0:**
```razor
<Modal @ref="modal" Title="Confirm" OnClose="Cancel" CloseOnOverlayClick="false">
    Are you sure?
    <FooterContent>
        <button @onclick="Cancel">Cancel</button>
        <button @onclick="Confirm">OK</button>
    </FooterContent>
</Modal>
```

---

### 4. Renamed parameters — Chart

| v0.x parameter | v1.0 parameter | Notes |
|---|---|---|
| `Type` | `ChartType` | Renamed to avoid conflict with C# `Type` |
| `DataPoints` | _(use `SetData()`)_ | Data is set imperatively via `SetData` |
| `SeriesColors` | `Colors` | Shortened name |
| `SeriesLabels` | `Labels` | Shortened name |

**v0.x:**
```razor
<Chart Type="bar" DataPoints="@points" SeriesColors="@colors" SeriesLabels="@labels" />
```

**v1.0:**
```razor
<Chart @ref="chart" ChartType="ChartType.Bar" Labels="@labels" Colors="@colors" />

@code {
    private Chart<DataPoint> chart;

    protected override void OnInitialized()
        => chart.SetData(points);
}
```

---

### 5. `FormField` removed — replaced by `InputGroup`

The `FormField` component was removed. Use `InputGroup` from the new `Form`
component instead.

**v0.x:**
```razor
<FormField Label="Email" Placeholder="you@example.com" @bind-Value="email" />
```

**v1.0:**
```razor
<Form Model="@model" OnValidSubmit="Submit">
    <InputGroup Label="Email">
        <InputText @bind-Value="model.Email" placeholder="you@example.com" />
    </InputGroup>
</Form>
```

The `Form` component integrates with Blazor's `EditContext`, giving you built-in
validation support that `FormField` lacked.

---

## Step-by-Step Upgrade Checklist

1. **Add `CascadingThemeProvider`** to `App.razor` or `MainLayout.razor`.
2. **Replace `FormField`** usages with `InputGroup` inside a `<Form>` component.
3. **Update DataTable** — rename parameters per the table above and switch from
   property binding to `SetData()`.
4. **Update Modal** — rename parameters per the table above.
5. **Update Chart** — rename `Type` → `ChartType`, `DataPoints` → `SetData()`,
   and the series parameters.
6. Build the project (`dotnet build`) and resolve any remaining CS errors.

---

## Deprecated APIs

The following APIs were deprecated in v1.0 and will be removed in v2.0:

| Deprecated | Replacement |
|---|---|
| `DataTable.Refresh()` with no data argument | Call `SetData()` with fresh data, then the component auto-refreshes |
| `Chart.Refresh()` with no data argument | Same as above |

---

## Getting Help

- See `CHANGELOG.md` for the full list of changes per release.
- See `docs/MIGRATION_v2.md` for upgrading from v1.x to v2.0.
- Open an issue on the repository if you encounter problems not covered here.
