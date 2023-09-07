# Theming & Design Tokens

This guide explains how to customise the visual appearance of the component library using CSS custom properties (design tokens).

---

## How it works

Every component reads its colours, spacing, and typography from CSS custom properties defined on `:root`.  
Override any property in your own stylesheet to reskin the library without touching component source code.

```css
/* app.css — loaded after the library stylesheet */
:root {
    --bcl-color-primary: #7c3aed;   /* purple brand colour */
    --bcl-btn-primary-bg: #7c3aed;
    --bcl-btn-primary-bg-hover: #6d28d9;
}
```

---

## Global design tokens

### Colour palette

| Token | Default | Usage |
|-------|---------|-------|
| `--bcl-color-primary` | `#3b82f6` | Brand accent, focus rings, active states |
| `--bcl-color-primary-hover` | `#2563eb` | Hover variant of primary |
| `--bcl-color-secondary` | `#6b7280` | Secondary actions |
| `--bcl-color-danger` | `#ef4444` | Destructive actions, error states |
| `--bcl-color-success` | `#22c55e` | Positive feedback |
| `--bcl-color-warning` | `#f59e0b` | Caution states |
| `--bcl-color-info` | `#3b82f6` | Informational messages |
| `--bcl-color-surface` | `#ffffff` | Card and panel backgrounds |
| `--bcl-color-bg` | `#f8fafc` | Page background |
| `--bcl-color-text` | `#1e293b` | Primary text |
| `--bcl-color-text-muted` | `#64748b` | Secondary / placeholder text |
| `--bcl-color-border` | `#e2e8f0` | Default border colour |

### Typography

| Token | Default | Usage |
|-------|---------|-------|
| `--bcl-font-family` | `system-ui, -apple-system, sans-serif` | Base font stack |
| `--bcl-font-size-xs` | `0.75rem` | Labels, badges |
| `--bcl-font-size-sm` | `0.875rem` | Secondary text, small buttons |
| `--bcl-font-size-md` | `1rem` | Body text, default inputs |
| `--bcl-font-size-lg` | `1.125rem` | Subheadings |
| `--bcl-font-size-xl` | `1.25rem` | Section headings |
| `--bcl-font-weight-normal` | `400` | Body |
| `--bcl-font-weight-medium` | `500` | Labels, nav links |
| `--bcl-font-weight-bold` | `700` | Headings, emphasis |
| `--bcl-line-height` | `1.5` | Body text |

### Spacing scale

| Token | Default |
|-------|---------|
| `--bcl-space-1` | `0.25rem` (4 px) |
| `--bcl-space-2` | `0.5rem`  (8 px) |
| `--bcl-space-3` | `0.75rem` (12 px) |
| `--bcl-space-4` | `1rem`    (16 px) |
| `--bcl-space-6` | `1.5rem`  (24 px) |
| `--bcl-space-8` | `2rem`    (32 px) |
| `--bcl-space-12` | `3rem`   (48 px) |
| `--bcl-space-16` | `4rem`   (64 px) |

### Border & shadow

| Token | Default | Usage |
|-------|---------|-------|
| `--bcl-radius-sm` | `0.25rem` | Tags, badges |
| `--bcl-radius-md` | `0.375rem` | Inputs, buttons |
| `--bcl-radius-lg` | `0.5rem` | Cards, modals |
| `--bcl-radius-full` | `9999px` | Pills, avatars |
| `--bcl-shadow-sm` | `0 1px 2px rgba(0,0,0,.05)` | Subtle lift |
| `--bcl-shadow-md` | `0 4px 12px rgba(0,0,0,.1)` | Cards, dropdowns |
| `--bcl-shadow-lg` | `0 20px 60px rgba(0,0,0,.2)` | Modals |

---

## Creating a custom theme

1. Create a `theme.css` file in your application.
2. Override the tokens you want to change on `:root`.
3. Import it **after** the library stylesheet in your `index.html` or `App.razor`.

```html
<!-- index.html -->
<link rel="stylesheet" href="_content/BlazorComponentLibrary/blazor-component-library.css" />
<link rel="stylesheet" href="theme.css" />
```

### Example: "Ocean" theme

```css
/* theme-ocean.css */
:root {
    --bcl-color-primary:       #0891b2;
    --bcl-color-primary-hover: #0e7490;
    --bcl-color-secondary:     #475569;
    --bcl-color-surface:       #f0f9ff;
    --bcl-color-bg:            #e0f2fe;
    --bcl-color-text:          #0c4a6e;
    --bcl-color-border:        #bae6fd;

    --bcl-btn-primary-bg:         #0891b2;
    --bcl-btn-primary-bg-hover:   #0e7490;
    --bcl-nav-topbar-bg:          #0c4a6e;
    --bcl-nav-link-active-text:   #38bdf8;
}
```

---

## Dark mode support

The library ships with a built-in dark mode that activates automatically via `prefers-color-scheme: dark`.

```css
@media (prefers-color-scheme: dark) {
    :root {
        --bcl-color-surface:    #1e293b;
        --bcl-color-bg:         #0f172a;
        --bcl-color-text:       #f1f5f9;
        --bcl-color-text-muted: #94a3b8;
        --bcl-color-border:     #334155;

        --bcl-skeleton-bg:      #2d3748;
        --bcl-skeleton-shimmer: rgba(255, 255, 255, 0.1);
    }
}
```

### Forced dark mode (class-based)

If you manage dark mode with a CSS class instead of the media query, override the same tokens inside `.dark`:

```css
.dark {
    --bcl-color-surface: #1e293b;
    --bcl-color-bg:      #0f172a;
    --bcl-color-text:    #f1f5f9;
    /* … */
}
```

Then toggle the class on `<html>` from Blazor:

```csharp
await JSRuntime.InvokeVoidAsync("document.documentElement.classList.toggle", "dark", isDark);
```

---

## Per-component token reference

Each component page lists the tokens it consumes:

- [BclButton tokens](./components/BclButton.md#theming)
- [BclDataGrid tokens](./components/BclDataGrid.md#theming)
- [BclModal tokens](./components/BclModal.md#theming)
- [BclToast tokens](./components/BclToast.md#theming)
- [BclForm tokens](./components/BclForm.md#theming)
- [BclNavigation tokens](./components/BclNavigation.md#theming)

---

## Skeleton placeholder tokens

| Token | Default | Usage |
|-------|---------|-------|
| `--bcl-skeleton-bg` | `#e2e8f0` | Placeholder fill colour |
| `--bcl-skeleton-shimmer` | `rgba(255,255,255,.6)` | Shimmer highlight |
| `--bcl-skeleton-radius` | `4px` | Corner radius |
| `--bcl-skeleton-line-height` | `1em` | Height of a text skeleton line |
| `--bcl-skeleton-line-gap` | `0.5em` | Vertical gap between lines |
