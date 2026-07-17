# ModalExtensions

`ModalExtensions` provides static convenience methods for controlling modal dialogs within the `blazor-component-library`. It abstracts the underlying modal service, allowing imperative show, hide, and toggle operations, as well as advanced scenarios such as temporary title display, auto-hiding, and post-hide callbacks. The class also exposes static properties for checking visibility and setting modal content and behavior declaratively.

## API

### ShowAsync

```csharp
public static async Task ShowAsync()
```

Displays the modal. If the modal is already visible, calling this method has no effect. This method is asynchronous and completes once the modal has been fully rendered.

- **Parameters**: None.
- **Returns**: A `Task` representing the asynchronous operation.
- **Throws**: `InvalidOperationException` if the modal component has not been initialized before calling this method.

---

### HideAsync

```csharp
public static async Task HideAsync()
```

Hides the modal. If the modal is not currently visible, calling this method has no effect. The method completes once the hide transition has finished.

- **Parameters**: None.
- **Returns**: A `Task` representing the asynchronous operation.
- **Throws**: `InvalidOperationException` if the modal component has not been initialized.

---

### ToggleAsync

```csharp
public static async Task ToggleAsync()
```

Toggles the modal’s visibility. If the modal is visible, it hides it; if hidden, it shows it. The method completes once the transition has finished.

- **Parameters**: None.
- **Returns**: A `Task` representing the asynchronous operation.
- **Throws**: `InvalidOperationException` if the modal component has not been initialized.

---

### ShowWithTemporaryTitleAsync

```csharp
public static async Task ShowWithTemporaryTitleAsync(string title, TimeSpan duration)
```

Shows the modal with a temporary title that reverts to the original title after the specified duration. This is useful for displaying transient status messages or confirmations.

- **Parameters**:
  - `title` (`string`): The temporary title to display while the modal is shown.
  - `duration` (`TimeSpan`): The length of time the temporary title remains before reverting.
- **Returns**: A `Task` representing the asynchronous operation.
- **Throws**:
  - `ArgumentNullException` if `title` is `null`.
  - `InvalidOperationException` if the modal component has not been initialized.

---

### ShowWithAutoHideAsync

```csharp
public static async Task ShowWithAutoHideAsync(TimeSpan autoHideDelay)
```

Shows the modal and automatically hides it after the specified delay. This is intended for brief notifications or confirmation dialogs that do not require user interaction to dismiss.

- **Parameters**:
  - `autoHideDelay` (`TimeSpan`): The delay after which the modal will be automatically hidden.
- **Returns**: A `Task` representing the asynchronous operation.
- **Throws**: `InvalidOperationException` if the modal component has not been initialized.

---

### ShowWithAutoHideAndCallbackAsync

```csharp
public static async Task ShowWithAutoHideAndCallbackAsync(TimeSpan autoHideDelay, Func<Task> onHidden)
```

Shows the modal, automatically hides it after the specified delay, and invokes a callback once the hide transition completes. This allows chaining logic that must execute after the modal is dismissed.

- **Parameters**:
  - `autoHideDelay` (`TimeSpan`): The delay after which the modal will be automatically hidden.
  - `onHidden` (`Func<Task>`): An asynchronous callback invoked after the modal has been hidden.
- **Returns**: A `Task` representing the asynchronous operation.
- **Throws**:
  - `ArgumentNullException` if `onHidden` is `null`.
  - `InvalidOperationException` if the modal component has not been initialized.

---

### IsVisible

```csharp
public static bool IsVisible { get; }
```

Gets a value indicating whether the modal is currently visible. This property reflects the modal’s state at the time of access and does not trigger any side effects.

- **Value**: `true` if the modal is visible; otherwise, `false`.
- **Thread Safety**: Reading this property from a non-UI thread may return a stale value; it is intended for use on the synchronization context that owns the modal.

---

### SetFooterContent

```csharp
public static void SetFooterContent(RenderFragment content)
```

Sets the footer content of the modal. This allows dynamic replacement of the footer area with custom markup or components.

- **Parameters**:
  - `content` (`RenderFragment`): The content to render in the modal’s footer. Pass `null` to clear the footer.
- **Returns**: Nothing.
- **Throws**: `InvalidOperationException` if the modal component has not been initialized.

---

### SetChildContent

```csharp
public static void SetChildContent(RenderFragment content)
```

Sets the main body content of the modal. This replaces the entire child content area with the provided render fragment.

- **Parameters**:
  - `content` (`RenderFragment`): The content to render in the modal’s body. Pass `null` to clear the body.
- **Returns**: Nothing.
- **Throws**: `InvalidOperationException` if the modal component has not been initialized.

---

### SetCloseOnOverlayClick

```csharp
public static void SetCloseOnOverlayClick(bool enabled)
```

Configures whether clicking the modal’s backdrop overlay dismisses the modal. When enabled, any click outside the modal content area triggers a hide operation.

- **Parameters**:
  - `enabled` (`bool`): `true` to allow closing on overlay click; `false` to disable this behavior.
- **Returns**: Nothing.
- **Throws**: `InvalidOperationException` if the modal component has not been initialized.

## Usage

### Example 1: Basic Show and Hide with Footer Content

```csharp
using BlazorComponentLibrary;

// Set up the modal content before showing
ModalExtensions.SetChildContent(builder =>
{
    builder.AddContent(0, "Operation completed successfully.");
});

ModalExtensions.SetFooterContent(builder =>
{
    builder.OpenComponent<Button>(0);
    builder.AddAttribute(1, "Text", "OK");
    builder.AddAttribute(2, "OnClick", EventCallback.Factory.Create(this, async () =>
    {
        await ModalExtensions.HideAsync();
    }));
    builder.CloseComponent();
});

// Show the modal
await ModalExtensions.ShowAsync();

// Later, check visibility
if (ModalExtensions.IsVisible)
{
    // Perform additional logic while the modal is open
}
```

### Example 2: Auto-Hide with Callback and Temporary Title

```csharp
using Blazor.Services;

// Define a callback to execute after the modal hides
Func<Task> onHidden = async () =>
{
    await Task.Delay(100); // Simulate cleanup work
    Console.WriteLine("Modal dismissed, cleanup complete.");
};

// Show the modal with a temporary title, auto-hide after 3 seconds,
// and invoke the callback once hidden
await ModalExtensions.ShowWithAutoHideAndCallbackAsync(
    autoHideDelay: TimeSpan.FromSeconds(3),
    callback: onHidden
);

// Meanwhile, the title "Processing..." is shown for 2 seconds
await ModalExtensions.ShowWithTemporaryTitleAsync("Processing...", TimeSpan.FromSeconds(2));
```

## Notes

- **Initialization Requirement**: All methods and setters throw `InvalidOperationException` if the underlying modal component has not been initialized. Ensure the modal is registered in the dependency injection container and rendered at least once before invoking any member.
- **Thread Safety**: Members are designed to be called from the UI thread (the synchronization context that owns the modal). Calling `IsVisible` from a background thread may yield a stale value. `ShowAsync`, `HideAsync`, and `ToggleAsync` marshal to the UI context internally, but concurrent rapid calls may result in unexpected intermediate states.
- **Temporary Title Reversion**: `ShowWithTemporaryTitleAsync` starts a timer that reverts the title after the specified duration. If the modal is hidden before the duration elapses, the original title is restored immediately upon the next show.
- **Auto-Hide Overlap**: Calling `ShowWithAutoHideAsync` or `ShowWithAutoHideAndCallbackAsync` while a previous auto-hide timer is active cancels the previous timer and starts a new one. The callback from the earlier invocation will not fire.
- **Content Persistence**: `SetFooterContent` and `SetChildContent` modify the modal’s content immediately, even if the modal is currently visible. The UI updates on the next render cycle. Passing `null` clears the respective area.
- **Overlay Click Behavior**: `SetCloseOnOverlayClick` takes effect immediately. If the modal is currently visible and the setting is changed to `true`, subsequent overlay clicks will close it. Changing it to `false` prevents dismissal even if the overlay is clicked.
