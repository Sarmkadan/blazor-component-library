# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- `ToastOptions` (`DefaultDuration`, `MaxVisibleToasts`, `Position`, `PauseOnHover`), configured through
  `AddBlazorComponentLibrary(configure, configureToast)` and validated at host start with `ValidateOnStart`
- `LibraryComponentBase`, the shared base for every component. It provides `Class` (extra CSS classes on the root element)
  and `AdditionalAttributes` (unmatched attributes such as `aria-label` or `data-*` are splatted onto the root element)
- `ComponentConventionTests` enforce the shared parameter conventions over every public component

### Changed
- `IToastService.Show` and the `ToastContainer` extension methods take `int? durationMs = null`; `null` uses `ToastOptions.DefaultDuration`
- `ToastContainer` parameters `Position`, `MaxVisible` and `PauseOnHover` fall back to `ToastOptions` when not supplied
- Removed the public `ToastService.DefaultDurationMs` constant
- **Breaking:** `ThemeSwitcher` and `DragDropList` parameter `CssClass` is renamed to `Class`
- **Breaking:** `Pagination.PageChanged` is renamed to `CurrentPageChanged` so `@bind-CurrentPage` works
- **Breaking:** `DragDropList.OnOrderChanged` is renamed to `ItemsChanged` so `@bind-Items` works
- **Breaking:** `Tabs.OnActiveIndexChanged` is removed; `ActiveIndexChanged` already fires for the same event

### Fixed
- `PauseOnHover` had no effect because its conditional attribute was not emitted as Razor control flow
- Restored `ToastContainer.razor` markup, which had been truncated to a stray `RenderFragment` line
- `TabPanel` inline style had a stray `>` after the `display` value, so the browser ignored it and inactive panels stayed visible

## [2.0.0] - 2026-03-28

### Added
- Add virtualized data grid with sorting, filtering, and inline editing
- Docker support with multi-stage builds
- Health check endpoints (/health, /health/ready)
- Integration test suite with xUnit
- Migration guide from v1.x

### Changed
- Upgraded to .NET 10.0
- Modern C# features (records, primary constructors)
- Improved API consistency

### Fixed
- Various edge cases found through testing

## [1.1.0] - 2026-01-28

### Added
- Docker containerization support with multi-stage builds
- docker-compose.yml for easy local development and deployment
- HEALTHCHECK in Docker image for monitoring container health
- .NET 10 support with updated NuGet dependencies
- Migration guide (docs/MIGRATION_v2.md) for upgrading from v1.x

### Changed
- Upgraded target framework from .NET 8.0 to .NET 10.0
- Updated all NuGet packages to .NET 10 compatible versions
- Improved Dockerfile with optimized build stages and runtime

### Technical Details
- Multi-stage Docker build: SDK stage for compilation, ASP.NET runtime for execution
- Health endpoint support at `/health`
- Container port: 8080 (ASPNETCORE_URLS=http://+:8080)
- Auto-restart policy in docker-compose.yml

## [1.0.0] - 2025-01-01

### Initial Release
- Blazor component library foundation
- Basic component infrastructure
- .NET 8.0 support
