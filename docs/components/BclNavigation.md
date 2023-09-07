# BclNavigation

A responsive navigation component supporting top-bar, sidebar, and breadcrumb layouts.

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Variant` | `string` | `"topbar"` | Layout: `topbar`, `sidebar`, `breadcrumb` |
| `Items` | `IEnumerable<NavItem>` | `[]` | Navigation link definitions |
| `ActiveHref` | `string` | `""` | Href of the currently active link; used for `aria-current="page"` |
| `Collapsed` | `bool` | `false` | Collapses the sidebar to icon-only mode (sidebar variant) |
| `OnItemClick` | `EventCallback<NavItem>` | — | Callback invoked when a nav item is clicked |
| `LogoContent` | `RenderFragment` | — | Optional logo/brand slot (topbar variant) |
| `FooterContent` | `RenderFragment` | — | Optional footer slot (sidebar variant) |

### NavItem model

| Property | Type | Description |
|----------|------|-------------|
| `Label` | `string` | Display text |
| `Href` | `string` | Navigation URL |
| `Icon` | `string` | Optional CSS icon class or SVG markup |
| `Badge` | `string?` | Optional badge text (e.g. notification count) |
| `Children` | `IEnumerable<NavItem>` | Nested items for dropdown/sub-menu |

## Basic usage — top bar

```razor
<BclNavigation Variant="topbar" Items="@navItems" ActiveHref="@currentPath">
    <LogoContent>
        <img src="/logo.svg" alt="Acme" height="32" />
    </LogoContent>
</BclNavigation>

@code {
    private string currentPath => NavigationManager.Uri;

    private List<NavItem> navItems = new()
    {
        new NavItem { Label = "Dashboard", Href = "/dashboard", Icon = "icon-home" },
        new NavItem { Label = "Reports",   Href = "/reports",   Icon = "icon-chart" },
        new NavItem { Label = "Settings",  Href = "/settings",  Icon = "icon-cog" },
    };
}
```

## Sidebar with collapse

```razor
<BclNavigation Variant="sidebar"
               Items="@sidebarItems"
               @bind-Collapsed="sidebarCollapsed"
               ActiveHref="@currentPath">
    <FooterContent>
        <BclButton Variant="ghost" OnClick="@Logout">Sign out</BclButton>
    </FooterContent>
</BclNavigation>

@code {
    private bool sidebarCollapsed = false;
    private async Task Logout() => await AuthService.SignOutAsync();
}
```

## Breadcrumb

```razor
<BclNavigation Variant="breadcrumb" Items="@breadcrumbs" />

@code {
    private List<NavItem> breadcrumbs = new()
    {
        new NavItem { Label = "Home",     Href = "/" },
        new NavItem { Label = "Products", Href = "/products" },
        new NavItem { Label = "Edit",     Href = "" }, // current page — no link
    };
}
```

## Accessibility

- The component renders a `<nav>` element with a unique `aria-label` (e.g. `"Main navigation"`) to distinguish it from other landmarks on the page.
- The active link has `aria-current="page"`.
- The breadcrumb variant uses `aria-label="Breadcrumb"` on the `<nav>` and marks the last item with `aria-current="page"`.
- Dropdown sub-menus must have `aria-expanded` and be keyboard-navigable with arrow keys.
- The sidebar collapse toggle needs a descriptive `aria-label` (e.g. `"Collapse sidebar"`).

## Theming

```css
:root {
    /* Top bar */
    --bcl-nav-topbar-bg:        #1e293b;
    --bcl-nav-topbar-text:      #f1f5f9;
    --bcl-nav-topbar-height:    4rem;
    --bcl-nav-link-active-text: #38bdf8;
    --bcl-nav-link-hover-bg:    rgba(255, 255, 255, 0.08);

    /* Sidebar */
    --bcl-nav-sidebar-bg:       #0f172a;
    --bcl-nav-sidebar-text:     #cbd5e1;
    --bcl-nav-sidebar-width:    16rem;
    --bcl-nav-sidebar-width-collapsed: 4rem;

    /* Breadcrumb */
    --bcl-nav-breadcrumb-text:        #64748b;
    --bcl-nav-breadcrumb-active-text: #1e293b;
    --bcl-nav-breadcrumb-separator:   "/";
}
```
