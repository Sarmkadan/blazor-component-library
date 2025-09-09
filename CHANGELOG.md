# Changelog

All notable changes to the Blazor Component Library are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.0] - 2026-05-04

### Added
- Event bus system for inter-component communication
- Rate limiting middleware for API protection
- Background task service for async operations
- Webhook handler for external integrations
- Advanced logging configuration options
- Comprehensive API documentation
- Docker containerization support
- CI/CD pipeline with GitHub Actions
- Complete examples directory with 5 working samples
- FAQ and troubleshooting documentation

### Changed
- Improved service registration with fluent configuration
- Enhanced error messages for better debugging
- Optimized pagination for large datasets
- Updated dependencies to latest stable versions

### Fixed
- Memory leak in cache service cleanup
- Race condition in concurrent data access
- Null reference exception in modal rendering

### Deprecated
- Legacy `ComponentSettings` class (use `LibraryOptions` instead)

## [1.1.0] - 2026-04-01

### Added
- Form validation with custom validators
- User role-based access control (RBAC)
- Theme system with CSS variable generation
- Data export to CSV, JSON, XML formats
- In-memory caching with TTL support
- Comprehensive middleware pipeline
- Service registry for dependency management
- Result pattern for standardized responses
- HTTP client factory for API integration
- Background task scheduling

### Changed
- Refactored service layer for better separation of concerns
- Improved repository pattern implementation
- Enhanced security with password hashing
- Better error handling throughout

### Fixed
- UTF-8 encoding issues in data export
- Pagination off-by-one error
- Cache key collision in high-concurrency scenarios

## [1.0.0] - 2026-03-01

### Added
- Core component library infrastructure
- Data table models with sorting and filtering
- Chart dataset management system
- Form field configuration and models
- Modal configuration system
- User management with authentication
- Theme configuration models
- Repository pattern with in-memory storage
- Service layer for business logic
- Dependency injection integration
- Configuration extensions for Program.cs
- Custom exception hierarchy
- Application constants and utilities
- Comprehensive README and documentation

### Features
- **Models**: 8 domain model classes for all components
- **Services**: 5 core services for data management
- **Repositories**: Repository pattern with interfaces
- **Utilities**: 6 utility classes for common operations
- **Formatters**: Data export to multiple formats
- **Middleware**: Request pipeline components
- **Controllers**: 5 HTTP API endpoints
- **Integration**: External service integration support

---

## Version Format

Version numbers follow [Semantic Versioning](https://semver.org/):

- **MAJOR** (1.x.0) - Breaking changes or significant features
- **MINOR** (x.1.0) - New features, backward compatible
- **PATCH** (x.x.1) - Bug fixes and patches

## Release Schedule

- **Major releases**: Approximately annually
- **Minor releases**: Quarterly
- **Patch releases**: As needed for bug fixes
- **Security patches**: Critical security issues are patched immediately

## Upgrading

### Upgrade from 1.1.x to 1.2.0

1. Update NuGet package to latest
2. No breaking changes - existing code continues to work
3. Optional: Enable new event bus system in configuration

```csharp
services.AddBlazorComponentLibrary(options =>
{
    options.EnableEventBus = true;  // New in 1.2.0
    options.EnableRateLimiting = true;
});
```

### Upgrade from 1.0.x to 1.1.0

1. Update NuGet package
2. Replace deprecated `ComponentSettings` with `LibraryOptions`
3. Update service registration (optional - backward compatible)

## Deprecations

### Deprecated in 1.2.0
- `LegacyFormValidation` class - Use `FormService.ValidateFormAsync()` instead
- `DirectCache` class - Use `ICacheService` instead

### Will be Removed in 2.0.0
- All classes marked with `[Obsolete]` attribute

## Security

### Security Advisories

Security advisories for this project are published in:
- GitHub Security Advisories: https://github.com/sarmkadan/blazor-component-library/security/advisories
- Release notes for patch versions

### Reporting Security Issues

Please report security issues privately to the maintainer at https://sarmkadan.com

Do not open public issues for security vulnerabilities.

## Support Matrix

| Version | .NET Target | Status | Support Ends |
|---------|------------|--------|--------------|
| 1.2.x | .NET 10 | Active | 2027-05-04 |
| 1.1.x | .NET 10 | Maintenance | 2026-12-01 |
| 1.0.x | .NET 10 | EOL | 2026-06-01 |

**Status Legend:**
- **Active**: Receives new features and bug fixes
- **Maintenance**: Receives security and critical bug fixes only
- **EOL**: End of Life - no further updates

## Future Roadmap

### Planned for 1.3.0
- Real-time updates with SignalR support
- Advanced filtering with query builders
- Custom column templates for data tables
- Drag-and-drop support for components
- Accessibility improvements (WCAG 2.1)

### Planned for 2.0.0
- Blazor hybrid app support
- MAUI integration
- Full-text search capabilities
- Multi-language support (i18n)
- Advanced analytics and reporting

---

**Maintained by:** Vladyslav Zaiets  
**Website:** https://sarmkadan.com  
**License:** MIT
