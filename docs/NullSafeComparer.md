# NullSafeComparer

The `NullSafeComparer` is a utility component within the Blazor Component Library designed to facilitate safe data comparison and rendering operations in scenarios where data sources may contain null values. It provides a robust mechanism for sorting, filtering, and displaying tabular data without triggering `NullReferenceException` errors, while exposing configurable rendering templates and event callbacks to integrate seamlessly into Blazor applications.

## API

### `Compare`
*   **Type:** `public int Compare`
*   **Purpose:** Executes the comparison logic between two items, handling null inputs gracefully to determine sort order.
*   **Parameters:** Implicitly operates on the generic type `TItem` associated with the instance.
*   **Return Value:** Returns an `int` indicating the relative order (-1, 0, or 1).
*   **Throws:** No exceptions are thrown for null inputs due to internal guard clauses; standard comparison exceptions may occur if the underlying type implementation is invalid.

### `TableHeader`
*   **Type:** `public RenderFragment TableHeader`
*   **Purpose:** Defines the markup rendered within the header section of the associated table component.
*   **Parameters:** None (property setter accepts a `RenderFragment`).
*   **Return Value:** Returns the `RenderFragment` to be rendered by the Blazor renderer.
*   **Throws:** Does not throw; renders an empty fragment if null.

### `RowTemplate`
*   **Type:** `public RenderFragment<TItem> RowTemplate`
*   **Purpose:** Specifies the template used to render individual rows, receiving the current item of type `TItem` as context.
*   **Parameters:** Accepts a `RenderFragment<TItem>` delegate.
*   **Return Value:** Returns the rendered fragment for a specific data item.
*   **Throws:** May throw if the template logic itself encounters an error, but the comparer ensures the item passed is handled safely.

### `OnRowClick`
*   **Type:** `public EventCallback<TItem> OnRowClick`
*   **Purpose:** Invokes a callback event when a user clicks on a specific row, passing the clicked item as the argument.
*   **Parameters:** Requires an assigned `EventCallback<TItem>` in the consuming component.
*   **Return Value:** Returns a `Task` upon invocation completion.
*   **Throws:** Does not throw if the callback is unassigned; propagates exceptions thrown within the user-defined callback handler.

### `IsSortable`
*   **Type:** `public bool IsSortable`
*   **Purpose:** Toggles the ability for the associated data grid to perform sorting operations using this comparer.
*   **Parameters:** Boolean flag.
*   **Return Value:** Returns the current state of the sortability flag.
*   **Throws:** Does not throw.

### `IsFilterable`
*   **Type:** `public bool IsFilterable`
*   **Purpose:** Determines whether filtering logic is applied to the data set managed by this component.
*   **Parameters:** Boolean flag.
*   **Return Value:** Returns the current state of the filterability flag.
*   **Throws:** Does not throw.

### `PageSize`
*   **Type:** `public int PageSize`
*   **Purpose:** Sets the number of items to display per page when pagination is active.
*   **Parameters:** Integer value representing the count.
*   **Return Value:** Returns the configured page size.
*   **Throws:** May throw an `ArgumentOutOfRangeException` if set to a non-positive value, depending on internal guard clause strictness.

### `EnableVirtualization`
*   **Type:** `public bool EnableVirtualization`
*   **Purpose:** Enables or disables UI virtualization to optimize performance when rendering large datasets.
*   **Parameters:** Boolean flag.
*   **Return Value:** Returns the virtualization state.
*   **Throws:** Does not throw.

### `SetData`
*   **Type:** `public void SetData`
*   **Purpose:** Initializes or updates the internal data source that the comparer and grid will operate upon.
*   **Parameters:** Accepts the data collection (signature implies method overload or context-based injection).
*   **Return Value:** `void`.
*   **Throws:** Throws `ArgumentNullException` if the provided data collection is null, enforced by guard clauses.

### `Refresh`
*   **Type:** `public void Refresh`
*   **Purpose:** Forces the component to re-evaluate the current data, re-apply sorting/filtering, and trigger a UI re-render.
*   **Parameters:** None.
*   **Return Value:** `void`.
*   **Throws:** Does not throw under normal operation; may throw if the internal state is corrupted.

### `SortBy`
*   **Type:** `public void SortBy`
*   **Purpose:** Explicitly triggers a sort operation based on the current configuration and comparer logic.
*   **Parameters:** May accept sorting criteria depending on overload (context implies configuration via properties).
*   **Return Value:** `void`.
*   **Throws:** Does not throw for null data elements; ensures stable sort execution.

## Usage

### Example 1: Basic Grid Configuration with Null Safety
This example demonstrates initializing the `NullSafeComparer` within a Blazor component to handle a list of products that may contain null entries, enabling sorting and defining a custom row layout.

```csharp
@using BlazorComponentLibrary

<NullSafeComparer TItem="Product" 
                  IsSortable="true" 
                  IsFilterable="true" 
                  PageSize="20"
                  SetData="@_products"
                  SortBy="@( () => SortByColumn() )">
    
    <TableHeader>
        <th>Product Name</th>
        <th>Price</th>
    </TableHeader>

    <RowTemplate Context="product">
        <td>@product?.Name</td>
        <td>@product?.Price.ToString("C")</td>
    </RowTemplate>

    <OnRowClick Callback="HandleRowClick" />

</NullSafeComparer>

@code {
    private List<Product> _products = GetProductData();

    private void SortByColumn()
    {
        // Logic to determine sort key
    }

    private void HandleRowClick(Product item)
    {
        if (item != null)
        {
            Console.WriteLine($"Selected: {item.Name}");
        }
    }
}
```

### Example 2: Virtualized Data with Dynamic Refresh
This example illustrates enabling virtualization for large datasets and programmatically refreshing the view after an external data update.

```csharp
@inject IDataService DataService
@using BlazorComponentLibrary

<NullSafeComparer TItem="Customer" 
                  EnableVirtualization="true" 
                  PageSize="50"
                  IsSortable="true">
    
    <RowTemplate Context="customer">
        <div class="customer-row">
            @customer?.FullName
        </div>
    </RowTemplate>

</NullSafeComparer>

@code {
    private NullSafeComparer<Customer> _comparerRef;

    protected override async Task OnInitializedAsync()
    {
        var data = await DataService.GetCustomersAsync();
        _comparerRef.SetData(data);
    }

    public async Task ReloadDataAsync()
    {
        var freshData = await DataService.GetCustomersAsync();
        _comparerRef.SetData(freshData);
        _comparerRef.Refresh();
    }
}
```

## Notes

*   **Null Handling:** The primary function of this component is to prevent runtime crashes when `TItem` or properties within `TItem` are null. The `Compare` method and rendering pipeline include guard clauses to bypass logic or render empty states rather than throwing `NullReferenceException`.
*   **Thread Safety:** The `SetData`, `Refresh`, and `SortBy` methods are not thread-safe. If data updates originate from background threads (e.g., SignalR callbacks or timer events), ensure invocation occurs on the UI synchronization context using `InvokeAsync`.
*   **Guard Clauses:** Recent updates have strengthened guard clauses within `SetData` and initialization logic. Passing a null collection to `SetData` will result in an immediate exception rather than a silent failure or delayed error during rendering.
*   **Virtualization Constraints:** When `EnableVirtualization` is set to `true`, the `RowTemplate` must render elements with consistent heights to ensure the virtualization scrollbar calculates positions accurately.
*   **Event Callbacks:** The `OnRowClick` callback will not fire if the clicked row corresponds to a null item in the source collection, as the component filters out null references before attaching event handlers.
