# =============================================================================
# Author: Vladyslav Zaiets | https://sarmkadan.com
# CTO & Software Architect
# =============================================================================
# Makefile for Blazor Component Library
# Provides convenient build and development commands

.PHONY: help build test clean publish docker-build docker-up docker-down docs lint format

# Default target
help:
	@echo "Blazor Component Library - Available Commands"
	@echo ""
	@echo "Usage: make [target]"
	@echo ""
	@echo "Targets:"
	@echo "  build        - Build the project"
	@echo "  test         - Run all tests"
	@echo "  clean        - Clean build artifacts"
	@echo "  restore      - Restore NuGet packages"
	@echo "  pack         - Create NuGet package"
	@echo "  publish      - Publish to NuGet (requires API key)"
	@echo "  format       - Format code with dotnet-format"
	@echo "  lint         - Run code quality checks"
	@echo "  docker-build - Build Docker image"
	@echo "  docker-up    - Start Docker containers"
	@echo "  docker-down  - Stop Docker containers"
	@echo "  docs         - Generate documentation"
	@echo "  version      - Show version information"
	@echo ""

# Restore NuGet packages
restore:
	@echo "Restoring NuGet packages..."
	dotnet restore

# Build the project
build: restore
	@echo "Building project..."
	dotnet build --configuration Release

# Run tests
test: build
	@echo "Running tests..."
	dotnet test --configuration Release --verbosity normal --no-build

# Clean build artifacts
clean:
	@echo "Cleaning build artifacts..."
	dotnet clean
	rm -rf bin/
	rm -rf obj/
	rm -rf dist/
	@echo "Clean complete."

# Create NuGet package
pack: build
	@echo "Creating NuGet package..."
	dotnet pack --configuration Release --no-build --output dist/

# Format code
format:
	@echo "Formatting code..."
	dotnet format
	@echo "Format complete."

# Run code quality checks
lint:
	@echo "Running code quality checks..."
	dotnet build --configuration Release
	@echo "Lint complete."

# Build Docker image
docker-build:
	@echo "Building Docker image..."
	docker build -t blazor-component-library:latest .
	@echo "Docker build complete."

# Start Docker containers
docker-up:
	@echo "Starting Docker containers..."
	docker-compose up -d
	@echo "Containers started. Access at http://localhost:5000"

# Stop Docker containers
docker-down:
	@echo "Stopping Docker containers..."
	docker-compose down
	@echo "Containers stopped."

# Generate documentation
docs:
	@echo "Documentation files are located in the docs/ directory"
	@echo "- docs/getting-started.md"
	@echo "- docs/architecture.md"
	@echo "- docs/api-reference.md"
	@echo "- docs/faq.md"
	@echo ""
	@echo "View README.md for comprehensive documentation"

# Show version
version:
	@dotnet --version
	@echo ""
	@echo "Project: Blazor Component Library"
	@echo "Version: 1.2.0"
	@echo "Author: Vladyslav Zaiets"
	@echo "Website: https://sarmkadan.com"

# Development targets (without .PHONY declaration)
dev-watch:
	@echo "Watching for changes (press Ctrl+C to stop)..."
	dotnet watch build

dev-test-watch:
	@echo "Watching for test changes (press Ctrl+C to stop)..."
	dotnet watch test

# CI/CD targets
ci-build: clean restore build test lint
	@echo "CI build complete."

ci-pack: ci-build pack
	@echo "CI pack complete."

# Default target if no arguments provided
.DEFAULT_GOAL := help
