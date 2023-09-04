# Migration Guide: v1.x to v2.0

## Overview
Version 2.0 introduces Docker containerization support and upgrades to .NET 10. This guide walks you through the changes and migration steps.

## Breaking Changes

### Framework Upgrade
- **Minimum .NET version**: .NET 10.0 (from .NET 8.0)
- All projects must target `net10.0` in their .csproj files
- Update all NuGet packages to versions compatible with .NET 10

### API Changes
- Component base classes remain compatible with v1.x
- Event handlers continue to work without modification
- Parameter binding is unchanged

## Step-by-Step Migration

### 1. Update Project File

Change your .csproj `TargetFramework`:

```xml
<!-- Before -->
<TargetFramework>net8.0</TargetFramework>

<!-- After -->
<TargetFramework>net10.0</TargetFramework>
```

### 2. Update NuGet Dependencies

Run the following commands to update packages:

```bash
dotnet package update
dotnet restore
```

### 3. Docker Deployment

#### Building the Docker Image

```bash
docker build -t blazor-component-library:2.0.0 .
```

#### Running with Docker Compose

```bash
docker-compose up -d
```

The application will be available at `http://localhost:8080`.

#### Health Check

The container includes a health check endpoint at `/health`. Docker will automatically restart unhealthy containers.

### 4. Environment Configuration

Set environment variables when running in Docker:

```bash
docker run -e ASPNETCORE_ENVIRONMENT=Production \
           -e ASPNETCORE_URLS=http://+:8080 \
           -p 8080:8080 \
           blazor-component-library:2.0.0
```

## Verification

After migration, verify:

1. Project builds successfully with `dotnet build`
2. Tests pass: `dotnet test`
3. Docker image builds: `docker build -t blazor-component-library:2.0.0 .`
4. Container starts and health check passes:
   ```bash
   docker run --health-cmd='curl -f http://localhost:8080/health' \
              -p 8080:8080 \
              blazor-component-library:2.0.0
   ```

## Known Issues

- No known compatibility issues between v1.x and v2.0

## Support

For issues during migration, check the CHANGELOG.md for detailed release notes.
