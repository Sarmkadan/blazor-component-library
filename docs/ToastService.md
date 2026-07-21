# ToastService

The `ToastService` is a core utility within the `blazor-component-library` designed to manage the lifecycle and display state of toast notifications across a Blazor application. It acts as a centralized coordinator, allowing components to trigger, dismiss, and control the timing of transient messages without requiring direct references to UI elements. By exposing methods to pause and resume internal timers, it supports user interactions such as hovering over a notification to prevent it from auto-dismissing.

## API

### `public ToastService`
Initializes a new instance of the `ToastService` class. This constructor sets up the internal collections required to track active toasts and manages the subscription lifecycle for state changes. It does not accept parameters and does not throw exceptions under normal operating conditions.

### `public void Show`
Triggers the display of a new toast notification.
*   **Parameters**: Accepts configuration details for the toast, typically including the message content, severity level (e.g., Info, Success, Error), and optional duration settings.
*   **Return Value**: `void`. The method updates the internal state, which propagates changes to subscribed UI components via events.
*   **Exceptions**: May throw an `ArgumentNullException` if the provided message content is null or empty.

### `public void Dismiss`
Removes a specific toast notification from the active list.
*   **Parameters**: Requires a unique identifier (ID) corresponding to the specific toast instance to be removed.
*   **Return Value**: `void`.
*   **Exceptions**: Throws an `ArgumentException` if the provided ID does not match any currently active toast.

### `public void DismissAll`
Immediately clears all active toast notifications from the display.
*   **Parameters**: None.
*   **Return Value**: `void`.
*   **Exceptions**: Does not throw exceptions; if no toasts are active, the method completes silently.

### `public void PauseTimer`
Halts the automatic countdown timer for a specific active toast, preventing it from auto-dismissing.
*   **Parameters**: Requires the unique identifier (ID) of the toast whose timer should be paused.
*   **Return Value**: `void`.
*   **Exceptions**: Throws an `ArgumentException` if the ID is not found among active toasts.

### `public void ResumeTimer`
Resumes the automatic countdown timer for a previously paused toast.
*   **Parameters**: Requires the unique identifier (ID) of the toast whose timer should be resumed.
*   **Return Value**: `void`.
*   **Exceptions**: Throws an `ArgumentException` if the ID is not found or if the toast is not currently in a paused state.

### `public void Dispose`
Releases unmanaged resources and unsubscribes from internal events to prevent memory leaks.
*   **Parameters**: None.
*   **Return Value**: `void`.
*   **Exceptions**: This method is idempotent; calling it multiple times does not throw exceptions. It should be called when the service is no longer needed, typically by the dependency injection container or a owning component implementing `IDisposable`.

## Usage

### Basic Notification Trigger
The following example demonstrates injecting the service into a Blazor component and displaying a success message upon a successful operation.

```csharp
@inject ToastService ToastService

@code {
    private async Task SaveDataAsync()
    {
        try
        {
            await DataService.SaveAsync();
            ToastService.Show("Data saved successfully.", Severity.Success);
        }
        catch (Exception ex)
        {
            ToastService.Show($"Error: {ex.Message}", Severity.Error);
        }
    }
}
```

### Interactive Timer Control
This example illustrates managing the toast timer manually, such as pausing the dismissal countdown when a user hovers over a notification area and resuming it when they leave.

```csharp
@inject ToastService ToastService

@code {
    private string _currentToastId;

    private void ShowPersistentToast()
    {
        // Assume Show returns the ID or sets it via a callback in a real implementation
        _currentToastId = Guid.NewGuid().ToString(); 
        ToastService.Show("Please review this important update.", Severity.Info, duration: 5000);
    }

    private void OnToastMouseEnter()
    {
        if (!string.IsNullOrEmpty(_currentToastId))
        {
            ToastService.PauseTimer(_currentToastId);
        }
    }

    private void OnToastMouseLeave()
    {
        if (!string.IsNullOrEmpty(_currentToastId))
        {
            ToastService.ResumeTimer(_currentToastId);
        }
    }
}
```

## Notes

*   **Thread Safety**: The `ToastService` is not inherently thread-safe. In Blazor Server applications, all interactions with this service must occur on the UI synchronization context. If invoking methods from background threads or tasks, ensure execution is marshaled back to the UI thread using `InvokeAsync`.
*   **Identifier Validity**: Methods requiring a toast ID (`Dismiss`, `PauseTimer`, `ResumeTimer`) rely on the validity of the provided string. Passing an ID for a toast that has already expired or been dismissed will result in an `ArgumentException`. Callers should maintain accurate state tracking of active IDs.
*   **Disposal Lifecycle**: As the service maintains event subscriptions to notify UI components of state changes, failing to call `Dispose` when the service instance is being manually managed (rather than handled by the DI container scope) may result in memory leaks or dangling event references.
*   **Concurrent Modifications**: Calling `DismissAll` while iterating over toast collections in a UI component may cause collection modification exceptions if the component does not handle the change event defensively. UI consumers should snapshot collections before rendering.
