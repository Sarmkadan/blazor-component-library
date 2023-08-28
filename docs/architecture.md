# Architecture Guide

Comprehensive overview of the Blazor Component Library architecture, design patterns, and extensibility points.

## System Design Overview

```
┌─────────────────────────────────────────────┐
│   Blazor Components (UI Layer)              │
│  DataTable, Chart, Form, Modal Components   │
└──────────────────────┬──────────────────────┘
                       │ Services Injection
┌──────────────────────▼──────────────────────┐
│   Application Services (Business Logic)     │
│  ComponentService, DataService, etc.        │
│  • Validation                               │
│  • Business Rules                           │
│  • Orchestration                            │
└──────────────────────┬──────────────────────┘
                       │ Repository Pattern
┌──────────────────────▼──────────────────────┐
│   Data Access Layer (Repositories)          │
│  IComponentRepository, IDataRepository      │
│  • CRUD Operations                          │
│  • Query Building                           │
│  • Filtering & Pagination                   │
└──────────────────────┬──────────────────────┘
                       │
┌──────────────────────▼──────────────────────┐
│   Infrastructure & Support Services        │
│  Caching, Events, Formatters, Middleware   │
│  • State Management                         │
│  • Cross-cutting Concerns                   │
│  • Utilities & Helpers                      │
└─────────────────────────────────────────────┘
```

## Layered Architecture

### 1. Presentation Layer (Components)

**Location:** Blazor `.razor` files in consuming applications

**Responsibilities:**
- User interaction and input collection
- Rendering data in UI
- Calling services via dependency injection
- State management for UI-specific data

**Example:**
```csharp
@page "/users"
@inject DataService DataService

<table>
    @foreach (var row in rows)
    {
        <tr>
            <td>@row.Values["Name"]</td>
        </tr>
    }
</table>

@code {
    private List<DataTableRow> rows = new();
    protected override async Task OnInitializedAsync()
    {
        rows = await DataService.GetTableDataAsync("users");
    }
}
```

### 2. Application Services Layer

**Location:** `Services/` directory

**Core Services:**

| Service | Responsibility |
|---------|-----------------|
| **ComponentService** | Component CRUD, search, statistics |
| **DataService** | Table/chart data management |
| **FormService** | Field validation, form logic |
| **ThemeService** | Theme creation, CSS variable generation |
| **UserService** | Authentication, user management |

**Key Characteristics:**
- Stateless, testable methods
- Async/await for all I/O
- Exception handling and logging
- Business rule validation
- Dependency on repositories

**Example:**
```csharp
public class ComponentService
{
    private readonly IComponentRepository _repository;
    private readonly ILogger<ComponentService> _logger;

    public async Task<ComponentConfig> CreateComponentAsync(ComponentConfig config)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(config.Name))
            throw new ArgumentNullException(nameof(config.Name));

        _logger.LogInformation("Creating component: {Name}", config.Name);

        // Call repository
        return await _repository.CreateAsync(config);
    }
}
```

### 3. Data Access Layer (Repositories)

**Location:** `Repositories/` directory

**Patterns:**
- **Repository Pattern** - Abstraction over data storage
- **Async Operations** - All I/O is async
- **In-Memory Default** - Swappable with EF Core, Dapper, etc.

**Example Interface:**
```csharp
public interface IComponentRepository
{
    Task<ComponentConfig> CreateAsync(ComponentConfig config);
    Task<ComponentConfig?> GetByIdAsync(int id);
    Task<IEnumerable<ComponentConfig>> GetAllAsync();
    Task UpdateAsync(ComponentConfig config);
    Task DeleteAsync(int id);
}
```

**In-Memory Implementation:**
```csharp
public class ComponentRepository : IComponentRepository
{
    private static readonly Dictionary<int, ComponentConfig> Store = new();
    private static int _nextId = 1;

    public async Task<ComponentConfig> CreateAsync(ComponentConfig config)
    {
        config.Id = _nextId++;
        Store[config.Id] = config;
        return await Task.FromResult(config);
    }

    public async Task<ComponentConfig?> GetByIdAsync(int id)
    {
        Store.TryGetValue(id, out var config);
        return await Task.FromResult(config);
    }

    // ... other methods
}
```

**Swapping with EF Core:**
```csharp
public class EfComponentRepository : IComponentRepository
{
    private readonly DbContext _context;

    public async Task<ComponentConfig> CreateAsync(ComponentConfig config)
    {
        _context.Components.Add(config);
        await _context.SaveChangesAsync();
        return config;
    }

    // ... other methods
}
```

### 4. Infrastructure Layer

**Subsystems:**

#### Caching (`Caching/` directory)
- Abstraction over cache implementation
- Key generation helpers
- TTL management

#### Events (`Events/` directory)
- Event bus for decoupled communication
- Publisher-subscriber pattern
- Event routing

#### Middleware (`Middleware/` directory)
- Request pipeline components
- Cross-cutting concerns
- Exception handling, logging, rate limiting

#### Formatters (`Formatters/` directory)
- Data transformation (CSV, JSON, XML)
- Factory pattern for format selection

#### Utilities (`Utilities/` directory)
- String, DateTime, Collection helpers
- Validation and cryptography utilities
- Reusable logic

## Design Patterns

### 1. Repository Pattern

Provides abstraction over data access:

```csharp
// Depends on abstraction, not implementation
public class ComponentService
{
    private readonly IComponentRepository _repository;

    // Can be in-memory, EF Core, Dapper, MongoDB, etc.
}
```

**Benefits:**
- Testable without database
- Easy to swap implementations
- Centralized data access logic

### 2. Dependency Injection

All dependencies resolved at runtime:

```csharp
// Registration
services.AddScoped<IComponentRepository, ComponentRepository>();
services.AddScoped<ComponentService>();

// Resolution in components
@inject ComponentService ComponentService
```

**Benefits:**
- Loose coupling
- Testability
- Composable configuration

### 3. Service Layer

Business logic encapsulated in services:

```csharp
// Services contain business rules
public class ComponentService
{
    public async Task<ComponentConfig> CreateComponentAsync(ComponentConfig config)
    {
        // Validation
        ValidateComponentName(config.Name);

        // Business logic
        var existing = await _repository.GetByNameAsync(config.Name);
        if (existing != null)
            throw new ConflictException("Component already exists");

        return await _repository.CreateAsync(config);
    }
}
```

### 4. Result Pattern

Standardized operation results:

```csharp
public class Result
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public object? Data { get; set; }
}

// Usage
var result = new Result
{
    IsSuccess = true,
    Data = component
};
```

**Benefits:**
- Consistent API responses
- Error information included
- No exceptions for expected failures

### 5. Factory Pattern

Creating objects without specifying exact classes:

```csharp
public class FormatterFactory
{
    public static IFormatter CreateFormatter(ExportFormat format)
    {
        return format switch
        {
            ExportFormat.Csv => new CsvFormatter(),
            ExportFormat.Json => new JsonFormatter(),
            ExportFormat.Xml => new XmlFormatter(),
            _ => throw new ArgumentException()
        };
    }
}
```

### 6. Event-Driven Architecture

Decoupled communication between components:

```csharp
// Publish event
await _eventBus.PublishAsync(new ComponentCreatedEvent 
{ 
    ComponentId = 1, 
    Timestamp = DateTime.UtcNow 
});

// Subscribe to event
_eventBus.Subscribe<ComponentCreatedEvent>(async (e) =>
{
    _logger.LogInformation("Component created: {Id}", e.ComponentId);
});
```

## Extension Points

### Custom Services

Implement additional business logic:

```csharp
public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly SmtpClient _client;

    public async Task SendAsync(string to, string subject, string body)
    {
        // Implementation
    }
}

// Register
services.AddScoped<IEmailService, EmailService>();
```

### Custom Repositories

Swap storage implementation:

```csharp
public class MongoDbComponentRepository : IComponentRepository
{
    private readonly IMongoCollection<ComponentConfig> _collection;

    public async Task<ComponentConfig> CreateAsync(ComponentConfig config)
    {
        await _collection.InsertOneAsync(config);
        return config;
    }
}

// Register instead of default
services.AddScoped<IComponentRepository, MongoDbComponentRepository>();
```

### Custom Validators

Add domain-specific validation:

```csharp
public class ComponentNameValidator
{
    public void Validate(string name)
    {
        if (!Regex.IsMatch(name, @"^[a-zA-Z0-9_-]+$"))
            throw new ArgumentException("Invalid component name");

        if (name.Length > 100)
            throw new ArgumentException("Name too long");
    }
}
```

### Custom Middleware

Add request pipeline processing:

```csharp
public class PerformanceTrackingMiddleware : IMiddleware
{
    private readonly ILogger<PerformanceTrackingMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();

        await next(context);

        stopwatch.Stop();
        _logger.LogInformation(
            "Request {Path} completed in {Ms}ms",
            context.Request.Path,
            stopwatch.ElapsedMilliseconds
        );
    }
}

// Register in Program.cs
app.UseMiddleware<PerformanceTrackingMiddleware>();
```

### Custom Formatters

Add data export formats:

```csharp
public class XlsxFormatter : IFormatter
{
    public string Format(List<DataTableRow> data)
    {
        // Use EPPlus or similar library
        // Return base64 encoded Excel file
    }
}
```

## Error Handling Strategy

### Exception Hierarchy

```
ComponentLibraryException (base)
├── InvalidComponentException       // Bad input
├── ComponentNotFoundException      // Resource missing
├── FormValidationException        // Form validation failed
├── UnauthorizedException         // Auth required
├── ForbiddenException            // Insufficient permissions
├── ConflictException             // Resource conflict
└── MissingDependencyException    // Service not registered
```

### Error Handling Example

```csharp
public async Task<ComponentConfig> GetComponentAsync(int id)
{
    try
    {
        var component = await _repository.GetByIdAsync(id);
        if (component == null)
            throw new ComponentNotFoundException($"Component {id} not found");

        return component;
    }
    catch (DatabaseException ex)
    {
        _logger.LogError(ex, "Database error retrieving component");
        throw new ComponentLibraryException("Failed to retrieve component", ex);
    }
}
```

## Data Flow Example

### Creating a Component

1. **UI Layer** - User clicks "Create Component" button
2. **Blazor Component** - Calls `ComponentService.CreateComponentAsync(config)`
3. **Service Layer** - Validates input, applies business rules
4. **Repository Layer** - Persists to storage
5. **Infrastructure** - May publish `ComponentCreatedEvent`
6. **UI Update** - Component re-renders with new data

```csharp
// Step 1-2: UI calls service
var component = await ComponentService.CreateComponentAsync(config);

// Step 3: Service validates and orchestrates
public async Task<ComponentConfig> CreateComponentAsync(ComponentConfig config)
{
    ValidateComponentName(config.Name);                    // Business rule
    var existing = await _repository.GetByNameAsync(config.Name);
    if (existing != null) throw new ConflictException();
    
    var created = await _repository.CreateAsync(config);   // Step 4
    
    await _eventBus.PublishAsync(                          // Step 5
        new ComponentCreatedEvent { ComponentId = created.Id }
    );
    
    return created;
}

// Step 5: Event subscribers notified
_eventBus.Subscribe<ComponentCreatedEvent>(async (e) =>
{
    _logger.LogInformation("Component created: {Id}", e.ComponentId);
    // Could trigger notifications, cache invalidation, etc.
});
```

## Performance Considerations

### Caching Strategy

```csharp
public async Task<ComponentConfig?> GetComponentAsync(int id)
{
    // Try cache first
    var cacheKey = $"component:{id}";
    var cached = await _cacheService.GetAsync<ComponentConfig>(cacheKey);
    if (cached != null) return cached;

    // Load from repository
    var component = await _repository.GetByIdAsync(id);
    if (component != null)
    {
        // Cache for 1 hour
        await _cacheService.SetAsync(
            cacheKey, 
            component, 
            TimeSpan.FromHours(1)
        );
    }

    return component;
}
```

### Pagination for Large Datasets

```csharp
// Don't load all data at once
var allData = await _repository.GetAllAsync();  // ❌ Bad

// Use pagination
var page = await _repository.GetPageAsync(
    pageNumber: 1,
    pageSize: 50
);  // ✅ Good
```

### Async Operations

```csharp
// Non-blocking I/O throughout
public async Task<List<Component>> GetComponentsAsync()
{
    return await _repository.GetAllAsync();  // ✅ Async
}

public List<Component> GetComponents()
{
    return _repository.GetAll().Result;  // ❌ Blocks thread
}
```

## Testing Strategy

The architecture supports unit testing without external dependencies:

```csharp
public class ComponentServiceTests
{
    private readonly Mock<IComponentRepository> _repositoryMock;
    private readonly ComponentService _service;

    public ComponentServiceTests()
    {
        _repositoryMock = new Mock<IComponentRepository>();
        _service = new ComponentService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateComponent_WithValidConfig_ReturnComponent()
    {
        // Arrange
        var config = new ComponentConfig { Name = "Test" };
        _repositoryMock.Setup(r => r.CreateAsync(config))
            .ReturnsAsync(new ComponentConfig { Id = 1, Name = "Test" });

        // Act
        var result = await _service.CreateComponentAsync(config);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }
}
```

---

This architecture provides a solid foundation for building scalable, maintainable Blazor applications. The layering ensures separation of concerns, and the patterns enable easy testing and extension.
