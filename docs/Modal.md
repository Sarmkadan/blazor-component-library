# Modal

The `Modal` component provides a lightweight, customizable dialog overlay for Blazor applications. It supports a title, arbitrary body content, optional footer content, and configurable dismissal behavior. The modal can be shown and hidden programmatically via its `Show` and `Hide` methods, and it raises an `OnClose` callback when the user dismisses it.

## API

### `Title` (string)

The text displayed in the modal’s header bar. If set to `null` or an empty string, the header area is rendered without text. This property can be bound to a two-way binding or set directly.

### `ChildContent` (RenderFragment?)

The Razor content rendered inside the modal’s body. When `null`, the body area is empty. This is typically used to pass arbitrary markup or components.

### `FooterContent` (RenderFragment?)

The Razor content rendered in the modal’s footer section. When `null`, the footer is omitted entirely. Common uses include action buttons (e.g., “Save”, “Cancel”).

### `OnClose` (EventCallback)

A callback invoked when the modal is closed, either by clicking the overlay (if `CloseOnOverlayClick` is `true`), by pressing the Escape key, or by calling `Hide`. The callback receives no arguments. If the callback throws an exception, the modal will still close; the exception is surfaced through Blazor’s error handling.

### `CloseOnOverlayClick` (bool)

When `true`, clicking the semi-transparent overlay behind the modal triggers the `OnClose` callback and hides the modal. The default value is `false`, meaning overlay clicks are ignored.

### `Show` (async Task)

Displays the modal. If the modal is already visible, this method is a no-op (it does not throw and does not re-render the modal). The method returns a `Task` that completes when the modal has been rendered and is visible.

### `Hide` (async Task)

Hides the modal. If the modal is already hidden, this method is a no-op. The `OnClose` callback is **not** invoked when `Hide` is called programmatically. The returned `Task` completes when the modal has been removed from the DOM.

## Usage

### Basic confirmation dialog

```razor
@* In a Blazor component *@
<Modal @ref="confirmModal"
       Title="Confirm Delete"
       CloseOnOverlayClick="false">
    <ChildContent>
        <p>Are you sure you want to delete this item?</p>
    </ChildContent>
    <FooterContent>
        <button class="btn btn-secondary" @onclick="() => confirmModal.Hide()">Cancel</button>
        <button class="btn btn-danger" @onclick="DeleteItem">Delete</button>
    </FooterContent>
</Modal>

<button @onclick="() => confirmModal.Show()">Delete Item</button>

@code {
    private Modal confirmModal;

    private async Task DeleteItem()
    {
        // Perform deletion logic...
        await confirmModal.Hide();
    }
}
```

### Modal with dynamic content and overlay close

```razor
@* Modal that shows details and closes on overlay click *@
<Modal @ref="detailsModal"
       Title="@($"Details: {SelectedItem?.Name}")"
       CloseOnOverlayClick="true"
       OnClose="HandleDetailsClosed">
    <ChildContent>
        @if (SelectedItem != null)
        {
            <dl>
                <dt>ID</dt>
                <dd>@SelectedItem.Id</dd>
                <dt>Description</dt>
                <dd>@SelectedItem.Description</dd>
            </dl>
        }
    </ChildContent>
</Modal>

@code {
    private Modal detailsModal;
    private Item? SelectedItem;

    private async Task ShowDetails(Item item)
    {
        SelectedItem = item;
        await detailsModal.Show();
    }

    private void HandleDetailsClosed()
    {
        SelectedItem = null;
    }
}
```

## Notes

- **Null and empty input**: Setting `Title` to `null` or an empty string safely renders the header without text. `ChildContent` and `FooterContent` can be `null`; the corresponding sections are omitted entirely.
- **Repeated calls**: Calling `Show` on an already visible modal or `Hide` on an already hidden modal is safe and results in a no-op. No exceptions are thrown.
- **Programmatic hide vs. user dismiss**: The `OnClose` callback is only raised when the modal is closed by user interaction (overlay click or Escape key). Calling `Hide` programmatically does **not** trigger `OnClose`.
- **Thread safety**: Blazor components execute on a single synchronization context (the UI thread). `Show` and `Hide` are asynchronous but are designed to be called from event handlers or lifecycle methods. No internal locking is required; concurrent calls from multiple threads are not supported and may lead to undefined behavior.
- **Escape key handling**: The modal automatically listens for the Escape key and closes itself, invoking `OnClose` if `CloseOnOverlayClick` is `true`. This behavior is independent of the `CloseOnOverlayClick` property.
