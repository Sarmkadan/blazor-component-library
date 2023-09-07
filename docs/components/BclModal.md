# BclModal

An accessible dialog component with focus trapping, overlay click-to-close, and focus restoration on dismiss.

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Title` | `string` | `""` | Text displayed in the modal header |
| `ChildContent` | `RenderFragment` | — | Body content of the modal |
| `FooterContent` | `RenderFragment` | — | Optional footer (action buttons, etc.) |
| `OnClose` | `EventCallback` | — | Callback invoked after the modal closes |
| `CloseOnOverlayClick` | `bool` | `true` | When `true`, clicking the backdrop closes the modal |

## Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `Show()` | `Task` | Opens the modal and saves focus origin |
| `Hide()` | `Task` | Closes the modal, invokes `OnClose`, and restores focus |

## Basic usage

```razor
<BclModal Title="Confirm deletion" @ref="confirmModal" OnClose="@HandleClose">
    <ChildContent>
        <p>Are you sure you want to delete this item? This action cannot be undone.</p>
    </ChildContent>
    <FooterContent>
        <BclButton Variant="danger" OnClick="@DeleteItem">Delete</BclButton>
        <BclButton Variant="secondary" OnClick="@(() => confirmModal.Hide())">Cancel</BclButton>
    </FooterContent>
</BclModal>

<BclButton OnClick="@(() => confirmModal.Show())">Open dialog</BclButton>

@code {
    private BclModal confirmModal = default!;

    private async Task DeleteItem()
    {
        await ItemService.DeleteAsync(selectedId);
        await confirmModal.Hide();
    }

    private void HandleClose() => Console.WriteLine("Modal closed");
}
```

## Accessibility

- The modal container should have `role="dialog"` and `aria-modal="true"`.
- Set `aria-labelledby` to the ID of the title element so the dialog name is announced.
- Focus is automatically moved to the first focusable element inside the modal on open.
- Focus is constrained within the modal while it is open (focus trap).
- Pressing `Escape` should call `Hide()` — wire this up in `OnKeyDown` if not already handled.
- On close, focus returns to the element that triggered the modal (WCAG 2.1 SC 2.4.3).

## Theming

```css
:root {
    --bcl-modal-overlay-bg:  rgba(0, 0, 0, 0.5);
    --bcl-modal-bg:          #ffffff;
    --bcl-modal-radius:      0.5rem;
    --bcl-modal-shadow:      0 20px 60px rgba(0, 0, 0, 0.3);
    --bcl-modal-max-width:   32rem;
    --bcl-modal-header-bg:   #f8fafc;
    --bcl-modal-header-text: #1e293b;
    --bcl-modal-footer-bg:   #f8fafc;
    --bcl-modal-padding:     1.5rem;
    --bcl-modal-z-index:     1000;
}
```
