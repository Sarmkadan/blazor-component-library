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

## ToastContainerExtensions

Provides extension methods for `ToastContainer` that offer convenient shortcuts for common toast notification scenarios. These methods wrap the underlying `IToastService` calls with pre-configured toast types and durations, reducing boilerplate code when working with toast notifications.

```csharp
@inject IToastService Toast

// Show a success toast that auto-dismisses after 4 seconds
<ToastContainer @ref="toastContainer" />

toastContainer.ShowSuccess("Profile updated successfully!");

// Show a warning toast that requires manual dismissal
toastContainer.ShowWarning("Please review the changes before submitting.", durationMs: 0);

// Show an error toast with custom duration
toastContainer.ShowError("Failed to save document.", durationMs: 6000);

// Show an informational toast (manual dismissal only)
toastContainer.ShowInfo("Background task started...");

// Dismiss all active toasts
toastContainer.DismissAll();
```

---

## DragDropList

A drag-and-drop reorderable list component that enables users to reorder items using the HTML5 Drag and Drop API. The component maintains the order of items and fires the `OnOrderChanged` event whenever the list is reordered through drag-and-drop interactions.



```razor
<DragDropList Items="@tasks"
              ItemTemplate="@(task => RenderTask(task))"
              OnOrderChanged="HandleTasksReordered"
              Enabled="true"
              CssClass="my-custom-list">
</DragDropList>

@code {
    private List<string> tasks = new() { "Task 1", "Task 2", "Task 3", "Task 4" };

    private RenderFragment<DragDropList<string>> RenderTask(string task) => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddContent(1, task);
        builder.CloseElement();
    };

    private void HandleTasksReordered(IList<string> reorderedTasks)
    {
        tasks = new List<string>(reorderedTasks);
        StateHasChanged();
    }
}
```

**Parameters**

| Parameter | Type | Default | Description |
|-----------------|----------------------------|---------|------------------------------------------------|
| Items | `IList<TItem>` | `[]` | Ordered collection of items to render |
| ItemTemplate | `RenderFragment<TItem>` | — | Template for each row |
| OnOrderChanged | `EventCallback<IList<TItem>>` | — | Fires with the reordered list after a drop |
| Enabled | `bool` | `true` | Enable/disable drag interaction |
| CssClass | `string?` | `null` | Extra CSS class(es) for the root `<ul>` |

---

## Chart

A flexible chart component for rendering data visualizations in Blazor applications. The Chart component supports multiple chart types (bar, line, pie, etc.) and provides methods to dynamically update data and refresh the visualization. Annotations can be added to highlight specific data points or thresholds.




```razor
<Chart @ref="chartRef"
       ChartType="ChartType.Bar"
       Title="Sales by Quarter"
       Labels="@labels"
       Colors="@colors"
       Options="@chartOptions">
    <ChildContent>
        @foreach (var item in data)
        {
            <ChartDataset Data="@item.Values" />
        }
    </ChildContent>
</Chart>

@code {
private Chart<object> chartRef;
private List<string> labels = new() { "Q1", "Q2", "Q3", "Q4" };
private List<string> colors = new() { "#36a2eb", "#ff6384", "#4bc0c0", "#ffcd56" };
private object chartOptions = new { responsive = true, maintainAspectRatio = false };
private List<ChartData> data = new()
{
    new ChartData { Values = new List<int> { 50, 75, 100, 80 } },
    new ChartData { Values = new List<int> { 30, 60, 85, 95 } }
};

public class ChartData
{
    public List<int> Values { get; set; }
}

private void UpdateChartData()
{
    chartRef.SetData(data);
    chartRef.Refresh();
}
}
```

**Parameters**

| Parameter | Type | Default | Description |
|-----------|----------|---------|-------------|
| ChartType | `ChartType` | — | Type of chart to render (bar, line, pie, etc.) |
| Title | `string` | `string.Empty` | Chart title displayed above the visualization |
| Labels | `IEnumerable<string>` | `Enumerable.Empty<string>()` | Labels for data points |
| Colors | `IEnumerable<string>` | `Enumerable.Empty<string>()` | Colors for data series |
| Options | `object` | `{}` | Chart.js configuration options as an anonymous object |
| Annotations | `IEnumerable<ChartAnnotation>` | `Enumerable.Empty<ChartAnnotation>()` | Annotations overlaid on the chart |
| ChildContent | `RenderFragment?` | `null` | Custom content rendered inside the chart body |

**Methods**

- `SetData(IEnumerable<TData> data)` – Sets the data source for the chart
- `Refresh()` – Refreshes the chart, re-rendering its content




---


## Skeleton

A loading skeleton placeholder component that displays animated placeholders while content is being fetched or rendered. The skeleton provides visual feedback to users during asynchronous operations, improving perceived performance and reducing layout shifts.

```razor
<Skeleton Type="SkeletonType.Text" Width="200px" Height="24px" Lines="3" Animated="true" />
```

**Parameters**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Type | `SkeletonType` | `Text` | Type of skeleton: Text, Circle, or Rectangle |
| Width | `string` | `100%` | Width of the skeleton element |
| Height | `string` | `auto` | Height of the skeleton element |
| Lines | `int` | `3` | Number of text lines rendered when Type is Text. Ignored for Circle and Rectangle types |
| Animated | `bool` | `true` | Apply pulse animation to indicate loading state |

---

## SkeletonExtensions

Provides extension methods for `Skeleton` components that enable fluent configuration through method chaining. These utilities simplify the creation of skeleton placeholders with common patterns like avatars, buttons, and cards, while also allowing fine-grained control over dimensions, line counts, and animation states.



```csharp
@using BlazorComponentLibrary.Components.Skeleton

// Create a text skeleton with 3 lines
var textSkeleton = Skeleton.AsText(3)
    .WithWidth("200px")
    .WithHeight("24px")
    .Animated();

// Create a circular avatar skeleton
var avatarSkeleton = Skeleton.AsAvatar();

// Create a button placeholder skeleton
var buttonSkeleton = Skeleton.AsButton();

// Create a card placeholder skeleton
var cardSkeleton = Skeleton.AsCard();

// Create a custom rectangle skeleton
var customSkeleton = Skeleton.AsRectangle("300px", "150px")
    .WithLines(5)
    .Static();
```

**Available Methods:**

- `AsText(int lines = 3)` – Configures the skeleton as a text placeholder with the specified number of lines
- `AsCircle(string size = "40px")` – Configures the skeleton as a circle with equal width and height
- `AsRectangle(string width = "100%", string height = "auto")` – Configures the skeleton as a rectangle with custom dimensions
- `WithWidth(string width)` – Sets the width of the skeleton
- `WithHeight(string height)` – Sets the height of the skeleton
- `WithLines(int lines)` – Sets the number of lines for text skeleton types
- `WithAnimation(bool animated)` – Enables or disables the animation effect
- `Animated()` – Sets the skeleton to be animated
- `Static()` – Sets the skeleton to be static (no animation)
- `AsAvatar()` – Configures the skeleton as a circular avatar (40px × 40px)
- `AsButton()` – Configures the skeleton as a standard button placeholder (120px × 40px)
- `AsCard()` – Configures the skeleton as a standard card placeholder (100% width × 200px height)

---

## DragDropListExtensions

Provides extension methods for `DragDropList<TItem>` that offer convenient utilities for manipulating and querying drag-and-drop lists programmatically. These methods simplify common operations like moving items to specific positions, swapping items, and checking list contents without manual index management.


```csharp
@using BlazorComponentLibrary.Components.DragDropList

// Sample list of tasks
var tasks = new List<string> { "Task 1", "Task 2", "Task 3", "Task 4" };

var dragDropList = new DragDropList<string> { Items = tasks };

// Move "Task 3" to the beginning of the list
dragDropList.MoveToBeginning("Task 3");

// Move "Task 1" to the end of the list
dragDropList.MoveToEnd("Task 1");

// Swap the first and last items
dragDropList.SwapItems(0, dragDropList.Count() - 1);

// Get the index of a specific item
var taskIndex = dragDropList.IndexOf("Task 2");

// Check if an item exists in the list
bool hasTask = dragDropList.Contains("Task 4");

// Get the total number of items
int totalItems = dragDropList.Count();

// Get a read-only view of the items
var readOnlyItems = dragDropList.AsReadOnly();
```

**Available Methods:**

- `MoveItem<TItem>(TItem item, int fromIndex, int toIndex)` – Moves an item from one index to another
- `MoveToBeginning<TItem>(TItem item)` – Moves an item to the beginning of the list
- `MoveToEnd<TItem>(TItem item)` – Moves an item to the end of the list
- `SwapItems<TItem>(int index1, int index2)` – Swaps two items by their indices
- `IndexOf<TItem>(TItem item)` – Gets the current index of a specific item
- `Contains<TItem>(TItem item)` – Determines whether the list contains a specific item
- `Count<TItem>()` – Gets the number of items in the list
- `AsReadOnly<TItem>()` – Gets a read-only view of the items in the list


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
