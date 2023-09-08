# Blazor Component Library

![CI](https://github.com/sarmkadan/blazor-component-library/actions/workflows/ci.yml/badge.svg)
![License](https://img.shields.io/github/license/sarmkadan/blazor-component-library)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)

A reusable Blazor component library for modern web applications. Provides production-ready UI components with consistent theming, accessibility support, and responsive design out of the box.

## Features

- Blazor WebAssembly compatible components
- Consistent theming and design tokens
- Accessibility-first approach (WCAG 2.1 AA)
- Responsive layout primitives
- Lightweight with minimal dependencies
- **Theme switcher** — three-way Light / Dark / System toggle with local storage persistence
- **Toast notifications** — queued, auto-dismissing alerts with four severity levels
- **Drag-and-drop list** — reorderable lists via the HTML5 Drag and Drop API

## Requirements

- .NET 10.0 SDK or later
- Docker (for containerized deployment)

## Quick Start

### Install via NuGet

```bash
dotnet add package BlazorComponentLibrary
```

### Register in Program.cs

```csharp
builder.Services.AddBlazorComponentLibrary();
```

### Use in a Razor component

```razor
@using BlazorComponentLibrary

<BclButton Variant="ButtonVariant.Primary" OnClick="HandleClick">
    Submit
</BclButton>

<BclDataGrid Items="@items" Striped="true">
    <BclColumn Field="Name" Title="Full Name" Sortable="true" />
    <BclColumn Field="Email" Title="Email Address" />
</BclDataGrid>
```

### Local Development

```bash
git clone https://github.com/sarmkadan/blazor-component-library.git
cd blazor-component-library
dotnet restore
dotnet build
dotnet run
```

### Docker Deployment

```bash
docker-compose up -d
```

The application will be available at `http://localhost:8080`.

## Architecture

```
BlazorComponentLibrary/
  Components/         - Razor component implementations
  Themes/             - CSS/SCSS theming and design tokens
  Services/           - DI-registered services (toast, modal, theme)
  Models/             - Component parameter models and enums
  Extensions/         - ServiceCollection extension methods
  wwwroot/            - Static assets (CSS, JS interop)
```

## Components

### ThemeSwitcher

A three-way toggle (Light / System / Dark) that persists the user's choice in
`localStorage` and applies a `data-bcl-theme` attribute to `<html>` so CSS custom
properties update without a page reload.

```razor
<ThemeSwitcher ShowLabel="true" />
```

**Parameters**

| Parameter  | Type     | Default | Description                                 |
|------------|----------|---------|---------------------------------------------|
| ShowLabel  | `bool`   | `true`  | Render text label beside each icon           |
| CssClass   | `string?`| `null`  | Extra CSS class(es) for the root element     |

**Service** — inject `IThemeService` anywhere to read or change the theme
programmatically and subscribe to the `ThemeChanged` event.

```csharp
@inject IThemeService ThemeService

ThemeService.SetTheme(ThemeMode.Dark);
ThemeService.ThemeChanged += mode => Console.WriteLine($"Theme is now {mode}");
```

---

### ToastContainer / IToastService

Place `<ToastContainer />` once in your root layout. Inject `IToastService`
wherever you need to show a notification.

```razor
@* MainLayout.razor *@
<ToastContainer Position="ToastPosition.BottomRight" MaxVisible="5" />
```

```csharp
@inject IToastService Toast

Toast.Show("File saved.", ToastType.Success);
Toast.Show("Low disk space.", ToastType.Warning, durationMs: 0); // manual dismiss
```

**ToastContainer parameters**

| Parameter  | Type            | Default                       | Description                            |
|------------|-----------------|-------------------------------|----------------------------------------|
| Position   | `ToastPosition` | `BottomRight`                 | Screen corner for the toast stack      |
| MaxVisible | `int`           | `5`                           | Max simultaneously visible toasts      |

**Toast types** — `Info`, `Success`, `Warning`, `Error`

**Positions** — `TopLeft`, `TopCenter`, `TopRight`, `BottomLeft`, `BottomCenter`, `BottomRight`

---

### DragDropList&lt;TItem&gt;

A drag-and-drop reorderable list powered by the HTML5 Drag and Drop API. Fires
`OnOrderChanged` with the updated list after every successful drop.

```razor
<DragDropList TItem="string"
              Items="@_tasks"
              OnOrderChanged="@(updated => _tasks = updated.ToList())"
              Enabled="true">
    <ItemTemplate Context="task">
        <span>@task</span>
    </ItemTemplate>
</DragDropList>
```

**Parameters**

| Parameter       | Type                       | Default | Description                                    |
|-----------------|----------------------------|---------|------------------------------------------------|
| Items           | `IList<TItem>`             | `[]`    | Ordered collection of items to render           |
| ItemTemplate    | `RenderFragment<TItem>`    | —       | Template for each row                           |
| OnOrderChanged  | `EventCallback<IList<TItem>>` | —    | Fires with the reordered list after a drop      |
| Enabled         | `bool`                     | `true`  | Enable/disable drag interaction                 |
| CssClass        | `string?`                  | `null`  | Extra CSS class(es) for the root `<ul>`         |

---

## Documentation

- [Migration Guide](docs/MIGRATION_v2.md) - Upgrading from v1.x to v2.0
- [Changelog](CHANGELOG.md) - Release notes and version history

## Contributing

Contributions are welcome. See [CONTRIBUTING.md](CONTRIBUTING.md) for development setup and guidelines.

## License

MIT License. See [LICENSE](LICENSE) for details.
