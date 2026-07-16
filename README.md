# Blazor Component Library
A reusable Blazor component library for modern web applications.

![Build](https://github.com/sarmkadan/blazor-component-library/actions/workflows/build.yml/badge.svg)
![License](https://img.shields.io/github/license/sarmkadan/blazor-component-library?label=License&color=blue)
![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)

Provides production-ready UI components with consistent theming, accessibility support, and responsive design out of the box.

## Features

- Blazor WebAssembly compatible components
- Consistent theming and design tokens
- Accessibility-first approach (WCAG 2.1 AA)
- Responsive layout primitives
- Lightweight with minimal dependencies
- **Theme switcher** — three-way Light / Dark / System toggle with local storage persistence
- **Toast notifications** — queued, auto-dismissing alerts with four severity levels
- **Drag-and-drop list** — reorderable lists via the HTML5 Drag and Drop API

## Requirements

- .NET 10.0 SDK or later
- Docker (for containerized deployment)

## Quick Start

### Install via NuGet

```bash
dotnet add package BlazorComponentLibrary
```

### Register in Program.cs

```csharp
builder.Services.AddBlazorComponentLibrary();
```

### Use in a Razor component

```razor
@using BlazorComponentLibrary

<BclButton Variant="ButtonVariant.Primary" OnClick="HandleClick">
    Submit
</BclButton>

<BclDataGrid Items="@items" Striped="true">
    <BclColumn Field="Name" Title="Full Name" Sortable="true" />
    <BclColumn Field="Email" Title="Email Address" />
</BclDataGrid>
```

### Local Development

```bash
git clone https://github.com/sarmkadan/blazor-component-library.git
cd blazor-component-library
dotnet restore
dotnet build
dotnet run
```

### Docker Deployment

```bash
docker-compose up -d
```

The application will be available at `http://localhost:8080`.

## Architecture

```
BlazorComponentLibrary/
  Components/         - one folder per component (razor + code-behind + interface + scoped css)
  Services/           - DI-registered services (theme, toast)
  Extensions/         - ServiceCollection extension methods
  Exceptions/         - library exception hierarchy
```

Design decisions, data flow, and known limitations are documented in
[docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

## Components

### ThemeSwitcher

A three-way toggle (Light / System / Dark) that persists the user's choice in
`localStorage` and applies a `data-bcl-theme` attribute to `<html>` so CSS custom
properties update without a page reload.

```razor
<ThemeSwitcher ShowLabel="true" />
```

**Parameters**

| Parameter  | Type     | Default | Description                                 |
|------------|----------|---------|---------------------------------------------|
| ShowLabel  | `bool`   | `true`  | Render text label beside each icon           |
| CssClass   | `string?`| `null`  | Extra CSS class(es) for the root element     |

**Service** — inject `IThemeService` anywhere to read or change the theme
programmatically and subscribe to the `ThemeChanged` event.

```csharp
@inject IThemeService ThemeService

ThemeService.SetTheme(ThemeMode.Dark);
ThemeService.ThemeChanged += mode => Console.WriteLine($"Theme is now {mode}");
```

---

### ToastContainer / IToastService

Place `<ToastContainer />` once in your root layout. Inject `IToastService`
wherever you need to show a notification.

```razor
@* MainLayout.razor *@
<ToastContainer Position="ToastPosition.BottomRight" MaxVisible="5" />
```

```csharp
@inject IToastService Toast

Toast.Show("File saved.", ToastType.Success);
Toast.Show("Low disk space.", ToastType.Warning, durationMs: 0); // manual dismiss
```

**ToastContainer parameters**

| Parameter  | Type            | Default                       | Description                            |
|------------|-----------------|-------------------------------|----------------------------------------|
| Position   | `ToastPosition` | `BottomRight`                 | Screen corner for the toast stack      |
| MaxVisible | `int`           | `5`                           | Max simultaneously visible toasts      |

**Toast types** — `Info`, `Success`, `Warning`, `Error`

**Positions** — `TopLeft`, `TopCenter`, `TopRight`, `BottomLeft`, `BottomCenter`, `BottomRight`

---

### DragDropList&lt;TItem&gt;

A drag-and-drop reorderable list powered by the HTML5 Drag and Drop API. Fires
`OnOrderChanged` with the updated list after every successful drop.

```razor
<DragDropList TItem="string"
              Items="@_tasks"
              OnOrderChanged="@(updated => _tasks = updated.ToList())"
              Enabled="true">
    <ItemTemplate Context="task">
        <span>@task</span>
    </ItemTemplate>
</DragDropList>
```

**Parameters**

| Parameter       | Type                       | Default | Description                                    |
|-----------------|----------------------------|---------|------------------------------------------------|
| Items           | `IList<TItem>`             | `[]`    | Ordered collection of items to render           |
| ItemTemplate    | `RenderFragment<TItem>`    | —       | Template for each row                           |
| OnOrderChanged  | `EventCallback<IList<TItem>>` | —    | Fires with the reordered list after a drop      |
| Enabled         | `bool`                     | `true`  | Enable/disable drag interaction                 |
| CssClass        | `string?`                  | `null`  | Extra CSS class(es) for the root `<ul>`         |

---

## Examples

Check out the [examples/](examples/) directory for complete, runnable snippets:
- `BasicUsage.cs`: Getting started and simple service usage.
- `AdvancedUsage.cs`: Advanced theme management and component interactions.
- `IntegrationExample.cs`: Wiring up DI in an ASP.NET Core application.

## Documentation

- [Migration Guide](docs/MIGRATION_v2.md) - Upgrading from v1.x to v2.0
- [Changelog](CHANGELOG.md) - Release notes and version history

## Contributing

Contributions are welcome. See [CONTRIBUTING.md](CONTRIBUTING.md) for development setup and guidelines.

## Performance Benchmarks

The library includes comprehensive performance benchmarks using BenchmarkDotNet to ensure optimal performance of critical operations.

### Running Benchmarks

To run all benchmarks:

```bash
cd Benchmarks
dotnet run -c Release
```

To run specific benchmark categories:

```bash
# Run sorting benchmarks only
dotnet run -c Release --filter *Sorting*

# Run DataTable benchmarks only
dotnet run -c Release --filter *DataTable*

# Run DragDropList benchmarks only
dotnet run -c Release --filter *DragDropList*
```

To generate detailed reports with memory diagnostics:

```bash
cd Benchmarks
dotnet run -c Release -- --memory --runtimes net10.0 net90
```

### Benchmark Results

The benchmarks measure:
- **Throughput**: Operations per second for critical methods
- **Memory Allocations**: GC pressure and object allocations
- **Comparison**: Different sorting approaches and reordering strategies


Key benchmarks include:
- Null-safe sorting performance
- Complex data sorting (5000 items)
- Drag-and-drop list reordering
- DataTable operations (set data, sorting)

### Sample Results

| Method                         | Categories   | Mean           | Error        | StdDev       | Gen0    | Allocated |
|------------------------------- |------------- |---------------:|-------------:|-------------:|--------:|----------:|
| DataTableSetData               | DataTable    |     5,190.9 ns |    103.71 ns |    184.34 ns |  0.0610 |     560 B |
| DataTableSortById              | DataTable    |     5,432.3 ns |    108.65 ns |    187.42 ns |  0.0763 |     672 B |
| DataTableSortByName            | DataTable    |     5,270.3 ns |    105.23 ns |    150.92 ns |  0.0763 |     672 B |
| DataTableSortByStatus          | DataTable    |     5,188.4 ns |    102.67 ns |    171.54 ns |  0.0763 |     672 B |
| DragDropListReorderSmall       | DragDropList |       997.2 ns |     19.95 ns |     50.42 ns |  0.9613 |    8056 B |
| DragDropListReorderLarge       | DragDropList |       926.4 ns |     19.77 ns |     57.35 ns |  0.9623 |    8056 B |
| DragDropListReorderFirstToLast | DragDropList |       806.5 ns |     16.86 ns |     49.17 ns |  0.9623 |    8056 B |
| DragDropListReorderLastToFirst | DragDropList |       814.2 ns |     16.27 ns |     36.72 ns |  0.9623 |    8056 B |
| SortWithNullSafeComparer       | Sorting      |   524,552.4 ns | 10,093.35 ns | 10,365.12 ns |  2.9297 |   28336 B |
| SortWithNullChecks             | Sorting      |   383,354.1 ns |  7,411.12 ns |  7,278.71 ns |  3.4180 |   29536 B |
| SortComplexDataById            | Sorting      |   116,048.6 ns |  2,171.63 ns |  3,860.07 ns | 14.1602 |  120336 B |
| SortComplexDataByName          | Sorting      | 3,114,414.1 ns | 61,506.22 ns | 93,926.51 ns | 15.6250 |  140368 B |
| SortComplexDataByStatus        | Sorting      |   794,083.6 ns | 15,480.66 ns | 19,578.08 ns | 16.6016 |  140368 B |


For detailed results, run the benchmarks locally and review the generated report in the `BenchmarkDotNet.Artifacts` directory.


## License

MIT License. See [LICENSE](LICENSE) for details.
