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

## ToastService

The `ToastService` manages the lifecycle of notification alerts within the application, providing programmatic control over displaying and removing toasts. It handles the automatic scheduling of dismissals for temporary alerts, ensuring notifications are cleaned up efficiently.

```csharp
using BlazorComponentLibrary.Services;

// Create a new ToastService instance
var toastService = new ToastService();

// Show a success toast notification
toastService.Show("Task completed successfully!", ToastType.Success, durationMs: 5000);

// Dismiss all active toasts
toastService.DismissAll();

// Dispose of the service to clean up timer resources
toastService.Dispose();
```

---

## ToastServiceTests
The `ToastServiceTests` class provides comprehensive unit tests for `ToastService`, validating its core behavior such as toast creation, event triggering, and dismissal functionality. These tests ensure that the service correctly manages its active toast list and handles edge cases, such as empty messages or unknown IDs.

```csharp
[Fact]
public void Show_AddsToastToActiveList()
{
    var service = new ToastService();
    service.Show("Hello world");
    Assert.Single(service.ActiveToasts);
}

[Fact]
public void DismissAll_ClearsActiveToasts()
{
    var service = new ToastService();
    service.Show("A");
    service.Show("B");
    service.Show("C");

    service.DismissAll();

    Assert.Empty(service.ActiveToasts);
}
```

---

## FormTests

The `FormTests` class validates the `Form` component's functionality, ensuring proper model binding, validation handling, and state management. These tests verify key behaviors such as the form's handling of models with `Name` and `Age` properties, validating models against data annotations, and resetting the validation state when a new model is set.

```csharp
[Fact]
public async Task Validate_InvalidModel_ReturnsFalseAndExposesErrors()
{
    // Arrange: Setup model with invalid properties
    var form = new Form<ValidatedModel>();
    form.SetModel(new ValidatedModel { Name = null, Age = 0 });

    // Act
    var result = await form.Validate();

    // Assert
    Assert.False(result);
    Assert.False(form.IsValid);
    Assert.NotEmpty(form.ValidationErrors);
}
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

## DragDropListTests

The `DragDropListTests` class provides comprehensive unit tests for the `DragDropList<T>.Reorder` functionality. These tests validate correct reordering behavior, ensure immutability of the source list, and verify proper handling of boundary conditions and invalid indices.

```csharp
[Fact]
public void Reorder_MovesItemFromLowerToHigherIndex()
{
    var items = new List<string> { "A", "B", "C", "D" };
    var result = DragDropList<string>.Reorder(items, fromIndex: 0, toIndex: 2);
    Assert.Equal(new[] { "B", "C", "A", "D" }, result);
}

[Fact]
public void Reorder_NegativeFromIndex_ThrowsArgumentOutOfRange()
{
    var items = new List<string> { "A", "B" };
    Assert.Throws<ArgumentOutOfRangeException>(() =>
        DragDropList<string>.Reorder(items, fromIndex: -1, toIndex: 0));
}
```

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


## ChartExtensions


Provides extension methods for chart components that simplify common chart operations such as adding annotations, setting data, and configuring chart properties. These methods offer a fluent API for enhancing charts with threshold lines, event markers, reference bands, and dynamic updates, reducing boilerplate code when working with chart visualizations.




```csharp
@using BlazorComponentLibrary.Components.Chart

// Create a chart with sample data
var chart = new Chart<object>();
var data = new List<double> { 10, 20, 30, 40, 50 };

// Set data and refresh in one operation
chart.SetDataAndRefresh(data);

// Add a threshold line at value 35 with a label
var threshold = chart.AddThresholdLine(35, "Target", color: "#ff0000");

// Add an event marker at position 2.5
var eventMarker = chart.AddEventMarker(2.5, "Launch", color: "#ffcd56");

// Add a reference band from 15 to 45
var referenceBand = chart.AddReferenceBand(15, 45, "Acceptable Range", color: "#4bc0c080");

// Set a custom title
chart.SetTitle("Sales Performance - Q2 2025");

// Change chart type
chart.SetChartType(ChartType.Line);

// Clear all annotations
chart.ClearAnnotations();
```

**Available Methods:**

- `SetDataAndRefresh<TData>(IEnumerable<TData> data)` – Sets the chart data and refreshes the chart in a single operation
- `AddThresholdLine<TData>(double value, string? label = null, string? color = null, string? tooltip = null)` – Adds a threshold line annotation at the specified value
- `AddEventMarker<TData>(double position, string? label = null, string? color = null, string? tooltip = null)` – Adds an event marker annotation at the specified position
- `AddReferenceBand<TData>(double startValue, double endValue, string? label = null, string? color = null, string? tooltip = null)` – Adds a reference band annotation between two values
- `ClearAnnotations<TData>()` – Clears all annotations from the chart
- `SetTitle<TData>(string title)` – Sets the chart title
- `SetChartType<TData>(ChartType chartType)` – Sets the chart type




---


## ChartAnnotationExtensions

Provides extension methods for working with `ChartAnnotation` instances that simplify common annotation operations. These methods offer convenient utilities for creating formatted display text, validating annotations, cloning annotations for modifications, and updating annotation properties like color and tooltip text. This reduces boilerplate code when working with chart annotations programmatically.

```csharp
@using BlazorComponentLibrary.Components.Chart

// Create a chart annotation
var annotation = new ChartAnnotation
{
    Type = ChartAnnotationType.ThresholdLine,
    Value = 75,
    Label = "Target Sales",
    Color = "#ff0000",
    Tooltip = "Goal: $75,000 per quarter"
};

// Get formatted display text for the annotation
string displayText = annotation.GetDisplayText();
// Returns: "Target Sales"

// Check if the annotation is valid for rendering
bool isValid = annotation.IsValid();
// Returns: true

// Clone the annotation to modify without affecting the original
var clonedAnnotation = annotation.Clone();
clonedAnnotation.SetColor("#00ff00"); // Change to green

// Update the annotation's tooltip
annotation.SetTooltip("Updated goal: $80,000 per quarter");

// Get the formatted value text
string valueText = annotation.GetValueText();
// Returns: "75"

// Check if the annotation has a label
bool hasLabel = annotation.HasLabel();
// Returns: true
```

**Available Methods:**

- `GetDisplayText()` – Gets the display text for the annotation based on its type and configuration
- `IsValid()` – Determines whether the annotation has a valid configuration for rendering
- `Clone()` – Creates a deep copy of the annotation to allow modifications without affecting the original
- `SetColor(string color)` – Updates the annotation's color while preserving its other properties
- `SetTooltip(string tooltip)` – Updates the annotation's tooltip text while preserving its other properties
- `GetValueText()` – Gets the annotation's value as a formatted string using invariant culture
- `HasLabel()` – Determines whether the annotation has a label set


---

## ThemeServiceTests

The `ThemeServiceTests` class provides comprehensive unit tests for `ThemeService`, validating its core behavior such as theme initialization, theme switching, and event triggering. These tests ensure that the service correctly manages its current theme state and handles edge cases like null dependencies.

```csharp
// Create a ThemeService instance with a mock IJSRuntime
var jsRuntimeMock = new Mock<IJSRuntime>();
var service = new ThemeService(jsRuntimeMock.Object);

// Verify the default theme is System
Assert.Equal(ThemeMode.System, service.CurrentTheme);

// Set theme to Dark and verify it updates
service.SetTheme(ThemeMode.Dark);
Assert.Equal(ThemeMode.Dark, service.CurrentTheme);

// Subscribe to theme changes and verify event is raised
ThemeMode? raisedMode = null;
service.ThemeChanged += mode => raisedMode = mode;
service.SetTheme(ThemeMode.Light);
Assert.Equal(ThemeMode.Light, raisedMode);

// Verify null IJSRuntime throws ArgumentNullException
Assert.Throws<ArgumentNullException>(() => new ThemeService(null!));
```

---

## NullSafeComparer

A null-safe comparer that implements `IComparer<object?>` and places `null` values after non-null values. This comparer is particularly useful for sorting data tables and collections where some values may be null, ensuring consistent and predictable sorting behavior without throwing `NullReferenceException`.

```csharp
// Create a data table with nullable properties
var products = new List<Product>
{
    new Product { Id = 1, Name = "Laptop", Price = 999.99m },
    new Product { Id = 2, Name = "Mouse" },
    new Product { Id = 3, Name = "Keyboard", Price = 49.99m },
    new Product { Id = 4, Name = "Monitor" }
};

// Use NullSafeComparer with DataTable
var dataTable = new DataTable<Product>();
dataTable.SetData(products);

// Sort by price (nulls will appear last)
dataTable.SortBy(p => p.Price);

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal? Price { get; set; }
}
```

**Usage in DataTable component:**

- `Compare(object? x, object? y)` – Compares two objects, placing nulls after non-null values
- The comparer is automatically used by `DataTable<TItem>.SortBy()` when sorting nullable properties

## NullSafeComparerExtensions

```csharp
@using BlazorComponentLibrary.Components.DataTable

// Sample data with null values
var users = new List<User>
{
    new User { Id = 1, Name = "Alice", Age = 30 },
    new User { Id = 2, Name = "Bob" }, // Age is null
    new User { Id = 3, Name = "Charlie", Age = 25 },
    new User { Id = 4, Name = "Diana" }, // Age is null
    new User { Id = 5, Name = "Eve", Age = 35 }
};

// Sort users by age in ascending order (nulls come first)
var sortedAscending = users.OrderByNullSafe(u => u.Age).ToList();
// Result: [Bob (null), Diana (null), Charlie (25), Alice (30), Eve (35)]

// Sort users by age in descending order (nulls come first)
var sortedDescending = users.OrderByDescendingNullSafe(u => u.Age).ToList();
// Result: [Bob (null), Diana (null), Eve (35), Alice (30), Charlie (25)]

// Find the user with the minimum age (ignoring nulls)
var youngest = users.Min(u => u.Age);
// Returns: 25

// Find the user with the maximum age (ignoring nulls)
var oldest = users.Max(u => u.Age);
// Returns: 35

// Sort by age with explicit direction
var sortedByDirection = users.SortBy(u => u.Age, SortDirection.Ascending).ToList();

// Filter out null ages
var usersWithAges = users.WhereNotNull(u => u.Age).ToList();
// Returns: [Alice, Charlie, Eve]

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? Age { get; set; }
}
```

**Available Methods:**

- `OrderByNullSafe<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)` – Sorts elements in ascending order by the specified key using null-safe comparison
- `OrderByDescendingNullSafe<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)` – Sorts elements in descending order by the specified key using null-safe comparison  
- `Min<TSource>(this IEnumerable<TSource> source)` – Returns the minimum value in a sequence using null-safe comparison (ignores nulls)
- `Max<TSource>(this IEnumerable<TSource> source)` – Returns the maximum value in a sequence using null-safe comparison (ignores nulls)
- `SortBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, SortDirection direction = SortDirection.Ascending)` – Convenience method to sort by direction with null-safe comparison
- `WhereNotNull<TSource>(this IEnumerable<TSource> source)` – Filters out null values from a sequence
- `WhereNotNull<TSource>(this IEnumerable<TSource?> source)` – Filters out null values from a sequence of nullable value types

**SortDirection:**

- `SortDirection.Ascending` – Sort in ascending order (default)
- `SortDirection.Descending` – Sort in descending order

---

## Form

A generic form component for Blazor that provides model binding, validation, and submission handling. The Form component supports two-way data binding with any model type, automatic validation, and customizable submission logic.

```csharp
// Create a form with a model
var form = new Form<Person>();

// Set the model and handle submission
form.SetModel(new Person { Name = "John Doe", Email = "john@example.com" });
form.OnSubmit = model => {
    Console.WriteLine($"Form submitted with: {model.Name}, {model.Email}");
    return Task.FromResult(true);
};

// Validate the form
bool isValid = await form.Validate();

// Access the child content
RenderFragment content = form.ChildContent;
```

**Parameters**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| ChildContent | `RenderFragment?` | `null` | Content rendered inside the form |
| OnSubmit | `EventCallback<TModel>` | — | Callback invoked when the form is submitted |
| SetModel | `void` | — | Sets the model instance for the form |
| Validate | `Task<bool>` | — | Validates the form and returns whether it's valid |

**Generic Type Parameter**

- `TModel` - The model type that the form binds to (must have a parameterless constructor)

---

## ChartAnnotation

Represents a contextual annotation overlaid on a chart, such as a threshold line, event marker, or shaded reference band. Annotations help highlight important data points, thresholds, or ranges to provide additional context to chart viewers.



```csharp
// Create a threshold line annotation at value 75 with a label and custom color
var thresholdAnnotation = new ChartAnnotation
{
    Type = ChartAnnotationType.ThresholdLine,
    Value = 75,
    Label = "Target Sales",
    Color = "#ff0000",
    Tooltip = "Goal: $75,000 per quarter"
};

// Create an event marker annotation at position 2.5 with a label
var eventMarker = new ChartAnnotation
{
    Type = ChartAnnotationType.EventMarker,
    Value = 2.5,
    Label = "Product Launch",
    Color = "#ffcd56",
    Tooltip = "Launched Q2 2025"
};

// Create a reference band annotation between values 50 and 90
var referenceBand = new ChartAnnotation
{
    Type = ChartAnnotationType.ReferenceBand,
    Value = 50,
    EndValue = 90,
    Label = "Acceptable Range",
    Color = "#4bc0c080", // 50% opacity
    Tooltip = "Range: $50K - $90K"
};

// Add annotations to a chart
var chart = new Chart<object>();
chart.Annotations = new List<ChartAnnotation> { thresholdAnnotation, eventMarker, referenceBand };
```

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

## ModalExtensions

Provides extension methods for `Modal` components that simplify common modal operations such as toggling visibility, configuring content, and implementing auto-hide behavior with callbacks. These methods provide a convenient way to manage modal states programmatically, reducing boilerplate code when working with modals in your applications.

```csharp
@using BlazorComponentLibrary.Components.Modal

<Modal @ref="myModal" Title="Settings">
    <p>Modal content</p>
</Modal>

@code {
    private Modal myModal;

    private async Task HandleAction()
    {
        // Toggle visibility
        await myModal.ToggleAsync();

        // Show with auto-hide after 3 seconds
        await myModal.ShowWithAutoHideAsync(3000);

        // Configure modal properties
        myModal.SetCloseOnOverlayClick(true);
    }
}
```

## Modal

A flexible modal dialog component that provides a customizable container for displaying content, titles, and footer actions. The Modal component supports programmatic opening and closing, customizable title, content, and footer sections, and configurable behavior for closing via overlay clicks.

```razor
<Modal @ref="myModal" 
       Title="Confirmation" 
       CloseOnOverlayClick="true" 
       OnClose="HandleClose">
    <ChildContent>
        <p>Are you sure you want to proceed?</p>
    </ChildContent>
    <FooterContent>
        <button @onclick="async () => await myModal.Hide()">Close</button>
        <button class="btn-primary" @onclick="Confirm">Confirm</button>
    </FooterContent>
</Modal>

@code {
    private Modal myModal;

    private async Task OpenModal() => await myModal.Show();
    private void HandleClose() => Console.WriteLine("Modal closed.");
    private async Task Confirm()
    {
        // ... perform confirmation logic
        await myModal.Hide();
    }
}
```

**Parameters**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Title | `string` | `string.Empty` | Title of the modal dialog |
| ChildContent | `RenderFragment?` | `null` | Main content displayed within the modal body |
| FooterContent | `RenderFragment?` | `null` | Footer content for actions (buttons, etc.) |
| OnClose | `EventCallback` | — | Callback invoked when the modal is closed |
| CloseOnOverlayClick | `bool` | `true` | Whether clicking the overlay closes the modal |

**Methods**

- `Task Show()` – Displays the modal dialog.
- `Task Hide()` – Hides the modal dialog and restores focus.


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


## LibraryBenchmarks

`LibraryBenchmarks` is a performance testing class that utilizes BenchmarkDotNet to measure the efficiency of core library operations, including sorting algorithms, `DataTable` data management, and `DragDropList` reordering. It provides baseline comparisons for various approaches and helps ensure optimal performance for critical library features as the codebase evolves.

```csharp
using Benchmarks;
using BenchmarkDotNet.Running;

// Run all library benchmarks
var summary = BenchmarkRunner.Run<LibraryBenchmarks>();

// Example: Using specific benchmark members
var benchmarks = new LibraryBenchmarks();
benchmarks.Setup();

// Sort using null-safe comparer
var sortedData = benchmarks.SortWithNullSafeComparer();

// Sort complex data by Name
var sortedComplex = benchmarks.SortComplexDataByName();
```

---

## License

MIT License. See [LICENSE](LICENSE) for details.
