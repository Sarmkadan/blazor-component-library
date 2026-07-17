# DragDropList

A reusable Blazor component that renders an ordered list whose items can be reordered at runtime via HTML5 drag-and-drop gestures. The component maintains the item order internally and raises an event containing the fully reordered list so parent components can persist the new sequence without computing a diff.

## API

### `IList<TItem> Items`

Gets or sets the ordered collection of items to display. This list is mutated by the component when items are reordered via drag-and-drop. The collection should be initialized before rendering; otherwise the component will display an empty list.

- **Type**: `IList<TItem>`
- **Default**: `new List<TItem>()`
- **Mutated**: Yes, the internal list reference is replaced when items are reordered

### `RenderFragment<TItem> ItemTemplate`

Gets or sets the render template used to render each item in the list. The template receives the item instance and should return the markup to display for that item.

- **Type**: `RenderFragment<TItem>`
- **Required**: Yes, must be provided or the component will throw a null reference exception
- **Usage**: `<ItemTemplate Context="item">@item</ItemTemplate>`

### `EventCallback<IList<TItem>> OnOrderChanged`

Event raised after the user drops an item at a new position. The callback argument contains the fully reordered list reflecting the new sequence. Parent components should handle this event to persist the new order to their data store.

- **Type**: `EventCallback<IList<TItem>>`
- **Default**: Empty callback (no-op)
- **Fires**: Only when `Enabled` is true and a reordering operation completes successfully
- **Async**: Yes, the callback is awaited by the component

### `bool Enabled`

Gets or sets whether drag-and-drop reordering is currently enabled. When set to false the drag handle is still rendered but dragging has no effect, giving a disabled visual appearance while preserving the rendered markup.

- **Type**: `bool`
- **Default**: `true`
- **Behavior**: When false, all drag operations are ignored and the visual drag state is suppressed

### `string? CssClass`

Optional CSS class(es) appended to the root `<ul>` element. Use this to apply custom styling to the list container without overriding the component's default classes.

- **Type**: `string?`
- **Default**: `null`
- **Rendered**: The root element always receives the base class `bcl-dnd-list`; additional classes are appended with a space separator

### `static List<TItem> Reorder(IList<TItem> source, int fromIndex, int toIndex)`

Returns a new list with the item originally at `fromIndex` moved to `toIndex`. The source list is not mutated. This method is exposed publicly so parent components can compute the same reordering operation independently if needed.

- **Parameters**:
  - `source`: The source list to reorder. Must not be null.
  - `fromIndex`: Zero-based index of the item to move. Must be within the bounds of `source`.
  - `toIndex`: Zero-based target index after the move. Must be within the bounds of `source`.
- **Returns**: A new `List<TItem>` reflecting the reordered items
- **Exceptions**:
  - `ArgumentNullException`: Thrown when `source` is null
  - `ArgumentOutOfRangeException`: Thrown when `fromIndex` or `toIndex` is outside the bounds of `source`
- **Thread-safety**: Safe for concurrent reads; not safe for concurrent writes to the same source list

## Usage

### Basic Usage with In-Memory List

```csharp
@page "/basic-drag-drop"

<DragDropList Items="items" ItemTemplate="ItemTemplate"
              OnOrderChanged="HandleOrderChanged" />

@code {
    private List<string> items = new() { "Apple", "Banana", "Cherry", "Date" };

    private RenderFragment<string> ItemTemplate => (item) => builder =>
    {
        builder.OpenElement(0, "li");
        builder.AddContent(1, item);
        builder.CloseElement();
    };

    private async Task HandleOrderChanged(IList<string> newOrder)
    {
        items = new List<string>(newOrder);
        await InvokeAsync(StateHasChanged);
    }
}
```

### Usage with Complex Objects and Persistence

```csharp
@page "/priority-tasks"

<DragDropList Items="tasks" ItemTemplate="TaskTemplate"
              OnOrderChanged="SaveTaskOrder" Enabled="canReorder" />

@code {
    private List<TaskItem> tasks = new();
    private bool canReorder = true;

    protected override async Task OnInitializedAsync()
    {
        tasks = await TaskService.LoadTasksAsync();
    }

    private RenderFragment<TaskItem> TaskTemplate => (task) => builder =>
    {
        builder.OpenElement(0, "li");
        builder.AddAttribute(1, "draggable", true);
        builder.AddContent(2, $"{task.Priority}: {task.Title}");
        builder.CloseElement();
    };

    private async Task SaveTaskOrder(IList<TaskItem> newOrder)
    {
        tasks = new List<TaskItem>(newOrder);
        await TaskService.SaveTasksAsync(tasks);
    }

    private void ToggleReordering()
    {
        canReorder = !canReorder;
    }
}

public record TaskItem(string Title, int Priority);
```

## Notes

### Edge Cases
- **Empty List**: When `Items` is empty or null, the component renders an empty `<ul>` element without errors.
- **Single Item**: Dragging a single item onto itself has no effect; the `OnOrderChanged` event does not fire.
- **Identical Indices**: When `fromIndex` equals `toIndex` in the static `Reorder` method, the returned list is identical to the source list.
- **Disabled State**: When `Enabled` is false, the drag handle remains in the DOM but drag operations are ignored; the visual drag state is suppressed entirely.

### Thread Safety
- The component is **not thread-safe** for concurrent modifications to the `Items` collection. Parent components must ensure exclusive access when both the component and another thread may modify the list.
- The static `Reorder` method is safe for concurrent reads but not for concurrent writes to the same source list.
- Event callbacks (`OnOrderChanged`) are invoked asynchronously and should not block the UI thread; heavy persistence logic should be offloaded to avoid UI lag.

### Performance
- The component creates a new list instance on every drop operation, so frequent reordering may cause garbage collection pressure in long-running applications.
- The `Reorder` method performs O(n) operations (remove + insert) which is optimal for list reordering scenarios.

### Accessibility
- The component uses standard HTML5 drag-and-drop attributes which are accessible via keyboard in most modern browsers, though explicit keyboard handling would require additional ARIA attributes not provided by default.