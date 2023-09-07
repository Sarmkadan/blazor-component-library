# BclToast

A non-blocking notification component that displays brief messages at the edge of the viewport and disappears after a configurable duration.

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Message` | `string` | `""` | Text content of the notification |
| `Title` | `string` | `""` | Optional bold heading above the message |
| `Variant` | `string` | `"info"` | Visual style: `info`, `success`, `warning`, `error` |
| `Duration` | `int` | `4000` | Auto-dismiss delay in milliseconds. `0` disables auto-dismiss |
| `Position` | `string` | `"bottom-right"` | Viewport position: `top-left`, `top-center`, `top-right`, `bottom-left`, `bottom-center`, `bottom-right` |
| `ShowCloseButton` | `bool` | `true` | Renders an explicit close button |
| `OnDismiss` | `EventCallback` | — | Callback invoked when the toast is dismissed |

## Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `Show(string message, string variant)` | `Task` | Displays a toast with the given message and variant |
| `Dismiss()` | `Task` | Programmatically dismisses the toast |

## Basic usage

```razor
<BclToast @ref="toast" Position="top-right" />

<BclButton OnClick="@ShowSuccess">Save</BclButton>

@code {
    private BclToast toast = default!;

    private async Task ShowSuccess()
    {
        await SaveAsync();
        await toast.Show("Record saved successfully.", "success");
    }
}
```

## Variants

```razor
@* Informational *@
await toast.Show("Your session expires in 5 minutes.", "info");

@* Success *@
await toast.Show("Profile updated.", "success");

@* Warning *@
await toast.Show("Unsaved changes will be lost.", "warning");

@* Error *@
await toast.Show("Failed to connect. Please retry.", "error");
```

## Accessibility

- The toast container must have `role="status"` (or `role="alert"` for errors) and `aria-live="polite"` (or `aria-live="assertive"` for critical errors) so screen readers announce new messages without stealing focus.
- Do not rely solely on colour to convey the variant — include an icon or visible text label.
- Auto-dismiss duration should be long enough for users to read the message (WCAG 2.2 SC 2.2.1 recommends allowing users to extend time limits).
- The close button must have an accessible label: `aria-label="Dismiss notification"`.

## Theming

```css
:root {
    --bcl-toast-bg-info:     #eff6ff;
    --bcl-toast-text-info:   #1e40af;
    --bcl-toast-bg-success:  #f0fdf4;
    --bcl-toast-text-success:#166534;
    --bcl-toast-bg-warning:  #fffbeb;
    --bcl-toast-text-warning:#92400e;
    --bcl-toast-bg-error:    #fef2f2;
    --bcl-toast-text-error:  #991b1b;
    --bcl-toast-radius:      0.5rem;
    --bcl-toast-shadow:      0 4px 12px rgba(0, 0, 0, 0.15);
    --bcl-toast-max-width:   24rem;
    --bcl-toast-z-index:     1100;
}
```
