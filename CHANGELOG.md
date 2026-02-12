# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2026-01-28

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
