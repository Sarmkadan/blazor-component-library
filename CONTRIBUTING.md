# Contributing to Blazor Component Library

Thank you for your interest in contributing to the Blazor Component Library! This document provides guidelines and instructions for contributing.

## Code of Conduct

- Be respectful and inclusive
- Focus on constructive feedback
- Help others learn and grow
- Report issues professionally

## Ways to Contribute

### 1. Report Bugs

Found a bug? Please report it by opening a GitHub issue with:

- **Title:** Clear description of the bug
- **Reproduction:** Steps to reproduce the issue
- **Expected behavior:** What should happen
- **Actual behavior:** What actually happens
- **Environment:** .NET version, OS, browser
- **Screenshots:** If applicable

**Example:**
```
Title: DataTable export fails with special characters

Steps to reproduce:
1. Create a table with data containing ampersands (&)
2. Export to CSV format
3. Observe file corruption

Expected: CSV file should contain escaped characters
Actual: CSV file is corrupted
```

### 2. Request Features

Have an idea for improvement? Open a GitHub discussion:

- **Title:** Feature request summary
- **Problem:** What problem does it solve?
- **Solution:** How should it work?
- **Alternative:** Any alternatives considered?

### 3. Fix Bugs

Want to fix a bug?

1. Fork the repository
2. Create a feature branch: `git checkout -b fix/issue-description`
3. Make your changes
4. Add tests if applicable
5. Commit with clear message: `git commit -m 'Fix: description'`
6. Push to your fork: `git push origin fix/issue-description`
7. Open a Pull Request

### 4. Add Features

Implementing a new feature?

1. Open a discussion first to get feedback
2. Fork the repository
3. Create a feature branch: `git checkout -b feature/feature-name`
4. Follow code style guidelines
5. Add tests for new functionality
6. Add documentation
7. Commit and push your changes
8. Open a Pull Request

### 5. Improve Documentation

Documentation improvements are always welcome:

- Fix typos and clarify explanations
- Add examples and use cases
- Update outdated information
- Translate to other languages

## Development Setup

### Prerequisites

- .NET 10 SDK
- Visual Studio 2022 or VS Code
- Git

### Clone Repository

```bash
git clone https://github.com/sarmkadan/blazor-component-library.git
cd blazor-component-library
```

### Build and Test

```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Format code
dotnet format

# Check code quality
dotnet build --configuration Release
```

## Code Style Guidelines

### C# Conventions

**Naming Conventions:**
- `PascalCase` for classes, methods, properties, public members
- `camelCase` for local variables, parameters
- `_camelCase` for private fields
- `UPPER_CASE` for constants

**Example:**
```csharp
public class ComponentService
{
    private readonly IComponentRepository _repository;
    private const int DefaultPageSize = 25;

    public async Task<ComponentConfig> CreateComponentAsync(ComponentConfig config)
    {
        var createdComponent = await _repository.CreateAsync(config);
        return createdComponent;
    }
}
```

### Code Structure

**File Organization:**
```csharp
// 1. Using statements
using System;
using BlazorComponentLibrary.Models;

// 2. Namespace
namespace BlazorComponentLibrary.Services
{
    // 3. Class declaration
    public class ComponentService
    {
        // 4. Fields
        private readonly IComponentRepository _repository;

        // 5. Constructor
        public ComponentService(IComponentRepository repository)
        {
            _repository = repository;
        }

        // 6. Public methods
        public async Task<ComponentConfig> GetComponentAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        // 7. Private methods
        private void ValidateInput(ComponentConfig config)
        {
            // Validation logic
        }
    }
}
```

### Documentation Comments

```csharp
/// <summary>
/// Creates a new component configuration.
/// </summary>
/// <param name="config">The component configuration to create.</param>
/// <returns>The created component with assigned ID.</returns>
/// <exception cref="ArgumentNullException">Thrown when config is null.</exception>
public async Task<ComponentConfig> CreateComponentAsync(ComponentConfig config)
{
    // Implementation
}
```

### Error Handling

```csharp
// ✅ Good - specific exception handling
try
{
    result = await _repository.GetByIdAsync(id);
}
catch (ComponentNotFoundException ex)
{
    _logger.LogWarning("Component not found: {Id}", id);
    throw;
}

// ❌ Bad - too broad
try
{
    result = await _repository.GetByIdAsync(id);
}
catch (Exception)
{
    // Hide all errors
}
```

### Async/Await

```csharp
// ✅ Good - always async
public async Task<List<Component>> GetComponentsAsync()
{
    return await _repository.GetAllAsync();
}

// ❌ Bad - blocking
public List<Component> GetComponents()
{
    return _repository.GetAll().Result;
}
```

## Pull Request Process

### Before Submitting

1. **Test your changes**
   ```bash
   dotnet build
   dotnet test
   dotnet format
   ```

2. **Update documentation**
   - Add/update code comments
   - Update relevant documentation files
   - Add examples if applicable

3. **Commit messages**
   - Use imperative mood: "Add feature" not "Added feature"
   - Reference issues: "Fixes #123"
   - Be descriptive but concise

   **Examples:**
   ```
   Add caching support for components
   Fix: DataTable export with special characters
   Docs: Update API reference for v1.2.0
   ```

### PR Description Template

```markdown
## Description
Brief description of changes.

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Documentation update
- [ ] Performance improvement

## Related Issues
Fixes #(issue number)

## Testing
- [ ] Unit tests added/updated
- [ ] Manual testing completed
- [ ] No breaking changes

## Checklist
- [ ] Code follows style guidelines
- [ ] Documentation updated
- [ ] Tests pass locally
- [ ] No new warnings introduced
```

### Review Process

1. **Automated Checks**
   - Build status
   - Test coverage
   - Code quality

2. **Code Review**
   - Maintainer reviews code
   - Feedback provided
   - Requested changes made

3. **Merge**
   - Approved and merged
   - Included in next release

## Testing Guidelines

### Unit Tests

```csharp
[TestClass]
public class ComponentServiceTests
{
    private Mock<IComponentRepository> _repositoryMock;
    private ComponentService _service;

    [TestInitialize]
    public void Setup()
    {
        _repositoryMock = new Mock<IComponentRepository>();
        _service = new ComponentService(_repositoryMock.Object);
    }

    [TestMethod]
    public async Task CreateComponent_WithValidConfig_ReturnComponent()
    {
        // Arrange
        var config = new ComponentConfig { Name = "Test" };
        _repositoryMock
            .Setup(r => r.CreateAsync(config))
            .ReturnsAsync(new ComponentConfig { Id = 1, Name = "Test" });

        // Act
        var result = await _service.CreateComponentAsync(config);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
    }
}
```

### Integration Tests

```csharp
[TestClass]
public class ComponentServiceIntegrationTests
{
    private ComponentService _service;
    private IComponentRepository _repository;

    [TestInitialize]
    public void Setup()
    {
        _repository = new InMemoryComponentRepository();
        _service = new ComponentService(_repository);
    }

    [TestMethod]
    public async Task FullWorkflow_CreateUpdateDelete()
    {
        // Test complete workflow
    }
}
```

## Documentation Guidelines

### README.md

- Clear project description
- Installation instructions
- Quick start guide
- Feature overview
- Links to detailed docs

### Code Comments

- Explain WHY, not WHAT
- Keep comments up-to-date
- Use clear language
- Link to related code

```csharp
// ✅ Good - explains why
// Cache for 1 hour to reduce database queries
var cached = await _cacheService.GetAsync<Component>(key);

// ❌ Bad - just describes what code does
// Get from cache
var cached = await _cacheService.GetAsync<Component>(key);
```

## Release Process

### Version Numbers

Follow [Semantic Versioning](https://semver.org/):
- **MAJOR.MINOR.PATCH** (1.2.0)
- MAJOR: Breaking changes
- MINOR: New features (backward compatible)
- PATCH: Bug fixes

### Changelog

Update `CHANGELOG.md`:
```markdown
## [1.2.0] - 2026-05-04

### Added
- New feature description

### Changed
- Changed behavior description

### Fixed
- Fixed bug description
```

## Community

### Questions?

- Open a GitHub Discussion
- Check the [FAQ](docs/faq.md)
- Visit https://sarmkadan.com

### Stay Updated

- Watch the repository
- Subscribe to releases
- Follow the maintainer

## Recognition

Contributors will be:
- Listed in README contributors section
- Credited in release notes
- Mentioned in CHANGELOG

---

## Summary

1. **Fork and clone** the repository
2. **Create a branch** for your changes
3. **Follow code style** guidelines
4. **Add tests** for new functionality
5. **Update documentation**
6. **Submit a pull request** with clear description

Thank you for contributing! 🎉

For more information, see:
- [Getting Started](docs/getting-started.md)
- [Architecture](docs/architecture.md)
- [Code of Conduct](CODE_OF_CONDUCT.md)

---

**Maintained by:** Vladyslav Zaiets  
**Website:** https://sarmkadan.com
