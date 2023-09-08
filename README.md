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

## Documentation

- [Migration Guide](docs/MIGRATION_v2.md) - Upgrading from v1.x to v2.0
- [Changelog](CHANGELOG.md) - Release notes and version history

## Contributing

Contributions are welcome. See [CONTRIBUTING.md](CONTRIBUTING.md) for development setup and guidelines.

## License

MIT License. See [LICENSE](LICENSE) for details.
