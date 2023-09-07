# BclButton

A styled button component supporting multiple variants, sizes, and states.

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Variant` | `string` | `"primary"` | Visual style: `primary`, `secondary`, `danger`, `ghost`, `link` |
| `Size` | `string` | `"md"` | Size: `sm`, `md`, `lg` |
| `Disabled` | `bool` | `false` | Disables interaction and applies disabled styling |
| `Loading` | `bool` | `false` | Shows a spinner and disables the button |
| `Type` | `string` | `"button"` | HTML button type: `button`, `submit`, `reset` |
| `OnClick` | `EventCallback<MouseEventArgs>` | — | Callback invoked on click |
| `ChildContent` | `RenderFragment` | — | Button label/content |

## Basic usage

```razor
<BclButton Variant="primary" OnClick="@HandleClick">Save changes</BclButton>

<BclButton Variant="danger" Size="sm" OnClick="@HandleDelete">Delete</BclButton>

<BclButton Loading="@isSaving" Type="submit">Submit</BclButton>

@code {
    private bool isSaving = false;

    private async Task HandleClick()
    {
        isSaving = true;
        await SaveAsync();
        isSaving = false;
    }
}
```

## Variants example

```razor
<BclButton Variant="primary">Primary</BclButton>
<BclButton Variant="secondary">Secondary</BclButton>
<BclButton Variant="danger">Danger</BclButton>
<BclButton Variant="ghost">Ghost</BclButton>
<BclButton Variant="link">Link</BclButton>
```

## Accessibility

- The component renders a native `<button>` element so keyboard focus and `Enter`/`Space` activation work automatically.
- When `Disabled="true"`, the native `disabled` attribute is set, which removes the element from the tab order and announces its state to screen readers.
- When `Loading="true"`, add an `aria-label` that describes the in-progress action (e.g. `aria-label="Saving…"`) so assistive technology does not just announce "button".
- Ensure sufficient colour contrast between the button label and background (WCAG 2.1 AA requires 4.5:1 for normal text).

## Theming

Override these CSS custom properties to match your design system:

```css
:root {
    --bcl-btn-primary-bg:         #3b82f6;
    --bcl-btn-primary-bg-hover:   #2563eb;
    --bcl-btn-primary-text:       #ffffff;
    --bcl-btn-secondary-bg:       #6b7280;
    --bcl-btn-secondary-bg-hover: #4b5563;
    --bcl-btn-secondary-text:     #ffffff;
    --bcl-btn-danger-bg:          #ef4444;
    --bcl-btn-danger-bg-hover:    #dc2626;
    --bcl-btn-danger-text:        #ffffff;
    --bcl-btn-radius:             0.375rem;
    --bcl-btn-font-size-sm:       0.875rem;
    --bcl-btn-font-size-md:       1rem;
    --bcl-btn-font-size-lg:       1.125rem;
    --bcl-btn-padding-sm:         0.25rem 0.75rem;
    --bcl-btn-padding-md:         0.5rem 1.25rem;
    --bcl-btn-padding-lg:         0.75rem 1.75rem;
}
```
