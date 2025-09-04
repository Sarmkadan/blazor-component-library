# Changelog

All notable changes to the Blazor Component Library are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-09-22

### Added
- Stable public API — all interfaces frozen under semantic versioning guarantees
- Event bus system for inter-component communication (`IEventPublisher`, `EventBus`)
- Rate limiting middleware for API protection (`RateLimitingMiddleware`)
- Background task service for async operations (`BackgroundTaskService`)
- Webhook handler for external integrations (`WebhookHandler`)
- HTTP client factory with configurable retry policies (`HttpClientFactory`)
- Complete examples directory with six working Razor samples
- Docker and docker-compose support for containerised demo
- CI/CD pipeline with GitHub Actions (build, CodeQL, NuGet publish)
- Full API reference, architecture, deployment, and FAQ documentation

### Changed
- Promoted all interfaces from `internal` to `public` for downstream extensibility
- Consolidated service registration under a single `AddBlazorComponentLibrary()` extension
- Improved error messages and structured logging throughout

### Fixed
- Memory leak in `CacheService` cleanup on application shutdown
- Race condition in concurrent `DataRepository` writes
- Null reference in `ModalConfig` when `OnClose` callback was not set

## [0.9.1] - 2025-08-11

### Fixed
- Off-by-one error in `GetPagedDataAsync` returning wrong page boundaries
- UTF-8 BOM in CSV export causing Excel import failures
- Cache key collision when table names share a prefix

## [0.9.0] - 2025-07-28

### Added
- Data export to CSV, JSON, and XML via `FormatterFactory`
- `CsvFormatter`, `JsonFormatter`, `XmlFormatter` implementations
- `ExportFormat` enum for type-safe format selection
- `IFormatter` abstraction for custom export formats

### Changed
- `DataService.ExportToFormatAsync` now delegates to `FormatterFactory` instead of inline string building

## [0.8.0] - 2025-06-30

### Added
- Distributed cache abstraction (`ICacheService`, `CacheService`, `CacheKeyGenerator`)
- `CacheHelper` utility class for common cache patterns
- Configurable TTL support via `CacheDurationMinutes` option
- `EnableCaching` feature flag in `LibraryOptions`

### Changed
- `DataService` and `UserService` now use `ICacheService` for repeat reads
- Reduced cold-read latency by caching repository results for 30 minutes by default

## [0.7.0] - 2025-06-02

### Added
- User management and authentication (`UserService`, `UserRepository`)
- Password hashing with `CryptographyHelper`
- Role-based access control (`UserRole` enum, `HasPermissionAsync`)
- `IUserRepository` and `IUserService` interfaces

### Fixed
- `AuthenticateAsync` returning a non-null result on empty password strings

## [0.6.0] - 2025-05-05

### Added
- Full middleware pipeline: `ExceptionHandlingMiddleware`, `LoggingMiddleware`, `RateLimitingMiddleware`, `RequestValidationMiddleware`
- `ServiceRegistry` for programmatic service introspection
- `ResultPatternExtensions` and `DataServiceExtensions` helpers
- `ProgramExtensions` for one-line `app.UseBlazorComponentLibrary()` setup

### Changed
- Exception handling centralised in middleware — services no longer catch and swallow errors
- Request validation moved out of controllers into `RequestValidationMiddleware`

## [0.5.0] - 2025-04-07

### Added
- Theme management with light/dark mode (`Theme`, `ThemeService`, `ThemeRepository`)
- CSS variable generation from theme properties (`GenerateCssVariablesAsync`)
- Chart dataset management (`ChartDataset`, `ChartType` enum — 8 chart types)
- `ThemeController` HTTP endpoint

### Changed
- `ComponentConfig.Metadata` changed from `object` to `Dictionary<string, string>` for serialisation compatibility

## [0.4.0] - 2025-03-10

### Added
- Form field configuration and validation (`FormField`, `FormFieldType`, `FormService`, `FormRepository`)
- `ValidateFormAsync` returning structured `FormValidationResult` with per-field errors
- Custom validation rule support via `ValidationRules` dictionary on `FormField`
- `FormController` HTTP endpoint
- `ValidationHelper` utility for common validation patterns

### Fixed
- `FormField.IsRequired` not enforced when `ValidationRules` dictionary was empty

## [0.3.0] - 2025-02-17

### Added
- Repository pattern with interfaces (`IComponentRepository`, `IDataRepository`, `IFormRepository`)
- In-memory repository implementations with full CRUD
- `ComponentConfig` and `DataTableRow` pagination (`GetPagedDataAsync`)
- Dependency injection integration (`ServiceConfiguration`, `AddBlazorComponentLibrary()`)
- Application constants (`ApplicationConstants`) and custom exceptions (`ComponentLibraryException`)

### Changed
- Services no longer manage their own in-memory state — all storage delegated to repositories

## [0.2.0] - 2025-01-27

### Added
- Service layer: `ComponentService`, `DataService` with business logic and validation
- `Result<T>` and `Result` types for standardised operation outcomes
- `StringHelper`, `DateTimeHelper`, `CollectionHelper` utility classes
- `DataTableController` and `ComponentController` HTTP endpoints
- `ModalConfig` model with configurable variants and callbacks

### Changed
- `DataTableColumn` sort order changed from `bool Ascending` to `SortOrder` enum

## [0.1.0] - 2025-01-06

### Added
- Initial project scaffold targeting .NET 10 with `Microsoft.NET.Sdk.Razor`
- Core domain models: `ComponentConfig`, `DataTableColumn`, `DataTableRow`, `ChartDataset`, `FormField`, `ModalConfig`, `Theme`, `User`
- `BlazorComponentLibrary.csproj` with NuGet metadata (PackageId, Authors, Description, Tags)
- MIT license and initial README
- `.editorconfig` and `.gitignore`
- xUnit test project skeleton with FluentAssertions and Moq

---

## Version Format

Version numbers follow [Semantic Versioning](https://semver.org/):

- **MAJOR** (1.x.0) - Breaking changes or significant restructuring
- **MINOR** (x.1.0) - New features, backward compatible
- **PATCH** (x.x.1) - Bug fixes and patches

## Security

Security advisories are published at:
- GitHub Security Advisories: https://github.com/sarmkadan/blazor-component-library/security/advisories

Please report vulnerabilities privately at https://sarmkadan.com — do not open public issues.

---

**Maintained by:** Vladyslav Zaiets  
**Website:** https://sarmkadan.com  
**License:** MIT
